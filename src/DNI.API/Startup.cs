using System;
using System.IO;
using System.Reflection;

using DNI.Options;
using DNI.Services.Captcha;
using DNI.Services.Email;
using DNI.Services.Podcast;
using DNI.Services.ShowList;
using DNI.Services.Vodcast;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;

using RestSharp;

using SendGrid;

using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace DNI.API {
    /// <summary>
    ///     Program bootstrap configuration
    /// </summary>
    public class Startup {
        private readonly ILogger<Startup> _logger;

        private static string APINameSpace => Assembly.GetEntryAssembly().GetName().Name;

        private IConfiguration Configuration { get; }

        /// <summary>
        ///     Program bootstrap configuration
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="logger"></param>
        public Startup(IConfiguration configuration, ILogger<Startup> logger) {
            _logger = logger;
            Configuration = configuration;
        }

        /// <summary>
        ///     This method gets called by the runtime. Use this method to add services to the container.
        /// </summary>
        /// <param name="services"></param>
        public void ConfigureServices(IServiceCollection services) {
            // Options
            services.Configure<CAPTCHAOptions>(Configuration.GetSection("CAPTCHA"));
            services.Configure<GeneralOptions>(Configuration.GetSection("General"));
            services.Configure<YouTubeOptions>(Configuration.GetSection("YouTube"));

            // 3rd Party Services
            var sendGridAPIKey = Configuration.GetSection("SendGrid").GetValue("ApiKey", "");
            services
                .AddTransient<ISmtpClient, SmtpClient>()
                .AddTransient<IRestClient, RestClient>()
                .AddTransient<ISendGridClient>(p => new SendGridClient(sendGridAPIKey));

            // Services
            services
                .AddTransient<ICaptchaService, CaptchaService>()
                // .AddTransient<IEmailService, SendGridEmailService>();
                .AddTransient<IEmailService, SystemNetEmailService>()
                .AddTransient<IPodcastService, FiresidePodcastService>()
                .AddTransient<IVodcastService, YouTubeVodcastService>()
                .AddTransient<IShowListService, ShowListService>();

            // MVC
            services
                .AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            // Swagger
            services.AddSwaggerGen(c => {
                c.SwaggerDoc("v1", new Info {
                    Title = APINameSpace,
                    Version = $"v{GetVersion()}",
                    Description = "This API provides REST capabilities to the Documentation Not Included website"
                });

                // Convert all documentation urls to lowercase
                c.DocumentFilter<LowercaseDocumentFilter>();

                // Include code comments in API documentation
                var appPath = AppDomain.CurrentDomain.BaseDirectory;
                foreach(var file in Directory.GetFiles(appPath, "*.xml")) {
                    c.IncludeXmlComments(file);
                }
            });

            // Response caching
            // services.AddResponseCaching();

            // CORS
            services.AddCors();
        }

        /// <summary>
        ///     This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// </summary>
        /// <param name="app"></param>
        /// <param name="env"></param>
        public void Configure(IApplicationBuilder app, IHostingEnvironment env) {
            if(env.IsDevelopment()) {
                app.UseDeveloperExceptionPage();

                // Swagger
                app.UseSwagger();
                app.UseSwaggerUI(c => {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", $"DNI API v{GetVersion()}");
                    c.RoutePrefix = string.Empty;
                    c.DocumentTitle = $"{APINameSpace} v{GetVersion()} UI";
                    c.DocExpansion(DocExpansion.None);
                });
            } else {
                app.UseHsts();
            }

            var corsUris = Configuration.GetSection("AllowedHosts").Get<string[]>();
            app.UseCors(builder => builder
                .WithOrigins(corsUris)
                .WithExposedHeaders("Content-Disposition")
                .AllowAnyHeader()
                .AllowAnyMethod());

            app.UseHttpsRedirection();

            // Response caching
            // app.UseResponseCaching();

            // UseMVC Must come last otherwise CORS doesn't work
            app.UseMvc();
        }

        /// <summary>
        ///     Retrieves the string version from the appsettings.json file
        /// </summary>
        /// <returns></returns>
        private string GetVersion() {
            return Configuration.GetSection("General").GetValue("Version", "UNKNOWN");
        }
    }
}