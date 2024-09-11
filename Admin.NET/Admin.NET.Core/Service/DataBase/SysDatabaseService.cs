// Admin.NET 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。使用本项目应遵守相关法律法规和许可证的要求。
//
// 本项目主要遵循 MIT 许可证和 Apache 许可证（版本 2.0）进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 和 LICENSE-APACHE 文件。
//
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目二次开发而产生的一切法律纠纷和责任，我们不承担任何责任！

using Mapster.Adapters;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Npgsql;
using System.Linq;

namespace Admin.NET.Core.Service;

/// <summary>
/// 系统数据库管理服务 🧩
/// </summary>
[ApiDescriptionSettings(Order = 250)]
public class SysDatabaseService : IDynamicApiController, ITransient
{
    private readonly ISqlSugarClient _db;
    private readonly IViewEngine _viewEngine;
    private readonly CodeGenOptions _codeGenOptions;

    public SysDatabaseService(ISqlSugarClient db,
        IViewEngine viewEngine,
        IOptions<CodeGenOptions> codeGenOptions)
    {
        _db = db;
        _viewEngine = viewEngine;
        _codeGenOptions = codeGenOptions.Value;
    }

    /// <summary>
    /// 获取库列表 🔖
    /// </summary>
    /// <returns></returns>
    [DisplayName("获取库列表")]
    public List<string> GetList()
    {
        return App.GetOptions<DbConnectionOptions>().ConnectionConfigs.Select(u => u.ConfigId.ToString()).ToList();
    }

    /// <summary>
    /// 获取可视化库表结构 🔖
    /// </summary>
    /// <returns></returns>
    [DisplayName("获取可视化库表结构")]
    public VisualDbTable GetVisualDbTable()
    {
        var visualTableList = new List<VisualTable>();
        var visualColumnList = new List<VisualColumn>();
        var columnRelationList = new List<ColumnRelation>();

        // 遍历所有实体获取所有库表结构
        var random = new Random();
        var entityTypes = App.EffectiveTypes.Where(u => !u.IsInterface && !u.IsAbstract && u.IsClass && u.IsDefined(typeof(SugarTable), false)).ToList();
        foreach (var entityType in entityTypes)
        {
            var entityInfo = _db.EntityMaintenance.GetEntityInfoNoCache(entityType);

            var visualTable = new VisualTable
            {
                TableName = entityInfo.DbTableName,
                TableComents = entityInfo.TableDescription + entityInfo.DbTableName,
                X = random.Next(5000),
                Y = random.Next(5000)
            };
            visualTableList.Add(visualTable);

            foreach (EntityColumnInfo columnInfo in entityInfo.Columns)
            {
                var visualColumn = new VisualColumn
                {
                    TableName = columnInfo.DbTableName,
                    ColumnName = columnInfo.DbColumnName,
                    DataType = columnInfo.PropertyInfo.PropertyType.Name,
                    DataLength = columnInfo.Length.ToString(),
                    ColumnDescription = columnInfo.ColumnDescription,
                };
                visualColumnList.Add(visualColumn);

                // 根据导航配置获取表之间关联关系
                if (columnInfo.Navigat != null)
                {
                    var name1 = columnInfo.Navigat.GetName();
                    var name2 = columnInfo.Navigat.GetName2();
                    var relation = new ColumnRelation
                    {
                        SourceTableName = columnInfo.DbTableName,
                        SourceColumnName = name1,
                        Type = columnInfo.Navigat.GetNavigateType() == NavigateType.OneToOne ? "ONE_TO_ONE" : "ONE_TO_MANY",
                        TargetTableName = columnInfo.DbColumnName,
                        TargetColumnName = string.IsNullOrEmpty(name2) ? "Id" : name2
                    };
                    columnRelationList.Add(relation);
                }
            }
        }

        return new VisualDbTable { VisualTableList = visualTableList, VisualColumnList = visualColumnList, ColumnRelationList = columnRelationList };
    }

    /// <summary>
    /// 获取字段列表 🔖
    /// </summary>
    /// <param name="tableName">表名</param>
    /// <param name="configId">ConfigId</param>
    /// <returns></returns>
    [DisplayName("获取字段列表")]
    public List<DbColumnOutput> GetColumnList(string tableName, string configId = SqlSugarConst.MainConfigId)
    {
        var db = _db.AsTenant().GetConnectionScope(configId);
        if (string.IsNullOrWhiteSpace(tableName))
            return new List<DbColumnOutput>();

        return db.DbMaintenance.GetColumnInfosByTableName(tableName, false).Adapt<List<DbColumnOutput>>();
    }

    /// <summary>
    /// 获取数据库数据类型列表 🔖
    /// </summary>
    /// <param name="configId"></param>
    /// <returns></returns>
    [DisplayName("获取数据库数据类型列表")]
    public List<string> GetDbTypeList(string configId = SqlSugarConst.MainConfigId)
    {
        var db = _db.AsTenant().GetConnectionScope(configId);
        return db.DbMaintenance.GetDbTypes().OrderBy(u => u).ToList();
    }

    /// <summary>
    /// 增加列 🔖
    /// </summary>
    /// <param name="input"></param>
    [ApiDescriptionSettings(Name = "AddColumn"), HttpPost]
    [DisplayName("增加列")]
    public void AddColumn(DbColumnInput input)
    {
        var column = new DbColumnInfo
        {
            ColumnDescription = input.ColumnDescription,
            DbColumnName = input.DbColumnName,
            IsIdentity = input.IsIdentity == 1,
            IsNullable = input.IsNullable == 1,
            IsPrimarykey = input.IsPrimarykey == 1,
            Length = input.Length,
            DecimalDigits = input.DecimalDigits,
            DataType = input.DataType
        };
        var db = _db.AsTenant().GetConnectionScope(input.ConfigId);
        db.DbMaintenance.AddColumn(input.TableName, column);
        db.DbMaintenance.AddColumnRemark(input.DbColumnName, input.TableName, input.ColumnDescription);
        if (column.IsPrimarykey)
            db.DbMaintenance.AddPrimaryKey(input.TableName, input.DbColumnName);
    }

    /// <summary>
    /// 删除列 🔖
    /// </summary>
    /// <param name="input"></param>
    [ApiDescriptionSettings(Name = "DeleteColumn"), HttpPost]
    [DisplayName("删除列")]
    public void DeleteColumn(DeleteDbColumnInput input)
    {
        var db = _db.AsTenant().GetConnectionScope(input.ConfigId);
        db.DbMaintenance.DropColumn(input.TableName, input.DbColumnName);
    }

    /// <summary>
    /// 编辑列 🔖
    /// </summary>
    /// <param name="input"></param>
    [ApiDescriptionSettings(Name = "UpdateColumn"), HttpPost]
    [DisplayName("编辑列")]
    public void UpdateColumn(UpdateDbColumnInput input)
    {
        var db = _db.AsTenant().GetConnectionScope(input.ConfigId);
        db.DbMaintenance.RenameColumn(input.TableName, input.OldColumnName, input.ColumnName);
        if (db.DbMaintenance.IsAnyColumnRemark(input.ColumnName, input.TableName))
            db.DbMaintenance.DeleteColumnRemark(input.ColumnName, input.TableName);
        db.DbMaintenance.AddColumnRemark(input.ColumnName, input.TableName, string.IsNullOrWhiteSpace(input.Description) ? input.ColumnName : input.Description);
    }

    /// <summary>
    /// 获取表列表 🔖
    /// </summary>
    /// <param name="configId">ConfigId</param>
    /// <returns></returns>
    [DisplayName("获取表列表")]
    public List<DbTableInfo> GetTableList(string configId = SqlSugarConst.MainConfigId)
    {
        var db = _db.AsTenant().GetConnectionScope(configId);
        return db.DbMaintenance.GetTableInfoList(false);
    }

    /// <summary>
    /// 增加表 🔖
    /// </summary>
    /// <param name="input"></param>
    [ApiDescriptionSettings(Name = "AddTable"), HttpPost]
    [DisplayName("增加表")]
    public void AddTable(DbTableInput input)
    {
        if (input.DbColumnInfoList == null || !input.DbColumnInfoList.Any())
            throw Oops.Oh(ErrorCodeEnum.db1000);

        if (input.DbColumnInfoList.GroupBy(u => u.DbColumnName).Any(u => u.Count() > 1))
            throw Oops.Oh(ErrorCodeEnum.db1002);

        var config = App.GetOptions<DbConnectionOptions>().ConnectionConfigs.FirstOrDefault(u => u.ConfigId.ToString() == input.ConfigId);
        var db = _db.AsTenant().GetConnectionScope(input.ConfigId);
        var typeBuilder = db.DynamicBuilder().CreateClass(input.TableName, new SugarTable() { TableName = input.TableName, TableDescription = input.Description });
        input.DbColumnInfoList.ForEach(u =>
        {
            var dbColumnName = config.DbSettings.EnableUnderLine ? UtilMethods.ToUnderLine(u.DbColumnName.Trim()) : u.DbColumnName.Trim();
            // 虚拟类都默认string类型，具体以列数据类型为准
            typeBuilder.CreateProperty(dbColumnName, typeof(string), new SugarColumn()
            {
                IsPrimaryKey = u.IsPrimarykey == 1,
                IsIdentity = u.IsIdentity == 1,
                ColumnDataType = u.DataType,
                Length = u.Length,
                IsNullable = u.IsNullable == 1,
                DecimalDigits = u.DecimalDigits,
                ColumnDescription = u.ColumnDescription,
            });
        });
        db.CodeFirst.InitTables(typeBuilder.BuilderType());
    }

    /// <summary>
    /// 删除表 🔖
    /// </summary>
    /// <param name="input"></param>
    [ApiDescriptionSettings(Name = "DeleteTable"), HttpPost]
    [DisplayName("删除表")]
    public void DeleteTable(DeleteDbTableInput input)
    {
        var db = _db.AsTenant().GetConnectionScope(input.ConfigId);
        db.DbMaintenance.DropTable(input.TableName);
    }

    /// <summary>
    /// 编辑表 🔖
    /// </summary>
    /// <param name="input"></param>
    [ApiDescriptionSettings(Name = "UpdateTable"), HttpPost]
    [DisplayName("编辑表")]
    public void UpdateTable(UpdateDbTableInput input)
    {
        var db = _db.AsTenant().GetConnectionScope(input.ConfigId);
        db.DbMaintenance.RenameTable(input.OldTableName, input.TableName);
        try
        {
            if (db.DbMaintenance.IsAnyTableRemark(input.TableName))
                db.DbMaintenance.DeleteTableRemark(input.TableName);

            if (!string.IsNullOrWhiteSpace(input.Description))
                db.DbMaintenance.AddTableRemark(input.TableName, input.Description);
        }
        catch (NotSupportedException ex)
        {
            throw Oops.Oh(ex.ToString());
        }
    }

    /// <summary>
    /// 创建实体 🔖
    /// </summary>
    /// <param name="input"></param>
    [ApiDescriptionSettings(Name = "CreateEntity"), HttpPost]
    [DisplayName("创建实体")]
    public void CreateEntity(CreateEntityInput input)
    {
        var config = App.GetOptions<DbConnectionOptions>().ConnectionConfigs.FirstOrDefault(u => u.ConfigId.ToString() == input.ConfigId);
        input.Position = string.IsNullOrWhiteSpace(input.Position) ? "Admin.NET.Application" : input.Position;
        input.EntityName = string.IsNullOrWhiteSpace(input.EntityName) ? (config.DbSettings.EnableUnderLine ? CodeGenUtil.CamelColumnName(input.TableName, null) : input.TableName) : input.EntityName;
        string[] dbColumnNames = Array.Empty<string>();
        // Entity.cs.vm中是允许创建没有基类的实体的，所以这里也要做出相同的判断
        if (!string.IsNullOrWhiteSpace(input.BaseClassName))
        {
            _codeGenOptions.EntityBaseColumn.TryGetValue(input.BaseClassName, out dbColumnNames);
            if (dbColumnNames is null || dbColumnNames is { Length: 0 })
                throw Oops.Oh("基类配置文件不存在此类型");
        }
        var templatePath = GetEntityTemplatePath();
        var targetPath = GetEntityTargetPath(input);
        var db = _db.AsTenant().GetConnectionScope(input.ConfigId);
        DbTableInfo dbTableInfo = db.DbMaintenance.GetTableInfoList(false).FirstOrDefault(u => u.Name == input.TableName || u.Name == input.TableName.ToLower()) ?? throw Oops.Oh(ErrorCodeEnum.db1001);
        List<DbColumnInfo> dbColumnInfos = db.DbMaintenance.GetColumnInfosByTableName(input.TableName, false);
        dbColumnInfos.ForEach(u =>
        {
            u.PropertyName = config.DbSettings.EnableUnderLine ? CodeGenUtil.CamelColumnName(u.DbColumnName, dbColumnNames) : u.DbColumnName; // 转下划线后的列名需要再转回来
            u.DataType = CodeGenUtil.ConvertDataType(u, config.DbType);
        });
        if (_codeGenOptions.BaseEntityNames.Contains(input.BaseClassName, StringComparer.OrdinalIgnoreCase))
            dbColumnInfos = dbColumnInfos.Where(u => !dbColumnNames.Contains(u.PropertyName, StringComparer.OrdinalIgnoreCase)).ToList();

        var tContent = File.ReadAllText(templatePath);
        var tResult = _viewEngine.RunCompileFromCached(tContent, new
        {
            NameSpace = $"{input.Position}.Entity",
            input.TableName,
            input.EntityName,
            BaseClassName = string.IsNullOrWhiteSpace(input.BaseClassName) ? "" : $" : {input.BaseClassName}",
            input.ConfigId,
            dbTableInfo.Description,
            TableField = dbColumnInfos
        });
        File.WriteAllText(targetPath, tResult, Encoding.UTF8);
    }

    /// <summary>
    /// 创建种子数据 🔖
    /// </summary>
    /// <param name="input"></param>
    [ApiDescriptionSettings(Name = "CreateSeedData"), HttpPost]
    [DisplayName("创建种子数据")]
    public async Task CreateSeedData(CreateSeedDataInput input)
    {
        var config = App.GetOptions<DbConnectionOptions>().ConnectionConfigs.FirstOrDefault(u => u.ConfigId.ToString() == input.ConfigId);
        input.Position = string.IsNullOrWhiteSpace(input.Position) ? "Admin.NET.Core" : input.Position;

        var templatePath = GetSeedDataTemplatePath();
        var db = _db.AsTenant().GetConnectionScope(input.ConfigId);
        var tableInfo = db.DbMaintenance.GetTableInfoList(false).First(u => u.Name == input.TableName); // 表名
        List<DbColumnInfo> dbColumnInfos = db.DbMaintenance.GetColumnInfosByTableName(input.TableName, false); // 所有字段
        IEnumerable<EntityInfo> entityInfos = await GetEntityInfos();
        Type entityType = null;
        foreach (var item in entityInfos)
        {
            if (tableInfo.Name.ToLower() != (config.DbSettings.EnableUnderLine ? UtilMethods.ToUnderLine(item.DbTableName) : item.DbTableName).ToLower()) continue;
            entityType = item.Type;
            break;
        }
        if (entityType == null) throw Oops.Oh(ErrorCodeEnum.db1003);

        input.EntityName = entityType.Name;
        input.SeedDataName = entityType.Name + "SeedData";
        if (!string.IsNullOrWhiteSpace(input.Suffix))
            input.SeedDataName += input.Suffix;
        var targetPath = GetSeedDataTargetPath(input);

        // 查询所有数据
        var query = db.QueryableByObject(entityType);
        DbColumnInfo orderField = null; // 排序字段
        // 优先用创建时间排序
        orderField = dbColumnInfos.Where(u => u.DbColumnName.ToLower() == "create_time" || u.DbColumnName.ToLower() == "createtime").FirstOrDefault();
        if (orderField != null)
            query.OrderBy(orderField.DbColumnName);
        // 其次用Id排序
        orderField = dbColumnInfos.Where(u => u.DbColumnName.ToLower() == "id").FirstOrDefault();
        if (orderField != null)
            query.OrderBy(orderField.DbColumnName);
        IEnumerable recordsTmp = (IEnumerable)query.ToList();
        List<dynamic> records = recordsTmp.ToDynamicList();
        //这里要过滤已存在的数据
        if (input.FilterExistingData && records.Count() > 0)
        {
            //获取实体类型
            //获取所有种数据数据类型
            var entityTypes = App.EffectiveTypes.Where(u => !u.IsInterface && !u.IsAbstract && u.IsClass && u.IsDefined(typeof(SugarTable), false) && u.FullName.EndsWith("." + input.EntityName))
                .Where(u => !u.GetCustomAttributes<IgnoreTableAttribute>().Any())
                .ToList();
            if (entityTypes.Count == 1) //只有一个实体匹配才能过滤
            {
                //获取实体的主键对应的属性名称
                var pkInfo = entityTypes[0].GetProperties().Where(u => u.GetCustomAttribute<SugarColumn>() != null && u.GetCustomAttribute<SugarColumn>().IsPrimaryKey).First();
                if (pkInfo != null)
                {
                    var seedDataTypes = App.EffectiveTypes
                        .Where(u => !u.IsInterface && !u.IsAbstract && u.IsClass && u.GetInterfaces().Any(
                            i => i.HasImplementedRawGeneric(typeof(ISqlSugarEntitySeedData<>)) && i.GenericTypeArguments[0] == entityTypes[0]
                            )
                        )
                        .ToList();
                    //可能会重名的种子数据不作为过滤项
                    string doNotFilterfullName1 = $"{input.Position}.SeedData.{input.SeedDataName}";
                    string doNotFilterfullName2 = $"{input.Position}.{input.SeedDataName}"; //Core中的命名空间没有SeedData

                    PropertyInfo idPropertySeedData = records[0].GetType().GetProperty("Id");

                    for (int i = seedDataTypes.Count - 1; i >= 0; i--)
                    {
                        string fullName = seedDataTypes[i].FullName;
                        if ((fullName == doNotFilterfullName1) || (fullName == doNotFilterfullName2))
                            continue;
                        //开始删除重复数据
                        var instance = Activator.CreateInstance(seedDataTypes[i]);
                        var hasDataMethod = seedDataTypes[i].GetMethod("HasData");
                        var seedData = ((IEnumerable)hasDataMethod?.Invoke(instance, null))?.Cast<object>();
                        if (seedData == null) continue;

                        List<object> recordsToRemove = new List<object>();
                        foreach (var record in records)
                        {
                            object recordId = pkInfo.GetValue(record);
                            foreach (var d1 in seedData)
                            {
                                object dataId = idPropertySeedData.GetValue(d1);
                                if (recordId != null && dataId != null && recordId.Equals(dataId))
                                {
                                    recordsToRemove.Add(record);
                                    break;
                                }
                            }
                        }
                        foreach (var itemToRemove in recordsToRemove)
                        {
                            records.Remove(itemToRemove);
                        }
                    }
                }
            }
        }
        var timeConverter = new IsoDateTimeConverter { DateTimeFormat = "yyyy-MM-dd HH:mm:ss" };
        var recordsJSON = JsonConvert.SerializeObject(records, Formatting.Indented, timeConverter);

        // 检查有没有 System.Text.Json.Serialization.JsonIgnore 的属性
        var jsonIgnoreProperties = entityType.GetProperties().Where(p => (p.GetAttribute<System.Text.Json.Serialization.JsonIgnoreAttribute>() != null ||
            p.GetAttribute<JsonIgnoreAttribute>() != null) && p.GetAttribute<SugarColumn>() != null).ToList();
        var jsonIgnoreInfo = new List<List<JsonIgnoredPropertyData>>();
        if (jsonIgnoreProperties.Count > 0)
        {
            int recordIndex = 0;
            foreach (var r in (IEnumerable)records)
            {
                List<JsonIgnoredPropertyData> record = new();
                foreach (var item in jsonIgnoreProperties)
                {
                    object v = item.GetValue(r);
                    string strValue = "null";
                    if (v != null)
                    {
                        strValue = v.ToString();
                        if (v.GetType() == typeof(string))
                            strValue = "\"" + strValue + "\"";
                        else if (v.GetType() == typeof(DateTime))
                            strValue = "DateTime.Parse(\"" + ((DateTime)v).ToString("yyyy-MM-dd HH:mm:ss") + "\")";
                    }
                    record.Add(new JsonIgnoredPropertyData { RecordIndex = recordIndex, Name = item.Name, Value = strValue });
                }
                recordIndex++;
                jsonIgnoreInfo.Add(record);
            }
        }

        var tContent = File.ReadAllText(templatePath);
        var data = new
        {
            NameSpace = $"{input.Position}.SeedData",
            EntityNameSpace = entityType.Namespace,
            input.TableName,
            input.EntityName,
            input.SeedDataName,
            input.ConfigId,
            tableInfo.Description,
            JsonIgnoreInfo = jsonIgnoreInfo,
            RecordsJSON = recordsJSON
        };
        var tResult = _viewEngine.RunCompile(tContent, data, builderAction: builder =>
        {
            builder.AddAssemblyReferenceByName("System.Linq");
            builder.AddAssemblyReferenceByName("System.Collections");
            builder.AddUsing("System.Collections.Generic");
            builder.AddUsing("System.Linq");
        });
        File.WriteAllText(targetPath, tResult, Encoding.UTF8);
    }

    /// <summary>
    /// 获取库表信息
    /// </summary>
    /// <returns></returns>
    private async Task<IEnumerable<EntityInfo>> GetEntityInfos()
    {
        var entityInfos = new List<EntityInfo>();

        var type = typeof(SugarTable);
        var types = new List<Type>();
        if (_codeGenOptions.EntityAssemblyNames != null)
        {
            foreach (var assemblyName in _codeGenOptions.EntityAssemblyNames)
            {
                Assembly asm = Assembly.Load(assemblyName);
                types.AddRange(asm.GetExportedTypes().ToList());
            }
        }
        bool IsMyAttribute(Attribute[] o)
        {
            foreach (Attribute a in o)
            {
                if (a.GetType() == type)
                    return true;
            }
            return false;
        }
        Type[] cosType = types.Where(o =>
        {
            return IsMyAttribute(Attribute.GetCustomAttributes(o, true));
        }
        ).ToArray();

        foreach (var c in cosType)
        {
            var sugarAttribute = c.GetCustomAttributes(type, true)?.FirstOrDefault();

            var des = c.GetCustomAttributes(typeof(DescriptionAttribute), true);
            var description = "";
            if (des.Length > 0)
            {
                description = ((DescriptionAttribute)des[0]).Description;
            }
            entityInfos.Add(new EntityInfo()
            {
                EntityName = c.Name,
                DbTableName = sugarAttribute == null ? c.Name : ((SugarTable)sugarAttribute).TableName,
                TableDescription = description,
                Type = c
            });
        }
        return await Task.FromResult(entityInfos);
    }

    /// <summary>
    /// 获取实体模板文件路径
    /// </summary>
    /// <returns></returns>
    private static string GetEntityTemplatePath()
    {
        var templatePath = Path.Combine(App.WebHostEnvironment.WebRootPath, "template");
        return Path.Combine(templatePath, "Entity.cs.vm");
    }

    /// <summary>
    /// 获取种子数据模板文件路径
    /// </summary>
    /// <returns></returns>
    private static string GetSeedDataTemplatePath()
    {
        var templatePath = Path.Combine(App.WebHostEnvironment.WebRootPath, "template");
        return Path.Combine(templatePath, "SeedData.cs.vm");
    }

    /// <summary>
    /// 设置生成实体文件路径
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    private static string GetEntityTargetPath(CreateEntityInput input)
    {
        var backendPath = Path.Combine(new DirectoryInfo(App.WebHostEnvironment.ContentRootPath).Parent.FullName, input.Position, "Entity");
        if (!Directory.Exists(backendPath))
            Directory.CreateDirectory(backendPath);
        return Path.Combine(backendPath, input.EntityName + ".cs");
    }

    /// <summary>
    /// 设置生成种子数据文件路径
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    private static string GetSeedDataTargetPath(CreateSeedDataInput input)
    {
        var backendPath = Path.Combine(new DirectoryInfo(App.WebHostEnvironment.ContentRootPath).Parent.FullName, input.Position, "SeedData");
        if (!Directory.Exists(backendPath))
            Directory.CreateDirectory(backendPath);
        return Path.Combine(backendPath, input.SeedDataName + ".cs");
    }

    /// <summary>
    /// 备份数据库（PostgreSQL）🔖
    /// </summary>
    /// <returns></returns>
    [HttpPost, NonUnify]
    [DisplayName("备份数据库（PostgreSQL）")]
    public async Task<IActionResult> BackupDatabase()
    {
        if (_db.CurrentConnectionConfig.DbType != SqlSugar.DbType.PostgreSQL)
            throw Oops.Oh("只支持 PostgreSQL 数据库 😁");

        var npgsqlConn = new NpgsqlConnectionStringBuilder(_db.CurrentConnectionConfig.ConnectionString);
        if (npgsqlConn == null || string.IsNullOrWhiteSpace(npgsqlConn.Host) || string.IsNullOrWhiteSpace(npgsqlConn.Username) || string.IsNullOrWhiteSpace(npgsqlConn.Password) || string.IsNullOrWhiteSpace(npgsqlConn.Database))
            throw Oops.Oh("PostgreSQL 数据库配置错误");

        // 确保备份目录存在
        var backupDirectory = Path.Combine(Directory.GetCurrentDirectory(), "backups");
        Directory.CreateDirectory(backupDirectory);

        // 构建备份文件名
        string backupFileName = $"backup_{DateTime.Now:yyyyMMddHHmmss}.sql";
        string backupFilePath = Path.Combine(backupDirectory, backupFileName);

        // 启动pg_dump进程进行备份
        // 设置密码：export PGPASSWORD='xxxxxx'
        var bash = $"-U {npgsqlConn.Username} -h {npgsqlConn.Host} -p {npgsqlConn.Port} -E UTF8 -F c -b -v -f {backupFilePath} {npgsqlConn.Database}";
        var startInfo = new ProcessStartInfo
        {
            FileName = "pg_dump",
            Arguments = bash,
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            CreateNoWindow = true,
            EnvironmentVariables =
            {
                ["PGPASSWORD"] = npgsqlConn.Password
            }
        };

        //_logger.LogInformation("备份数据库：pg_dump " + bash);

        //try
        //{
        using (var backupProcess = Process.Start(startInfo))
        {
            await backupProcess.WaitForExitAsync();

            //var output = await backupProcess.StandardOutput.ReadToEndAsync();
            //var error = await backupProcess.StandardError.ReadToEndAsync();

            // 检查备份是否成功
            if (backupProcess.ExitCode != 0)
            {
                throw Oops.Oh($"备份失败：ExitCode({backupProcess.ExitCode})");
            }
        }

        //    _logger.LogInformation($"备份成功：{backupFilePath}");
        //}
        //catch (Exception ex)
        //{
        //    _logger.LogError(ex, $"备份失败：");
        //    throw;
        //}

        // 若备份成功则提供下载链接
        return new FileStreamResult(new FileStream(backupFilePath, FileMode.Open), "application/octet-stream")
        {
            FileDownloadName = backupFileName
        };
    }
}