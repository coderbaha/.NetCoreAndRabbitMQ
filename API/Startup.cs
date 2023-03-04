using API.Extensions;
using Business.Concrete;
using Business.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Publisher.Publisher;
using Publisher.Services;
using RabbitMQ.Client;
using Repository;
using Repository.Concrete;
using Repository.Interfaces;

namespace API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddControllers();
            services.AddAutoMapper(typeof(Startup));

            services.AddControllersWithViews(options =>
            options.CacheProfiles.Add("Duration45", new CacheProfile
            {
                Duration = 45
            }));
            services.AddResponseCaching();
            services.AddScoped<IDailyDocumentService, DailyDocumentManager>();

            services.AddSingleton(sp => new ConnectionFactory() {
                HostName = "localhost",
                //VirtualHost = "oksVirtualHosy-t",
                UserName = "guest",
                Password = "guest",
                /*HostName = Configuration.GetConnectionString("RabbitMQ"), *//*DispatchConsumersAsync = true*/
            });
            services.AddSingleton<RabbitMQClientService>();
            services.AddSingleton<RabbitMQPublisher>();
            services.AddSingleton<DapperContext>();
            services.AddScoped<IGetFileService, GetFileManager>();
            services.AddScoped<IGetFileSOAPService, GetFileSOAPManager>();
            services.AddScoped<ILogService, LogManager>();
            services.AddTransient<ILogRepository, LogRepository>();
            #region Configure Swagger  
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Türkiye Hayat Emeklilik", Version = "v1" });
                c.AddSecurityDefinition("basic", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "basic",
                    In = ParameterLocation.Header,
                    Description = "Basic Authorization header using the Bearer scheme."
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                          new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference
                                {
                                    Type = ReferenceType.SecurityScheme,
                                    Id = "basic"
                                }
                            },
                            new string[] {}
                    }
                });
            });
            #endregion

            services.AddAuthentication("BasicAuthentication")
    .AddScheme<AuthenticationSchemeOptions, AuthenticationHandler>("BasicAuthentication", null);

            services.AddScoped<IAuthorizeService, AuthorizeManager>();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "API v1"));
                app.UseResponseCaching();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
