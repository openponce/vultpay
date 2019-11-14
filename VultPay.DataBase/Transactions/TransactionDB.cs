using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoDB.Driver;
using VultPay.DataBase.Core;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace VultPay.DataBase.Transactions
{
    public class TransactionDB : BaseDB
    {
        public TransactionDB(IConfiguration configuration)
            : base(configuration) { }

        /// <summary>
        /// Cria o objeto com todas as dependencias
        /// </summary>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static TransactionDB Create(IConfiguration configuration)
        {
            return new TransactionDB(configuration);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="OrderId"></param>
        public async Task<Domain.ViewModel.LogTransaction> GetLogTransactionByOrderIdAsync(string OrderId)
        {
            try
            {
                var collection = MongoDatabaseInstance.GetCollection<Domain.ViewModel.LogTransaction>(LogCollection);
                var filter = Builders<Domain.ViewModel.LogTransaction>.Filter.Eq("TransactionResult.OrderId", OrderId);
                return await collection.Find(filter).FirstOrDefaultAsync();
            }
            catch
            {
                throw;
            }
        }
    }
}