//// Admin.NET 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。使用本项目应遵守相关法律法规和许可证的要求。
////
//// 本项目主要遵循 MIT 许可证和 Apache 许可证（版本 2.0）进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 和 LICENSE-APACHE 文件。
////
//// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目二次开发而产生的一切法律纠纷和责任，我们不承担任何责任！

//#if NET10_0_OR_GREATER

//using Microsoft.AspNetCore.Builder;
//using XiHan.Framework.Utils.Logging;
//using XiHan.Framework.Utils.Reflections;

//namespace Admin.NET.Core.Update;

///// <summary>
///// 自动版本更新中间件拓展
///// </summary>
///// <remarks>
///// 使用方法
///// 1.在 Admin.NET.Web.Core 的 Startup.cs 中的 Configure 方法中调用 app.UseAutoVersionUpdate()。
///// 2.在入口项目 Admin.NET.Web.Entry 的根目录下创建一个名为 UpdateScripts 的文件夹，并在其中放置 .sql 后缀的脚本文件
///// 3.脚本文件命名格式为版本号，例如 1.0.0.sql、1.0.1.sql 等，版本号应符合语义化版本规范。
///// 4.脚本的属性：复制到输出目录，设置为：始终复制。
///// 5.设置主节点的 Admin.NET.Application 的 Configuration/App.json 的 WorkerId 为 1。
///// 6.设置入口项目 Admin.NET.Web.Entry.csproj 的 Version。
///// ==================================================
///// 更新新版本时
///// 1.需在 UpdateScripts 文件夹中添加新的脚本文件，脚本文件名应为新版本号。
///// 2.设置入口项目 Admin.NET.Web.Entry.csproj 的 Version
///// </remarks>
//[SuppressSniffer]
//public static class AutoVersionUpdate
//{
//    /// <summary>
//    /// 使用自动版本更新中间件
//    /// </summary>
//    /// <param name="app"></param>
//    /// <returns></returns>
//    public static IApplicationBuilder UseAutoVersionUpdate(this IApplicationBuilder app)
//    {
//        LogHelper.Info("AutoVersionUpdate 中间件运行");

//        var snowIdOpt = App.GetConfig<SnowIdOptions>("SnowId", true);
//        if (snowIdOpt.WorkerId != 1)
//        {
//            LogHelper.Handle("非主节点，不执行脚本");
//            return app;
//        }

//        var currentVersion = GetEntryAssemblyCurrentVersion();
//        LogHelper.Handle($"当前版本：{currentVersion}");

//        var historyVersionInfo = GetEntryAssemblyHistoryVersionInfo();
//        var historyVersion = historyVersionInfo.Version;
//        var historyDate = historyVersionInfo.Date;
//        var historyIsRunScript = historyVersionInfo.IsRunScript;

//        LogHelper.Handle($"历史版本：{historyVersion},更新时间：{historyDate}，是否已执行{historyIsRunScript}");

//        // 历史版本为空、版本号相同，不执行脚本
//        if (historyVersion == string.Empty)
//        {
//            LogHelper.Handle("历史版本为空，默认为最新版本，不执行脚本");

//            // 保存当前版本信息
//            SetEntryAssemblyCurrentVersion(currentVersion, true);

//            return app;
//        }
//        else if (currentVersion.CompareTo(historyVersion) <= 0 && historyIsRunScript)
//        {
//            LogHelper.Handle("当前版本号与历史版本号相同，且已执行过脚本，不再执行");

//            // 保存当前版本信息
//            SetEntryAssemblyCurrentVersion(currentVersion, false);

//            return app;
//        }
//        else
//        {
//            LogHelper.Handle("当前版本号与历史版本号不同，或版本号相同但未执行过脚本，开始执行脚本");

//            var scriptSqlVersions = GetScriptSqlVersions();

//            // 若不存在当前版本的脚本，则只保存当前版本信息，不执行脚本
//            if (scriptSqlVersions.All(s => s.Version.CompareTo(currentVersion) < 0))
//            {
//                LogHelper.Handle("不存在当前版本的脚本，只保存当前版本信息，不执行脚本");

//                // 保存当前版本信息
//                SetEntryAssemblyCurrentVersion(currentVersion, false);

//                return app;
//            }

//            // 执行脚本
//            foreach (var sqlFileInfo in scriptSqlVersions)
//            {
//                var sqlVersion = sqlFileInfo.Version;

//                // 只执行大于历史版本的脚本，或者当前版本但未执行过
//                if (sqlVersion.CompareTo(historyVersion) < 0)
//                {
//                    LogHelper.Handle($"版本{sqlVersion}低于历史版本，跳过");
//                    continue;
//                }
//                if (sqlVersion == historyVersion && historyIsRunScript)
//                {
//                    LogHelper.Handle($"版本{sqlVersion}等于历史版本，且已执行过脚本，跳过");
//                    continue;
//                }

//                // 执行脚本
//                var sql = File.ReadAllText(sqlFileInfo.FilePath);
//                if (sql != null)
//                {
//                    LogHelper.Handle($"执行版本{sqlVersion}脚本");

//                    HandleSqlScript(app, sql, sqlVersion);
//                }
//            }
//        }

//        LogHelper.Success("AutoVersionUpdate 中间件结束");

//        return app;
//    }

//    #region 辅助方法

//    /// <summary>
//    /// 获取入口程序集当前版本信息
//    /// </summary>
//    /// <returns></returns>
//    private static string GetEntryAssemblyCurrentVersion()
//    {
//        var entryAssemblyVersion = ReflectionHelper.GetEntryAssemblyVersion();
//        return entryAssemblyVersion.ToString(3);
//    }

//    /// <summary>
//    /// 设置入口程序集当前版本信息
//    /// </summary>
//    /// <param name="version"></param>
//    /// <param name="isRunScript"></param>
//    private static void SetEntryAssemblyCurrentVersion(string version, bool isRunScript)
//    {
//        var path = Path.Combine(AppContext.BaseDirectory, "version.txt");
//        var now = DateTime.Now;
//        File.WriteAllText(path, $"{version}^{now:yyyy-MM-dd HH:mm:ss}^{isRunScript}");
//    }

//    /// <summary>
//    /// 获取入口程序集上一次运行版本信息
//    /// </summary>
//    /// <returns></returns>
//    private static HistoryVersionInfo GetEntryAssemblyHistoryVersionInfo()
//    {
//        var path = Path.Combine(AppContext.BaseDirectory, "version.txt");

//        // 检查文件是否存在
//        if (File.Exists(path))
//        {
//            // 文件存在时读取内容
//            var info = File.ReadAllText(path);

//            if (info.Contains('^'))
//            {
//                var parts = info.Split('^');
//                var version = parts.Length > 0 ? parts[0].ToString() : string.Empty;
//                var date = parts.Length > 1 ? parts[1] : string.Empty;
//                var isRunScript = parts.Length > 2 ? parts[2].ToBoolean() : false;

//                return new HistoryVersionInfo(version, date, isRunScript);
//            }
//        }

//        // 文件不存在或内容格式不正确时返回默认值
//        return new HistoryVersionInfo(string.Empty, string.Empty, false);
//    }

//    /// <summary>
//    /// 获取程序目录下的脚本 SQL 文件版本
//    /// </summary>
//    /// <returns></returns>
//    private static List<SqlFileInfo> GetScriptSqlVersions()
//    {
//        // 获取所有脚本文件
//        var path = Path.Combine(AppContext.BaseDirectory, "UpdateScripts");
//        var scriptFiles = Directory.GetFiles(path, "*.sql").ToList();

//        var sqlVersions = scriptFiles
//            .Select(s => new SqlFileInfo(Path.GetFileNameWithoutExtension(s), s))
//            .OrderBy(s => s.Version).ToList();
//        return sqlVersions;
//    }

//    /// <summary>
//    /// 保存当前版本信息
//    /// </summary>
//    /// <param name="app"></param>
//    /// <param name="sql"></param>
//    /// <param name="sqlVersion"></param>
//    private static void HandleSqlScript(IApplicationBuilder app, string sql, string sqlVersion)
//    {
//        using var scope = App.GetRequiredService<IServiceScopeFactory>().CreateScope();
//        var dbContext = scope.ServiceProvider.GetRequiredService<ISqlSugarClient>();

//        var isSuccess = false;

//        try
//        {
//            // 开启事务
//            dbContext.Ado.BeginTran();
//            dbContext.Ado.ExecuteCommand(sql);
//            dbContext.Ado.CommitTran();
//            isSuccess = true;
//        }
//        catch (Exception ex)
//        {
//            dbContext.Ado.RollbackTran();
//            LogHelper.Error($"AutoVersionUpdate 执行 SQL 脚本出错，版本：{sqlVersion}，错误：{ex.Message}");
//        }
//        finally
//        {
//            if (isSuccess)
//            {
//                // 保存当前版本信息
//                SetEntryAssemblyCurrentVersion(sqlVersion, true);
//            }
//        }
//    }

//    #endregion 辅助方法
//}

//public record SqlFileInfo(string Version, string FilePath);
//public record HistoryVersionInfo(string Version, string Date, bool IsRunScript);

//#endif // NET10_0_OR_GREATER