using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using VultPay.Infra.Core.Base;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Server.IISIntegration;
using System.Runtime.InteropServices;
using System.IO;
using Microsoft.Extensions.Configuration;
using VultPay.Domain.Constants;

namespace VultPay.Infra.Core.Services
{
    public static class ConfigureServicesDefault
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="services"></param>
        public static void AddDefaultConfigureServices(this IServiceCollection services)
        {
            ///Politicas de autorização
            services.AddAuthorization(options =>
            {
                ///Politica de autorização padrão
                options.AddPolicy("Default", (policy) =>
                {
                    policy.Requirements.Add(new AuthorizeRequirement(Domain.Enums.Owner.Default));
                });
            });

            ///Singleton do manipulador usado para 
            ///validação de requisições nos módulos do sistema
            services.AddSingleton<IAuthorizationHandler, AuthorizeHandler>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        public static IConfigurationBuilder AddDefaultAppSettings(IConfigurationBuilder config)
        {
            var AppSettingsJsonFile = GetAppSettingsFileLocation();
            if (File.Exists(AppSettingsJsonFile))
            {
                config.AddJsonFile(AppSettingsJsonFile);
            }
            else
            {
                Console.WriteLine("");
                Console.WriteLine("|=========================================|");
                Console.WriteLine("|AppSettingsJsonFile not found!!!         |");
                Console.WriteLine("|=========================================|");
                Console.WriteLine("");
            }
            return config;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static string GetAppSettingsFileLocation()
        {
            return Environment.GetEnvironmentVariable("APPSETTINGS_FILELOCATION");
        }
    }
}