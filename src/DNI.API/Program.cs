using System;

using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;

using NLog;
using NLog.Web;

namespace DNI.API {
    public class Program {
        public static void Main(string[] args) {

            var logger = NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger();
            try {
                logger.Debug("API startup");
                CreateWebHostBuilder(args)
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


            CreateWebHostBuilder(args)
                .Build()
                .Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .ConfigureLogging(logging => {
                    logging.ClearProviders();
                    logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
                })
                .UseNLog();
    }
}