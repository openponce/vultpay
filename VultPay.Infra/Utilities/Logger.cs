using Microsoft.Extensions.Configuration;
using VultPay.DataBase.Log;
using VultPay.Domain.Models.Application;
using Salaros.Configuration;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;

namespace VultPay.Infra.Utilities
{
    public class Logger
    {
        /// <summary>
        /// 
        /// </summary>
        IConfiguration Configuration { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        ConfigParser ConfigGlobal { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="configuration"></param>
        public Logger(IConfiguration configuration)
        {
            Configuration = configuration;
            //
            ConfigGlobal = new ConfigParser(Configuration.GetSection("GlobalConfiguration").Value, new ConfigParserSettings
            {
                MultiLineValues = MultiLineValues.Simple | 
                                  MultiLineValues.AllowValuelessKeys | 
                                  MultiLineValues.QuoteDelimitedValues | 
                                  MultiLineValues.AllowEmptyTopSection,
                Culture = new CultureInfo("en-US")
            });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static Logger Create(IConfiguration configuration)
        {
            return new Logger(configuration);
        }

        public async Task RegisterLogTransactionAsync(string status,
                                            Transaction transaction = null,
                                            TransactionResult transactionResult = null)
        {
            if (bool.Parse(ConfigGlobal.GetValue("Application", "RegisterTransactionLog")))
            {
                await LoggerDB
                        .Create(Configuration)
                        .RegisterLogTransactionAsync(new Domain.ViewModel.LogTransaction
                        {
                            Amount = transaction?.Payment?.Amount ?? 0,
                            Status = status,
                            Customer = transaction?.Customer,
                            Products = transaction?.Products,
                            PaymentType = transaction?.Payment?.Type.ToString(),
                            TransactionID = transaction?.OrderId,
                            TransactionDate = DateTime.Now,
                            TransactionResult = transactionResult
                        });
            }
        }
    }
}
