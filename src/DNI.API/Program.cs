using System;

using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using NLog;
using NLog.Web;

namespace DNI.API {
    /// <summary>
    ///     Entry point
    /// </summary>
    public class Program {
        /// <summary>
        ///     Entry point
        /// </summary>
        /// <param name="args"></param>
        public static void Main(string[] args) {
            var logger = NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger();
            try {
                logger.Debug("API startup");
                CreateHostBuilder(args)
                    .Build()
                    .Run();
            } catch(Exception ex) {
                logger.Error(ex, "API startup failure");
                throw;
            } finally {
                // Flush to stop internal timers/threads before application-exit (avoid segmentation fault on Linux)
                logger.Debug("API shutdown");
                LogManager.Shutdown();
            }
        }

        private static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder => {
                    webBuilder.UseStartup<Startup>();
                })
                .ConfigureLogging(logging => {
                    logging.ClearProviders();
                    logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
                })
                .UseNLog();
    }
}