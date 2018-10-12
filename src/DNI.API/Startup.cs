using DNI.Options;
using DNI.Services.Captcha;
using DNI.Services.Email;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using RestSharp;

using SendGrid;

using Swashbuckle.AspNetCore.Swagger;

namespace DNI.API {
    public class Startup {
        private readonly ILogger<Startup> _logger;

        public Startup(IConfiguration configuration, ILogger<Startup> logger) {
            _logger = logger;
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services) {
            // Options
            services.Configure<CAPTCHAOptions>(Configuration.GetSection("CAPTCHA"));
            services.Configure<GeneralOptions>(Configuration.GetSection("General"));

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
                .AddTransient<IEmailService, SystemNetEmailService>();

            // MVC
            services
                .AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            // Swagger
            services.AddSwaggerGen(c => {
                c.SwaggerDoc("v1", new Info {Title = "DNI API", Version = "v1"});
            });

            // CORS
            services.AddCors();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env) {
            if(env.IsDevelopment()) {
                app.UseDeveloperExceptionPage();

                // Swagger
                app.UseSwagger();
                app.UseSwaggerUI(c => {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "DNI API v1");
                    c.RoutePrefix = string.Empty;
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

            // UseMVC Must come last otherwise CORS doesn't work
            app.UseMvc();
        }
    }
}