// Admin.NET 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。使用本项目应遵守相关法律法规和许可证的要求。
//
// 本项目主要遵循 MIT 许可证和 Apache 许可证（版本 2.0）进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 和 LICENSE-APACHE 文件。
//
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目二次开发而产生的一切法律纠纷和责任，我们不承担任何责任！

using Microsoft.Data.Sqlite;
using DbType = SqlSugar.DbType;

namespace Admin.NET.Core;

public static class SqlSugarSetup
{
    // 多租户实例
    public static ITenant ITenant { get; set; }

    // 是否正在处理种子数据
    private static bool _isHandlingSeedData = false;

    /// <summary>
    /// SqlSugar 上下文初始化
    /// </summary>
    /// <param name="services"></param>
    public static void AddSqlSugar(this IServiceCollection services)
    {
        // 注册雪花Id
        var snowIdOpt = App.GetConfig<SnowIdOptions>("SnowId", true);
        YitIdHelper.SetIdGenerator(snowIdOpt);

        // 自定义 SqlSugar 雪花ID算法
        SnowFlakeSingle.WorkId = snowIdOpt.WorkerId;
        StaticConfig.CustomSnowFlakeFunc = YitIdHelper.NextId;
        // 注册 MongoDb
        InstanceFactory.CustomAssemblies = [typeof(SqlSugar.MongoDb.MongoDbProvider).Assembly];
        // 动态表达式 SqlFunc 支持，https://www.donet5.com/Home/Doc?typeId=2569
        StaticConfig.DynamicExpressionParserType = typeof(DynamicExpressionParser);
        StaticConfig.DynamicExpressionParsingConfig = new ParsingConfig
        {
            CustomTypeProvider = new SqlSugarTypeProvider()
        };

        var dbOptions = App.GetConfig<DbConnectionOptions>("DbConnection", true);
        dbOptions.ConnectionConfigs.ForEach(SetDbConfig);

        SqlSugarScope sqlSugar = new(dbOptions.ConnectionConfigs.Adapt<List<ConnectionConfig>>(), db =>
        {
            dbOptions.ConnectionConfigs.ForEach(config =>
            {
                var dbProvider = db.GetConnectionScope(config.ConfigId);
                SetDbAop(dbProvider, dbOptions.EnableConsoleSql, dbOptions.SuperAdminIgnoreIDeletedFilter);
                SetDbDiffLog(dbProvider, config);
            });
        });
        ITenant = sqlSugar;

        services.AddSingleton<ISqlSugarClient>(sqlSugar); // 单例注册
        services.AddScoped(typeof(SqlSugarRepository<>)); // 仓储注册
        services.AddUnitOfWork<SqlSugarUnitOfWork>(); // 事务与工作单元注册

        // 初始化数据库表结构及种子数据
        dbOptions.ConnectionConfigs.ForEach(config =>
        {
            InitDatabase(sqlSugar, config);
        });
    }

    /// <summary>
    /// 配置连接属性
    /// </summary>
    /// <param name="config"></param>
    public static void SetDbConfig(DbConnectionConfig config)
    {
        if (config.DbSettings.EnableConnStringEncrypt)
            config.ConnectionString = CryptogramUtil.Decrypt(config.ConnectionString);

        var configureExternalServices = new ConfigureExternalServices
        {
            EntityNameService = (type, entity) => // 处理表
            {
                entity.IsDisabledDelete = true; // 禁止删除非 sqlsugar 创建的列
                // 只处理贴了特性[SugarTable]表
                if (!type.GetCustomAttributes<SugarTable>().Any())
                    return;
                if (config.DbSettings.EnableUnderLine && !entity.DbTableName.Contains('_'))
                    entity.DbTableName = entity.DbTableName.ToUnderLine(); // 驼峰转下划线
            },
            EntityService = (type, column) => // 处理列
            {
                // 只处理贴了特性[SugarColumn]列
                if (!type.GetCustomAttributes<SugarColumn>().Any())
                    return;
                if (new NullabilityInfoContext().Create(type).WriteState is NullabilityState.Nullable)
                    column.IsNullable = true;
                if (config.DbSettings.EnableUnderLine && !column.IsIgnore && !column.DbColumnName.Contains('_'))
                    column.DbColumnName = column.DbColumnName.ToUnderLine(); // 驼峰转下划线
            },
            DataInfoCacheService = new SqlSugarCache(),
        };
        config.ConfigureExternalServices = configureExternalServices;
        config.InitKeyType = InitKeyType.Attribute;
        config.IsAutoCloseConnection = true;
        config.MoreSettings = new ConnMoreSettings
        {
            IsAutoRemoveDataCache = true, // 启用自动删除缓存，所有增删改会自动调用.RemoveDataCache()
            IsAutoDeleteQueryFilter = true, // 启用删除查询过滤器
            IsAutoUpdateQueryFilter = true, // 启用更新查询过滤器
            SqlServerCodeFirstNvarchar = true // 采用Nvarchar
        };

        // 若库类型是人大金仓则默认设置PG模式
        if (config.DbType == DbType.Kdbndp)
            config.MoreSettings.DatabaseModel = DbType.PostgreSQL; // 配置PG模式主要是兼容系统表差异

        // 若库类型是Oracle则默认主键名字和参数名字最大长度
        if (config.DbType == DbType.Oracle)
            config.MoreSettings.MaxParameterNameLength = 30;
    }

    /// <summary>
    /// 配置Aop
    /// </summary>
    /// <param name="db"></param>
    /// <param name="enableConsoleSql"></param>
    /// <param name="superAdminIgnoreIDeletedFilter"></param>
    public static void SetDbAop(SqlSugarScopeProvider db, bool enableConsoleSql, bool superAdminIgnoreIDeletedFilter)
    {
        // 设置超时时间
        db.Ado.CommandTimeOut = 30;

        // 打印SQL语句
        if (enableConsoleSql)
        {
            db.Aop.OnLogExecuting = (sql, pars) =>
            {
                //// 若参数值超过100个字符则进行截取
                //foreach (var par in pars)
                //{
                //    if (par.DbType != System.Data.DbType.String || par.Value == null) continue;
                //    if (par.Value.ToString().Length > 100)
                //        par.Value = string.Concat(par.Value.ToString()[..100], "......");
                //}

                var log = $"【{DateTime.Now}——执行SQL】\r\n{UtilMethods.GetNativeSql(sql, pars)}\r\n";
                var originColor = Console.ForegroundColor;
                if (sql.StartsWith("SELECT", StringComparison.OrdinalIgnoreCase))
                    Console.ForegroundColor = ConsoleColor.Green;
                if (sql.StartsWith("UPDATE", StringComparison.OrdinalIgnoreCase) || sql.StartsWith("INSERT", StringComparison.OrdinalIgnoreCase))
                    Console.ForegroundColor = ConsoleColor.Yellow;
                if (sql.StartsWith("DELETE", StringComparison.OrdinalIgnoreCase))
                    Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(log);
                Console.ForegroundColor = originColor;
            };
        }
        db.Aop.OnError = ex =>
        {
            if (ex.Parametres == null) return;
            var log = $"【{DateTime.Now}——错误SQL】\r\n{UtilMethods.GetNativeSql(ex.Sql, (SugarParameter[])ex.Parametres)}\r\n";
            Log.Error(log, ex);
        };
        db.Aop.OnLogExecuted = (sql, pars) =>
        {
            //// 若参数值超过100个字符则进行截取
            //foreach (var par in pars)
            //{
            //    if (par.DbType != System.Data.DbType.String || par.Value == null) continue;
            //    if (par.Value.ToString().Length > 100)
            //        par.Value = string.Concat(par.Value.ToString()[..100], "......");
            //}

            // 执行时间超过5秒时
            if (!(db.Ado.SqlExecutionTime.TotalSeconds > 5)) return;

            var fileName = db.Ado.SqlStackTrace.FirstFileName; // 文件名
            var fileLine = db.Ado.SqlStackTrace.FirstLine; // 行号
            var firstMethodName = db.Ado.SqlStackTrace.FirstMethodName; // 方法名
            var log = $"【{DateTime.Now}——超时SQL】\r\n【所在文件名】：{fileName}\r\n【代码行数】：{fileLine}\r\n【方法名】：{firstMethodName}\r\n" + $"【SQL语句】：{UtilMethods.GetNativeSql(sql, pars)}";
            Log.Warning(log);
        };

        // 数据审计
        db.Aop.DataExecuting = (_, entityInfo) =>
        {
            // 若正在处理种子数据则直接返回
            if (_isHandlingSeedData) return;

            // 新增/插入
            if (entityInfo.OperationType == DataFilterType.InsertByObject)
            {
                // 若主键是长整型且空则赋值雪花Id
                if (entityInfo.EntityColumnInfo.IsPrimarykey && !entityInfo.EntityColumnInfo.IsIdentity && entityInfo.EntityColumnInfo.PropertyInfo.PropertyType == typeof(long))
                {
                    var id = entityInfo.EntityColumnInfo.PropertyInfo.GetValue(entityInfo.EntityValue);
                    if (id == null || (long)id == 0)
                        entityInfo.SetValue(YitIdHelper.NextId());
                }
                // 若创建时间为空则赋值当前时间
                else if (entityInfo.PropertyName == nameof(EntityBase.CreateTime))
                {
                    var createTime = entityInfo.EntityColumnInfo.PropertyInfo.GetValue(entityInfo.EntityValue)!;
                    if (createTime == null || createTime.Equals(DateTime.MinValue))
                        entityInfo.SetValue(DateTime.Now);
                }
                // 若当前用户为空（非web线程时）
                if (App.User == null) return;

                dynamic entityValue = entityInfo.EntityValue;
                if (entityInfo.PropertyName == nameof(EntityBaseTenantId.TenantId))
                {
                    var tenantId = entityValue.TenantId;
                    if (tenantId == null || tenantId == 0)
                        entityInfo.SetValue(App.User.FindFirst(ClaimConst.TenantId)?.Value);
                }
                else if (entityInfo.PropertyName == nameof(EntityBase.CreateUserId))
                {
                    var createUserId = entityValue.CreateUserId;
                    if (createUserId == 0 || createUserId == null)
                        entityInfo.SetValue(App.User.FindFirst(ClaimConst.UserId)?.Value);
                }
                else if (entityInfo.PropertyName == nameof(EntityBase.CreateUserName))
                {
                    var createUserName = entityValue.CreateUserName;
                    if (string.IsNullOrEmpty(createUserName))
                        entityInfo.SetValue(App.User.FindFirst(ClaimConst.RealName)?.Value);
                }
                else if (entityInfo.PropertyName == "CreateOrgId")
                {
                    var createOrgId = entityValue.CreateOrgId;
                    if (createOrgId == 0 || createOrgId == null)
                        entityInfo.SetValue(App.User.FindFirst(ClaimConst.OrgId)?.Value);
                }
                else if (entityInfo.PropertyName == "CreateOrgName")
                {
                    var createOrgName = entityValue.CreateOrgName;
                    if (string.IsNullOrEmpty(createOrgName))
                        entityInfo.SetValue(App.User.FindFirst(ClaimConst.OrgName)?.Value);
                }
            }
            // 编辑/更新
            else if (entityInfo.OperationType == DataFilterType.UpdateByObject)
            {
                if (entityInfo.PropertyName == nameof(EntityBase.UpdateTime))
                    entityInfo.SetValue(DateTime.Now);
                else if (entityInfo.PropertyName == nameof(EntityBaseDel.DeleteTime))
                {
                    dynamic entityValue = entityInfo.EntityValue;
                    var isDelete = entityValue.IsDelete;
                    if (isDelete == true)
                    {
                        entityInfo.SetValue(DateTime.Now);
                    }
                }
                // 若当前用户为空（非web线程时）
                if (App.User == null) return;

                if (entityInfo.PropertyName == nameof(EntityBase.UpdateUserId))
                    entityInfo.SetValue(App.User?.FindFirst(ClaimConst.UserId)?.Value);
                else if (entityInfo.PropertyName == nameof(EntityBase.UpdateUserName))
                    entityInfo.SetValue(App.User?.FindFirst(ClaimConst.RealName)?.Value);
            }
        };

        // 是否为超级管理员
        var isSuperAdmin = App.User?.FindFirst(ClaimConst.AccountType)?.Value == ((int)AccountTypeEnum.SuperAdmin).ToString();

        // 配置假删除过滤器，如果当前用户是超级管理员并且允许忽略软删除过滤器则不会应用
        if (!isSuperAdmin || !superAdminIgnoreIDeletedFilter)
            db.QueryFilter.AddTableFilter<IDeletedFilter>(u => u.IsDelete == false);

        // 超管排除其他过滤器
        if (isSuperAdmin) return;

        // 配置租户过滤器
        var tenantId = App.User?.FindFirst(ClaimConst.TenantId)?.Value;
        if (!string.IsNullOrWhiteSpace(tenantId))
            db.QueryFilter.AddTableFilter<ITenantIdFilter>(u => u.TenantId == long.Parse(tenantId));

        // 配置用户机构（数据范围）过滤器
        SqlSugarFilter.SetOrgEntityFilter(db);

        // 配置自定义过滤器
        SqlSugarFilter.SetCustomEntityFilter(db);
    }

    /// <summary>
    /// 开启库表差异化日志
    /// </summary>
    /// <param name="db"></param>
    /// <param name="config"></param>
    private static void SetDbDiffLog(SqlSugarScopeProvider db, DbConnectionConfig config)
    {
        if (!config.DbSettings.EnableDiffLog) return;

        async void AopOnDiffLogEvent(DiffLogModel u)
        {
            // 记录差异数据
            var diffData = new List<dynamic>();
            for (int i = 0; i < u.AfterData.Count; i++)
            {
                var diffColumns = new List<dynamic>();
                var afterColumns = u.AfterData[i].Columns;
                var beforeColumns = u.BeforeData[i].Columns;
                for (int j = 0; j < afterColumns.Count; j++)
                {
                    if (afterColumns[j].Value.Equals(beforeColumns[j].Value)) continue;
                    diffColumns.Add(new
                    {
                        afterColumns[j].IsPrimaryKey,
                        afterColumns[j].ColumnName,
                        afterColumns[j].ColumnDescription,
                        BeforeValue = beforeColumns[j].Value,
                        AfterValue = afterColumns[j].Value,
                    });
                }

                diffData.Add(new { u.AfterData[i].TableName, u.AfterData[i].TableDescription, Columns = diffColumns });
            }

            var logDiff = new SysLogDiff
            {
                // 差异数据（字段描述、列名、值、表名、表描述）
                DiffData = JSON.Serialize(diffData),
                // 传进来的对象（如果对象为空，则使用首个数据的表名作为业务对象）
                BusinessData = u.BusinessData == null ? u.AfterData.FirstOrDefault()?.TableName : JSON.Serialize(u.BusinessData),
                // 枚举（insert、update、delete）
                DiffType = u.DiffType.ToString(),
                Sql = u.Sql,
                Parameters = JSON.Serialize(u.Parameters.Select(e => new { e.ParameterName, e.Value, TypeName = e.DbType.ToString() })),
                Elapsed = u.Time == null ? 0 : (long)u.Time.Value.TotalMilliseconds
            };
            var logDb = ITenant.IsAnyConnection(SqlSugarConst.LogConfigId) ? ITenant.GetConnectionScope(SqlSugarConst.LogConfigId) : ITenant.GetConnectionScope(SqlSugarConst.MainConfigId);
            await logDb.CopyNew().Insertable(logDiff).ExecuteCommandAsync();
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(DateTime.Now + $"\r\n*****开始差异日志*****\r\n{Environment.NewLine}{JSON.Serialize(logDiff)}{Environment.NewLine}*****结束差异日志*****\r\n");
        }

        db.Aop.OnDiffLogEvent = AopOnDiffLogEvent;
    }

    /// <summary>
    /// 初始化视图
    /// </summary>
    /// <param name="dbProvider"></param>
    private static void InitView(SqlSugarScopeProvider dbProvider)
    {
        var totalWatch = Stopwatch.StartNew(); // 开始总计时
        Log.Information($"初始化视图 {dbProvider.CurrentConnectionConfig.DbType} - {dbProvider.CurrentConnectionConfig.ConfigId}");
        var viewTypeList = App.EffectiveTypes.Where(u => !u.IsInterface && !u.IsAbstract && u.IsClass && u.GetInterfaces().Any(i => i.HasImplementedRawGeneric(typeof(ISqlSugarView)))).ToList();

        int taskIndex = 0, size = viewTypeList.Count;
        var taskList = viewTypeList.Select(viewType => Task.Run(() =>
        {
            // 开始计时
            var stopWatch = Stopwatch.StartNew();

            // 获取视图实体和配置信息
            var entityInfo = dbProvider.EntityMaintenance.GetEntityInfo(viewType) ?? throw new Exception("获取视图实体配置有误");

            // 如果视图存在，则删除视图
            if (dbProvider.DbMaintenance.GetViewInfoList(false).Any(it => it.Name.EqualIgnoreCase(entityInfo.DbTableName)))
                dbProvider.DbMaintenance.DropView(entityInfo.DbTableName);

            // 获取初始化视图查询SQL
            var sql = viewType.GetMethod(nameof(ISqlSugarView.GetQueryableSqlString))?.Invoke(Activator.CreateInstance(viewType), [dbProvider]) as string;
            if (string.IsNullOrWhiteSpace(sql)) throw new Exception("视图初始化Sql语句不能为空");

            // 创建视图
            dbProvider.Ado.ExecuteCommand($"CREATE VIEW {entityInfo.DbTableName} AS " + Environment.NewLine + " " + sql);

            // 停止计时
            stopWatch.Stop();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"初始化视图 {viewType.FullName,-58} ({dbProvider.CurrentConnectionConfig.ConfigId} - {Interlocked.Increment(ref taskIndex):D003}/{size:D003}，耗时：{stopWatch.ElapsedMilliseconds:N0} ms)");
        }));
        Task.WaitAll(taskList.ToArray());

        totalWatch.Stop(); // 停止总计时
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"初始化视图 {dbProvider.CurrentConnectionConfig.DbType} - {dbProvider.CurrentConnectionConfig.ConfigId} 总耗时：{totalWatch.ElapsedMilliseconds:N0} ms");
    }

    /// <summary>
    /// 等待数据库就绪
    /// </summary>
    /// <param name="dbProvider"></param>
    private static void WaitForDatabaseReady(SqlSugarScopeProvider dbProvider)
    {
        do
        {
            try
            {
                if (dbProvider.Ado.Connection.State != ConnectionState.Open)
                    dbProvider.Ado.Connection.Open();

                // 如果连接成功，直接返回
                Log.Information("数据库连接成功。");
                return;
            }
            catch (Exception ex)
            {
                Log.Warning($"数据库尚未就绪，等待中... 错误：{ex.Message}");
                Thread.Sleep(1000);
            }
        } while (true);
    }

    /// <summary>
    /// 初始化数据库
    /// </summary>
    /// <param name="db">SqlSugarScope 实例</param>
    /// <param name="config">数据库连接配置</param>
    private static void InitDatabase(SqlSugarScope db, DbConnectionConfig config)
    {
        var dbProvider = db.GetConnectionScope(config.ConfigId);

        // 初始化数据库  如果是没有数据库的话，是先初始化数据库再做连接
        if (config.DbSettings.EnableInitDb)
        {
            Log.Information($"初始化数据库 {config.DbType} - {config.ConfigId} - {config.ConnectionString}");
            if (config.DbType != DbType.Oracle) dbProvider.DbMaintenance.CreateDatabase();
        }

        // 等待数据库连接就绪
        WaitForDatabaseReady(dbProvider);

        // 初始化表结构
        if (config.TableSettings.EnableInitTable)
        {
            Log.Information($"初始化表结构 {config.DbType} - {config.ConfigId}");
            var entityTypes = GetEntityTypesForInit(config);
            InitializeTables(dbProvider, entityTypes, config);
        }

        // 初始化视图
        if (config.DbSettings.EnableInitView) InitView(dbProvider);

        // 初始化种子数据
        if (config.SeedSettings.EnableInitSeed) InitSeedData(db, config);
    }

    /// <summary>
    /// 获取需要初始化的实体类型
    /// </summary>
    /// <param name="config">数据库连接配置</param>
    /// <returns>实体类型列表</returns>
    private static List<Type> GetEntityTypesForInit(DbConnectionConfig config)
    {
        return App.EffectiveTypes
            .Where(u => !u.IsInterface && !u.IsAbstract && u.IsClass && u.IsDefined(typeof(SugarTable), false))
            .Where(u => !u.GetCustomAttributes<IgnoreTableAttribute>().Any())
            .WhereIF(config.TableSettings.EnableIncreTable, u => u.IsDefined(typeof(IncreTableAttribute), false))
            .Where(u => IsEntityForConfig(u, config))
            .ToList();
    }

    /// <summary>
    /// 判断实体是否属于当前配置
    /// </summary>
    /// <param name="entityType">实体类型</param>
    /// <param name="config">数据库连接配置</param>
    /// <returns>是否属于当前配置</returns>
    private static bool IsEntityForConfig(Type entityType, DbConnectionConfig config)
    {
        switch (config.ConfigId.ToString())
        {
            case SqlSugarConst.MainConfigId:
                return entityType.GetCustomAttributes<SysTableAttribute>().Any() ||
                       (!entityType.GetCustomAttributes<LogTableAttribute>().Any() &&
                        !entityType.GetCustomAttributes<TenantAttribute>().Any(o => o.configId.ToString() != config.ConfigId.ToString()));

            case SqlSugarConst.LogConfigId:
                return entityType.GetCustomAttributes<LogTableAttribute>().Any();

            default:
                {
                    var tenantAttribute = entityType.GetCustomAttribute<TenantAttribute>();
                    return tenantAttribute != null && tenantAttribute.configId.ToString() == config.ConfigId.ToString();
                }
        }
    }

    /// <summary>
    /// 初始化表结构
    /// </summary>
    /// <param name="dbProvider">SqlSugarScopeProvider 实例</param>
    /// <param name="entityTypes">实体类型列表</param>
    /// <param name="config">数据库连接配置</param>
    private static void InitializeTables(SqlSugarScopeProvider dbProvider, List<Type> entityTypes, DbConnectionConfig config)
    {
        // 删除视图再初始化表结构，防止因为视图导致无法同步表结构
        var viewTypeList = App.EffectiveTypes.Where(u => !u.IsInterface && !u.IsAbstract && u.IsClass && u.GetInterfaces().Any(i => i.HasImplementedRawGeneric(typeof(ISqlSugarView)))).ToList();
        foreach (var viewType in viewTypeList)
        {
            var entityInfo = dbProvider.EntityMaintenance.GetEntityInfo(viewType) ?? throw new Exception("获取视图实体配置有误");
            if (dbProvider.DbMaintenance.GetViewInfoList(false).Any(it => it.Name.EqualIgnoreCase(entityInfo.DbTableName)))
                dbProvider.DbMaintenance.DropView(entityInfo.DbTableName);
        }

        int count = 0, sum = entityTypes.Count;
        var tasks = entityTypes.Select(entityType => Task.Run(() =>
        {
            Console.WriteLine($"初始化表结构 {entityType.FullName,-64} ({config.ConfigId} - {Interlocked.Increment(ref count):D003}/{sum:D003})");
            UpdateNullableColumns(dbProvider, entityType);
            InitializeTable(dbProvider, entityType);
        }));

        Task.WhenAll(tasks).GetAwaiter().GetResult();
    }

    /// <summary>
    /// 更新表中不存在于实体的字段为可空
    /// </summary>
    /// <param name="dbProvider">SqlSugarScopeProvider 实例</param>
    /// <param name="entityType">实体类型</param>
    private static void UpdateNullableColumns(SqlSugarScopeProvider dbProvider, Type entityType)
    {
        var entityInfo = dbProvider.EntityMaintenance.GetEntityInfo(entityType);
        var dbColumns = dbProvider.DbMaintenance.GetColumnInfosByTableName(entityInfo.DbTableName) ?? new List<DbColumnInfo>();

        foreach (var dbColumn in dbColumns.Where(c => !c.IsPrimarykey && entityInfo.Columns.All(u => u.DbColumnName != c.DbColumnName)))
        {
            dbColumn.IsNullable = true;
            Retry(() =>
            {
                dbProvider.DbMaintenance.UpdateColumn(entityInfo.DbTableName, dbColumn);
            }, maxRetry: 3, retryIntervalMs: 1000);
        }
    }

    /// <summary>
    /// 初始化表
    /// </summary>
    /// <param name="dbProvider">SqlSugarScopeProvider 实例</param>
    /// <param name="entityType">实体类型</param>
    private static void InitializeTable(SqlSugarScopeProvider dbProvider, Type entityType)
    {
        Retry(() =>
        {
            if (entityType.GetCustomAttribute<SplitTableAttribute>() == null)
            {
                dbProvider.CodeFirst.InitTables(entityType);
            }
            else
            {
                dbProvider.CodeFirst.SplitTables().InitTables(entityType);
            }
        }, maxRetry: 3, retryIntervalMs: 1000);
    }

    /// <summary>
    /// 初始化种子数据
    /// </summary>
    /// <param name="db">SqlSugarScope 实例</param>
    /// <param name="config">数据库连接配置</param>
    private static void InitSeedData(SqlSugarScope db, DbConnectionConfig config)
    {
        var dbProvider = db.GetConnectionScope(config.ConfigId);
        _isHandlingSeedData = true;

        Log.Information($"初始化种子数据 {config.DbType} - {config.ConfigId}");
        var seedDataTypes = GetSeedDataTypes(config);

        int count = 0, sum = seedDataTypes.Count;
        var tasks = seedDataTypes.Select(seedType => Task.Run(() =>
        {
            var entityType = seedType.GetInterfaces().First().GetGenericArguments().First();
            if (!IsEntityForConfig(entityType, config)) return;

            var seedData = GetSeedData(seedType)?.ToList();
            if (seedData == null) return;

            AdjustSeedDataIds(seedData, config);
            InsertOrUpdateSeedData(dbProvider, seedType, entityType, seedData, config, ref count, sum);
        }));

        Task.WhenAll(tasks).GetAwaiter().GetResult();
        _isHandlingSeedData = false;
    }

    /// <summary>
    /// 获取种子数据类型
    /// </summary>
    /// <param name="config">数据库连接配置</param>
    /// <returns>种子数据类型列表</returns>
    private static List<Type> GetSeedDataTypes(DbConnectionConfig config)
    {
        return App.EffectiveTypes
            .Where(u => !u.IsInterface && !u.IsAbstract && u.IsClass && u.GetInterfaces().Any(i => i.HasImplementedRawGeneric(typeof(ISqlSugarEntitySeedData<>))))
            .WhereIF(config.SeedSettings.EnableIncreSeed, u => u.IsDefined(typeof(IncreSeedAttribute), false))
            .OrderBy(u => u.GetCustomAttributes(typeof(SeedDataAttribute), false).Length > 0 ? ((SeedDataAttribute)u.GetCustomAttributes(typeof(SeedDataAttribute), false)[0]).Order : 0)
            .ToList();
    }

    /// <summary>
    /// 获取种子数据
    /// </summary>
    /// <param name="seedType">种子数据类型</param>
    /// <returns>种子数据列表</returns>
    private static IEnumerable<object> GetSeedData(Type seedType)
    {
        var instance = Activator.CreateInstance(seedType);
        var hasDataMethod = seedType.GetMethod("HasData");
        return ((IEnumerable)hasDataMethod?.Invoke(instance, null))?.Cast<object>();
    }

    /// <summary>
    /// 调整种子数据的 ID
    /// </summary>
    /// <param name="seedData">种子数据列表</param>
    /// <param name="config">数据库连接配置</param>
    private static void AdjustSeedDataIds(IEnumerable<object> seedData, DbConnectionConfig config)
    {
        var seedId = config.ConfigId.ToLong();
        foreach (var data in seedData)
        {
            var idProperty = data.GetType().GetProperty(nameof(EntityBaseId.Id));
            if (idProperty == null || idProperty.PropertyType != typeof(Int64)) continue;

            var idValue = idProperty.GetValue(data);
            if (idValue == null || idValue.ToString() == "0" || string.IsNullOrWhiteSpace(idValue.ToString()))
            {
                idProperty.SetValue(data, ++seedId);
            }
        }
    }

    /// <summary>
    /// 插入或更新种子数据
    /// </summary>
    /// <param name="dbProvider">SqlSugarScopeProvider 实例</param>
    /// <param name="seedType">种子数据类型</param>
    /// <param name="entityType">实体类型</param>
    /// <param name="seedData">种子数据列表</param>
    /// <param name="config">数据库连接配置</param>
    /// <param name="count">当前处理的数量</param>
    /// <param name="sum">总数量</param>
    private static void InsertOrUpdateSeedData(SqlSugarScopeProvider dbProvider, Type seedType, Type entityType, IEnumerable<object> seedData, DbConnectionConfig config, ref int count, int sum)
    {
        var entityInfo = dbProvider.EntityMaintenance.GetEntityInfo(entityType);
        var dataList = seedData.ToList();

        if (entityType.GetCustomAttribute<SplitTableAttribute>(true) != null)
        {
            var initMethod = seedType.GetMethod("Init");
            initMethod?.Invoke(Activator.CreateInstance(seedType), new object[] { dbProvider });
        }
        else
        {
            int updateCount = 0, insertCount = 0;
            if (entityInfo.Columns.Any(u => u.IsPrimarykey))
            {
                var storage = dbProvider.StorageableByObject(dataList).ToStorage();
                if (seedType.GetCustomAttribute<IgnoreUpdateSeedAttribute>() == null)
                {
                    updateCount = storage.AsUpdateable
                        .IgnoreColumns(entityInfo.Columns
                            .Where(u => u.PropertyInfo.GetCustomAttribute<IgnoreUpdateSeedColumnAttribute>() != null)
                            .Select(u => u.PropertyName).ToArray())
                        .ExecuteCommand();
                }
                insertCount = storage.AsInsertable.ExecuteCommand();
            }
            else
            {
                if (!dbProvider.Queryable(entityInfo.DbTableName, entityInfo.DbTableName).Any())
                {
                    insertCount = dataList.Count;
                    dbProvider.InsertableByObject(dataList).ExecuteCommand();
                }
            }
            Console.WriteLine($"添加数据 {entityInfo.DbTableName,-32} ({config.ConfigId} - {Interlocked.Increment(ref count):D003}/{sum:D003}，数据量：{dataList.Count:D003}，插入 {insertCount:D003} 条记录，修改 {updateCount:D003} 条记录)");
        }
    }

    /// <summary>
    /// 初始化租户业务数据库
    /// </summary>
    /// <param name="iTenant"></param>
    /// <param name="config"></param>
    public static void InitTenantDatabase(ITenant iTenant, DbConnectionConfig config)
    {
        SetDbConfig(config);

        if (!iTenant.IsAnyConnection(config.ConfigId.ToString()))
            iTenant.AddConnection(config);
        var db = iTenant.GetConnectionScope(config.ConfigId.ToString());
        db.DbMaintenance.CreateDatabase();

        // 获取所有业务表-初始化租户库表结构（排除系统表、日志表、特定库表）
        var entityTypes = App.EffectiveTypes
            .Where(u => !u.GetCustomAttributes<IgnoreTableAttribute>().Any())
            .Where(u => !u.IsInterface && !u.IsAbstract && u.IsClass && u.IsDefined(typeof(SugarTable), false) &&
            !u.IsDefined(typeof(SysTableAttribute), false) && !u.IsDefined(typeof(LogTableAttribute), false) && !u.IsDefined(typeof(TenantAttribute), false)).ToList();
        if (entityTypes.Count == 0) return;

        foreach (var entityType in entityTypes)
        {
            var splitTable = entityType.GetCustomAttribute<SplitTableAttribute>();
            if (splitTable == null)
                db.CodeFirst.InitTables(entityType);
            else
                db.CodeFirst.SplitTables().InitTables(entityType);
        }
    }

    /// <summary>
    /// 简单的重试机制
    /// </summary>
    /// <param name="action"></param>
    /// <param name="maxRetry"></param>
    /// <param name="retryIntervalMs"></param>
    private static void Retry(Action action, int maxRetry, int retryIntervalMs)
    {
        int attempt = 0;
        while (true)
        {
            try
            {
                action();
                return;
            }
            catch (SqliteException ex) when (ex.SqliteErrorCode == 5) // SQLITE_BUSY
            {
                if (++attempt >= maxRetry)
                {
                    Log.Error($"简单的重试机制:{ex.Message}"); throw;
                }
                Log.Information($"数据库忙，正在重试... (尝试 {attempt}/{maxRetry})");
                Thread.Sleep(retryIntervalMs);
            }
        }
    }
}