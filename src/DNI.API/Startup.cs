using System;
using System.IO;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

using DNI.API.Requests;
using DNI.API.Validators;
using DNI.Options;
using DNI.Services.Captcha;
using DNI.Services.Email;
using DNI.Services.Mappers;
using DNI.Services.Podcast;
using DNI.Services.Shared.Mapping;
using DNI.Services.Shared.Paging;
using DNI.Services.Shared.Sorting;
using DNI.Services.Show;

using FluentValidation;
using FluentValidation.AspNetCore;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;

using RestSharp;

using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace DNI.API {
    /// <summary>
    ///     Program bootstrap configuration
    /// </summary>
    public class Startup {
        private static string APINameSpace => Assembly.GetEntryAssembly().GetName().Name;

        private IConfiguration Configuration { get; }

        /// <summary>
        ///     Program bootstrap configuration
        /// </summary>
        /// <param name="configuration"></param>
        public Startup(IConfiguration configuration) {
            Configuration = configuration;
        }

        /// <summary>
        ///     This method gets called by the runtime. Use this method to add services to the container.
        /// </summary>
        /// <param name="services"></param>
        public void ConfigureServices(IServiceCollection services) {
            // MVC
            services
                .AddControllers()
                //.AddMvc(o => { o.EnableEndpointRouting = false; })
                //.SetCompatibilityVersion(CompatibilityVersion.Version_3_0)
                .AddJsonOptions(options => {
                    // Configure request / response json serialization options
                    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                    options.JsonSerializerOptions.Converters.Clear();
                    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());

                    //options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                    //options.SerializerSettings.Formatting = Formatting.None;
                    //options.SerializerSettings.Converters.Clear();
                    //options.SerializerSettings.Converters.Add(new StringEnumConverter {
                    //    NamingStrategy = new DefaultNamingStrategy(),
                    //    AllowIntegerValues = true
                    //});
                })
                .AddFluentValidation();

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
                .AddScoped<IShowService, ShowService>()
                .AddTransient<IShowKeywordAggregationService, ShowKeywordAggregationService>()
                .AddTransient<IPagingCalculator<PodcastShow>, PagingCalculator<PodcastShow>>()
                .AddTransient<ISorter<PodcastShow>, Sorter<PodcastShow>>();

            // Mappers
            services
                .AddTransient<IMapper<PodcastShow, Show>, PodcastShowToShowMapper>();

            // Validators
            services
                .AddTransient<IValidator<GetShowsRequest>, GetShowsRequestValidator>();

            // Swagger
            services
                .AddSwaggerGen(c => {
                    c.SwaggerDoc("v1", new OpenApiInfo {
                        Title = APINameSpace,
                        Version = $"v{GetVersion()}",
                        Description = "This API provides REST capabilities to the Documentation Not Included website"
                    });

                    // TODO: Convert all documentation urls to lowercase

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
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env) {
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

            app.UseRouting();

            // app.UseResponseCaching();

            app.UseEndpoints(endpoints => {
                endpoints.MapControllers();
            });
            // UseMVC Must come last otherwise CORS doesn't work
            //app.UseMvc();
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