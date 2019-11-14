using Microsoft.Extensions.Configuration;
using VultPay.DataBase.Log;
using VultPay.Domain.Constants;
using VultPay.Domain.Models.Application;
using Salaros.Configuration;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VultPay.Application.Transactions
{
    public class SinglePayment : Inheritance.BaseApplication
    {
        /// <summary>
        /// 
        /// </summary>
        private Validators.TransactionValidator ValidateTransaction { get; set; }

        /// <summary>
        /// 
        /// </summary>
        private TransactionPool TransactionPool { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="configuration"></param>
        public SinglePayment(IConfiguration configuration) : base(configuration)
        {
            ValidateTransaction = Validators.TransactionValidator.Create(Configuration);
            TransactionPool = TransactionPool.Create(Configuration);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static SinglePayment Create(IConfiguration configuration)
        {
            return new SinglePayment(configuration);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="transaction"></param>
        /// <returns></returns>
        public async Task<TransactionResult> PayAsync(Transaction transaction)
        {
            try
            {
                //
                var validateResult = await ValidateTransaction.ValidateModelAsync(transaction, Domain.Enums.TransactionType.Pay);
                if (validateResult.Status == Domain.Enums.TransactionStatus.Error)
                    return validateResult;
                //
                return await TransactionPool.ExecuteAsync(transaction, Domain.Enums.TransactionType.Pay);
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="transaction"></param>
        /// <returns></returns>
        public async Task<TransactionResult> VoidAsync(string OrderId)
        {
            try
            {
                //
                var transaction = new Transaction { OrderId = OrderId };
                var validateResult = await ValidateTransaction.ValidateModelAsync(transaction, Domain.Enums.TransactionType.Chargeback);
                if (validateResult.Status == Domain.Enums.TransactionStatus.Error)
                    return validateResult;
                //
                return await TransactionPool.ExecuteAsync(transaction, Domain.Enums.TransactionType.Chargeback);
            }
            catch
            {
                throw;
            }
        }

    }
}