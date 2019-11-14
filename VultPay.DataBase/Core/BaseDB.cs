using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using SHA3.Net;
using System;
using System.Collections.Generic;
using System.Text;

namespace VultPay.DataBase.Core
{
    public class BaseDB
    {
        internal string DB_NAME = "vultpay";
        internal string SystemCollection = "system_log";
        internal string LogCollection = "transaction_log";
        internal string CartCollection = "cart_shop";

        internal IMongoDatabase MongoDatabaseInstance { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        internal IConfiguration Configuration { get; set; }

        /// <summary>
        /// 
        /// </summary>
        internal BaseDB()
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="configuration"></param>
        internal BaseDB(IConfiguration configuration)
        {
            Configuration = configuration;
            MongoDatabaseInstance = RepositoryMongoDB.Configure(Configuration, DB_NAME);
        }

        /// <summary>
        /// Faz o HASH SHA-3 da senha
        /// </summary>
        /// <param name="Password"></param>
        /// <returns></returns>
        internal string HashPassword(string Password)
        {
            StringBuilder x = new StringBuilder();

            foreach (var item in Sha3.Sha3256().ComputeHash(Encoding.UTF8.GetBytes(Password)))
            {
                x.Append(item.ToString("x2"));
            }

            return x.ToString();
        }

    }
}