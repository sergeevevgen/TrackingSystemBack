﻿using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.EntityFrameworkCore;
using NLog;
using ILogger = NLog.ILogger;
using TrackingSystem.Api.DataLayer.Data;
using TrackingSystem.Api.Shared.IManagers;
using TrackingSystem.Api.BusinessLogic.Managers;
using TrackingSystem.Api.DataLayer.DataAccessManagers;
using TrackingSystem.Api.BusinessLogic.DownloadLk;

namespace TrackingSystem.Api.AppLogic.Core
{
    /// <summary>
    /// https://habr.com/ru/companies/otus/articles/539762/
    /// </summary>
    public static class ServiceProviderExtension
    {
        public static void AddAllSingletones(this IServiceCollection services)
        {
            // NLog
            services.AddSingleton(typeof(ILogger), m => LogManager.GetCurrentClassLogger());
            services.AddSingleton<LkClient>();
        }

        public static void AddAllScoped(this IServiceCollection services, AppConfig appConfig)
        {
            services.AddDbContextPool<TrackingSystemContext>(
                dbContextOptions =>
                {
                    dbContextOptions.UseSqlServer(appConfig.DBConnectionString);
                    dbContextOptions.ConfigureWarnings(warnings =>
                    {

                    });
                }
            );

            // Менеджеры
            services.AddScoped<IUserManager, UserManager>();
            services.AddScoped<IIdentityManager, IdentityManager>();
            services.AddScoped<IJWTAuthManager, JWTAuthManager>();
            services.AddScoped<IJWTUserManager, JWTUserManager>();
            services.AddScoped<ISecurityTokenValidator, JwtSecurityTokenHandler>();
            services.AddScoped<IParserManager, ParserManager>();
        }

        public static void AddAllTransients(this IServiceCollection services)
        {
            // Слои доступа к данным
            services.AddTransient<IUserDbManager, UserDbManager>();
        }
    }
}
