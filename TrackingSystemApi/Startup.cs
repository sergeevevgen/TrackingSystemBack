using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using NLog;
using NLog.Config;
using NLog.Targets;
using System.IdentityModel.Tokens.Jwt;
using TrackingSystem.Api.AppLogic.Core;
using TrackingSystem.Api.BusinessLogic.Managers;
using TrackingSystem.Api.DataLayer.Data;
using TrackingSystem.Api.DataLayer.DataAccessManagers;
using TrackingSystem.Api.Shared.IManagers;

namespace TrackingSystem.Api
{
    /// <summary>
    /// Класс для настройки зависимостей
    /// </summary>
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IHostEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            Configuration = builder.Build();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            var appConfig = UpdateAppConfigFromEnvironment();

            var configBuilder = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                {
                    { "AppConfig:DBConnectionString", appConfig.DBConnectionString },
                    { "AppConfig:JWTIssuer", appConfig.JWTIssuer },
                    { "AppConfig:JWTAudience", appConfig.JWTAudience },
                    { "AppConfig:JWTAccessKey", appConfig.JWTAccessKey },
                    { "AppConfig:JWTRefreshKey", appConfig.JWTRefreshKey },
                    { "AppConfig:SenderEmail", appConfig.SenderEmail },
                    { "AppConfig:SenderPassword", appConfig.SenderPassword },
                    { "AppConfig:SmtpHost", appConfig.SmtpHost },
                    { "AppConfig:SmtpPort", appConfig.SmtpPort.ToString() },
                    { "AppConfig:EnableSsl", appConfig.EnableSsl.ToString() },
                    { "AppConfig:LkLogin", appConfig.LkLogin },
                    { "AppConfig:LkPassword", appConfig.LkPassword },
                });

            var configuration = configBuilder.Build();
            services.Configure<AppConfig>(configuration.GetSection("AppConfig"));

            services.AddHttpContextAccessor();

            services.AddAllSingletones();
            services.AddAllScoped(appConfig);
            services.AddAllTransients();

            services.AddControllers().AddNewtonsoftJson();

            services.AddDistributedMemoryCache();

            services.AddSession();

            services
                .AddAuthentication(x =>
                {
                    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                    x.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearerConfiguration(
                    appConfig.JWTIssuer,
                    appConfig.JWTAudience,
                    appConfig.JWTAccessKey);

            services.AddAuthorization();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "TrackingSystemRestAPI",
                    Version = "v1"
                });

                // Создание и добавление описания схемы безопасности "Bearer"
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "",                    // Описание схемы безопасности (пустая строка)
                    Name = "Authorization",              // Имя параметра заголовка, который будет использоваться для передачи токена
                    In = ParameterLocation.Header,       // Местоположение параметра - в заголовке запроса
                    Type = SecuritySchemeType.ApiKey,    // Тип схемы безопасности - API-ключ (API Key)
                    Scheme = "Bearer"                    // Название схемы безопасности - "Bearer" (часто используется для токенов)
                });

                // Создание и добавление требования безопасности
                c.AddSecurityRequirement(new OpenApiSecurityRequirement()
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            },
                            Scheme = "oauth2",          // Название схемы безопасности - "oauth2"
                            Name = "Bearer",           // Имя параметра заголовка, который будет использоваться для передачи токена
                            In = ParameterLocation.Header,  // Местоположение параметра - в заголовке запроса
                        },
                        new List<string>()   // Перечисление разрешений (в данном случае пустой список строк)
                    }
                });
            });
        }

        // This method gets called by the runtime. Use this method to configure the
        //HTTP request pipeline.
        public void Configure(IApplicationBuilder app)
        {
            var nLogConfig = new LoggingConfiguration();
            var logConsole = new ConsoleTarget();
            nLogConfig.AddRule(NLog.LogLevel.Info, NLog.LogLevel.Error, logConsole);
            LogManager.Configuration = nLogConfig;

            //if (env.IsDevelopment())
            //{
            IdentityModelEventSource.ShowPII = true;
            app.UseDeveloperExceptionPage();
            app.UseSwagger();
            app.UseSwaggerUI(c =>
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "TrackingSystemRestAPI v1"));
            //}
            app.UseHttpsRedirection();
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseSession();
            app.UseCors(builder => builder
               .AllowAnyHeader()
               .AllowAnyMethod()
               .SetIsOriginAllowed((host) => true)
               .AllowCredentials()
             );

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        private AppConfig UpdateAppConfigFromEnvironment()
        {
            var appConfig = Configuration.GetSection("AppConfig").Get<AppConfig>();

            // Строка подключения к БД
            appConfig.DBConnectionString = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING") ?? appConfig.DBConnectionString;

            // Издатель токена
            appConfig.JWTIssuer = Environment.GetEnvironmentVariable("JWT_ISSUER") ?? appConfig.JWTIssuer;

            // Потребитель токена
            appConfig.JWTAudience = Environment.GetEnvironmentVariable("JWT_AUDIENCE") ?? appConfig.JWTAudience;

            // Ключ для шифрации
            appConfig.JWTAccessKey = Environment.GetEnvironmentVariable("JWT_ACCESS_KEY") ?? appConfig.JWTAccessKey;

            // Ключ для обновления
            appConfig.JWTRefreshKey = Environment.GetEnvironmentVariable("JWT_REFRESH_KEY") ?? appConfig.JWTRefreshKey;

            // Почта отправителя
            appConfig.SenderEmail = Environment.GetEnvironmentVariable("SENDER_EMAIL") ?? appConfig.SenderEmail;

            // Пароль отправителя
            appConfig.SenderPassword = Environment.GetEnvironmentVariable("SENDER_PASSWORD") ?? appConfig.SenderPassword;

            // Smtp хост
            appConfig.SmtpHost = Environment.GetEnvironmentVariable("SMTP_HOST") ?? appConfig.SmtpHost;

            // Логин от лк
            appConfig.LkLogin = Environment.GetEnvironmentVariable("LK_LOGIN") ?? appConfig.LkLogin;

            // Пароль от лк
            appConfig.LkPassword = Environment.GetEnvironmentVariable("LK_PASSWORD") ?? appConfig.LkPassword;

            // Smtp порт
            if (int.TryParse(Environment.GetEnvironmentVariable("SMTP_PORT"), out int smtpPort))
            {
                appConfig.SmtpPort = smtpPort;
            }

            // Включает SSL при отправке сообщения на почту
            if (bool.TryParse(Environment.GetEnvironmentVariable("ENABLE_SSL"), out bool enableSsl))
            {
                appConfig.EnableSsl = enableSsl;
            }

            Configuration["AppConfig:DBConnectionString"] = appConfig.DBConnectionString;
            Configuration["AppConfig:JWTIssuer"] = appConfig.JWTIssuer;
            Configuration["AppConfig:JWTAudience"] = appConfig.JWTAudience;
            Configuration["AppConfig:JWTAccessKey"] = appConfig.JWTAccessKey;
            Configuration["AppConfig:JWTRefreshKey"] = appConfig.JWTRefreshKey;
            Configuration["AppConfig:SenderEmail"] = appConfig.SenderEmail;
            Configuration["AppConfig:SenderPassword"] = appConfig.SenderPassword;
            Configuration["AppConfig:SmtpHost"] = appConfig.SmtpHost;
            Configuration["AppConfig:SmtpPort"] = appConfig.SmtpPort.ToString();
            Configuration["AppConfig:EnableSsl"] = appConfig.EnableSsl.ToString();
            Configuration["AppConfig:LkLogin"] = appConfig.LkLogin;
            Configuration["AppConfig:LkPassword"] = appConfig.LkPassword;

            return appConfig;
        }
    }
}
