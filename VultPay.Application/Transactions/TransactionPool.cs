using Microsoft.Extensions.Configuration;
using VultPay.Domain.Models.Application;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace VultPay.Application.Transactions
{
    public class TransactionPool : Inheritance.BaseApplication
    {

        /// <summary>
        /// 
        /// </summary>
        private DataBase.Transactions.TransactionDB TransactionDB { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="configuration"></param>
        public TransactionPool(IConfiguration configuration) : base(configuration)
        {
            TransactionDB = new DataBase.Transactions.TransactionDB(Configuration);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static TransactionPool Create(IConfiguration configuration)
        {
            return new TransactionPool(configuration);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="transaction"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public async Task<TransactionResult> ExecuteAsync(Transaction transaction, Domain.Enums.TransactionType action)
        {
            try
            {
                switch (action)
                {
                    case Domain.Enums.TransactionType.Pay:
                        switch (transaction.Payment?.Type)
                        {
                            case Domain.Enums.PaymentType.CreditCard:
                                return await CreditCardTransactionAsync(transaction, action);
                            case Domain.Enums.PaymentType.DebitCard:
                                return await DebitCardTransactionAsync(transaction, action);
                            case Domain.Enums.PaymentType.BarCode:
                                return await BarCodeTransactionAsync(transaction, action);
                            default:
                                throw new Exception("Payment type not set");
                        }
                    case Domain.Enums.TransactionType.Release:
                        return await ReleaseTransactionAsync(transaction, action);
                    case Domain.Enums.TransactionType.Chargeback:
                        return await ChargeBackTransactionAsync(transaction, action);
                    default:
                        throw new Exception("Transaction type not set");
                }
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
        private async Task<TransactionResult> CreditCardTransactionAsync(Transaction transaction, Domain.Enums.TransactionType action)
        {
            switch (ConfigGlobal.GetValue("Payment", "CreditCardProvider").ToLower())
            {
                case "cielo":
                    return await CieloProviderAsync(transaction, action);
                case "mercadopago":
                    return await MercadoPagoProviderAsync(transaction, action);
                default:
                    throw new Exception("Provider not set");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="transaction"></param>
        /// <returns></returns>
        private async Task<TransactionResult> DebitCardTransactionAsync(Transaction transaction, Domain.Enums.TransactionType action)
        {
            switch (ConfigGlobal.GetValue("Payment", "DebitCardProvider").ToLower())
            {
                case "cielo":
                    return await CieloProviderAsync(transaction, action);
                default:
                    throw new Exception("Provider not set");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="transaction"></param>
        /// <returns></returns>
        private async Task<TransactionResult> BarCodeTransactionAsync(Transaction transaction, Domain.Enums.TransactionType action)
        {
            switch (ConfigGlobal.GetValue("Payment", "BarCodeProvider").ToLower())
            {
                case "cielo":
                    return await CieloProviderAsync(transaction, action);
                default:
                    throw new Exception("Provider not set");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="transaction"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        private async Task<TransactionResult> ChargeBackTransactionAsync(Transaction transaction, Domain.Enums.TransactionType action)
        {
            try
            {
                string provider;
                var order = await TransactionDB.GetLogTransactionByOrderIdAsync(transaction.OrderId);
                //
                switch (Enum.Parse(typeof(Domain.Enums.PaymentType), order.PaymentType))
                {
                    case Domain.Enums.PaymentType.CreditCard:
                        provider = ConfigGlobal.GetValue("Payment", "CreditCardProvider").ToLower();
                        break;
                    case Domain.Enums.PaymentType.DebitCard:
                        provider = ConfigGlobal.GetValue("Payment", "DebitCardProvider").ToLower();
                        break;
                    default:
                        throw new Exception("Payment type not set");
                }
                //
                switch (provider)
                {
                    case "cielo":
                        return await CieloProviderAsync(transaction, action);
                    default:
                        throw new Exception("Provider not set");
                }
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
        /// <param name="action"></param>
        /// <returns></returns>
        private async Task<TransactionResult> ReleaseTransactionAsync(Transaction transaction, Domain.Enums.TransactionType action)
        {
            try
            {
                switch (ConfigGlobal.GetValue("Payment", "CreditCardProvider").ToLower())
                {
                    case "cielo":
                        return await CieloProviderAsync(transaction, action);
                    default:
                        throw new Exception("Provider not set");
                }
            }
            catch
            {
                throw;
            }
        }

        #region Providers

        /// <summary>
        /// 
        /// </summary>
        /// <param name="transaction"></param>
        private async Task<TransactionResult> CieloProviderAsync(Transaction transaction, Domain.Enums.TransactionType action)
        {
            try
            {
                return await Providers
                                .Cielo
                                .Create(Configuration)
                                .ExecuteTransactionAsync(transaction, action);
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
        /// <param name="action"></param>
        /// <returns></returns>
        private async Task<TransactionResult> MercadoPagoProviderAsync(Transaction transaction, Domain.Enums.TransactionType action)
        {
            try
            {
                //
                var cardToken = await Providers
                                .MercadoPago
                                .Create(Configuration)
                                .GetCardToken(transaction);
                //
                return new TransactionResult()
                {
                    ExtraResultSet = new ExtraResultSet
                    {
                        ProviderResponse = cardToken
                    }
                };
            }
            catch
            {
                throw;
            }
        }

        #endregion

    }
}