using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using VultPay.DataBase.Core;
using VultPay.Domain.ViewModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace VultPay.DataBase.Log
{
    public class LoggerDB : BaseDB
    {
        /// <summary>
        /// 
        /// </summary>
        public LoggerDB(IConfiguration configuration)
            : base(configuration)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        public static LoggerDB Create(IConfiguration configuration)
        {
            return new LoggerDB(configuration);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="log"></param>
        /// <returns></returns>
        public async Task RegisterLogTransactionAsync(LogTransaction log)
        {
            var logTransaction = MongoDatabaseInstance.GetCollection<LogTransaction>(LogCollection);
            await logTransaction.InsertOneAsync(log);
        }
    }
}