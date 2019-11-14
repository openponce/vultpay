using Microsoft.Extensions.Configuration;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using VultPay.Domain.Constants;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace VultPay.DataBase.Core
{
    public sealed class RedisManager
    {
        private static CachingFramework.Redis.RedisContext instance;
        public static CachingFramework.Redis.RedisContext Init(IConfiguration configuration)
        {

            if (instance == null)
            {
                var redisConf = new StackExchange.Redis.ConfigurationOptions
                {
                    Password = "XXXXX",
                    KeepAlive = 60,
                    AbortOnConnectFail = false,
                    ConnectRetry = 1000,
                    ConnectTimeout = 10000
                };
                //
                var RedisEndPoints = configuration.GetSection("RedisEndPoints");
                //
                foreach (IConfigurationSection endPoint in RedisEndPoints.GetChildren())
                    redisConf.EndPoints.Add(endPoint.Value);
                //
                instance = new CachingFramework.Redis.RedisContext(redisConf);
            }

            return instance;
        }
    }
}
