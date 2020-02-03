using System;
using System.IO;
using System.Reflection;

using DNI.Options;
using DNI.Services.Captcha;
using DNI.Services.Email;
using DNI.Services.Podcast;
using DNI.Services.ShowList;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

using RestSharp;

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

            // 3rd Party Services
            services
                .AddTransient<ISmtpClient, SmtpClient>()
                .AddTransient<IRestClient, RestClient>();

            // Services
            services
                .AddTransient<ICaptchaService, CaptchaService>()
                .AddTransient<IEmailService, SystemNetEmailService>()
                .AddTransient<IPodcastService, FiresidePodcastService>()
                .AddTransient<IShowListService, ShowListService>();

            // MVC
            services
                .AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_2)
                .AddJsonOptions(options => {
                    // Configure request / response json serialization options
                    options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                    options.SerializerSettings.Formatting = Formatting.None;
                    options.SerializerSettings.Converters.Clear();
                    options.SerializerSettings.Converters.Add(new StringEnumConverter {
                        NamingStrategy = new DefaultNamingStrategy(),
                        AllowIntegerValues = true
                    });
                });

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
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", $"DNI Stream API v{GetVersion()}");
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