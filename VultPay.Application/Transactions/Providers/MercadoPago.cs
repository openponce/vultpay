using Microsoft.Extensions.Configuration;
using PuppeteerSharp;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace VultPay.Application.Transactions.Providers
{

    public class MercadoPago : Inheritance.BaseApplication
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="configuration"></param>
        public MercadoPago(IConfiguration configuration) : base(configuration)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static MercadoPago Create(IConfiguration configuration)
        {
            return new MercadoPago(configuration);
        }

        public async Task<string> GetCardToken(Domain.Models.Application.Transaction transaction)
        {
            return await Task.Run(() => { return transaction.ToString(); });
        }

    }
}
