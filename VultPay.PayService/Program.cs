using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using VultPay.Infra.Core.Services;

namespace VultPay.PayService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
              .UseKestrel(options =>
              {
                  options.Listen(IPAddress.Any, 80);
                  options.Listen(IPAddress.Any, 443, listenOptions =>
                  {
                      listenOptions.UseHttps("/vultpay_server.pfx", "B76ADC7F261246F9A3B30512A4CA16B6");
                  });
              })
            .ConfigureAppConfiguration(config =>
            {
                config = ConfigureServicesDefault.AddDefaultAppSettings(config);
            })
            .UseStartup<Startup>();
    }
}
