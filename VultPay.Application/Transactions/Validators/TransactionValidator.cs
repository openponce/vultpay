using Microsoft.Extensions.Configuration;
using VultPay.Domain.Models.Application;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VultPay.Application.Transactions.Validators
{
    public class TransactionValidator : Inheritance.BaseApplication
    {

        DataBase.Transactions.TransactionDB TransactionDB { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="configuration"></param>
        public TransactionValidator(IConfiguration configuration) : base(configuration)
        {
            //
            TransactionDB = new DataBase.Transactions.TransactionDB(Configuration);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static TransactionValidator Create(IConfiguration configuration)
        {
            return new TransactionValidator(configuration);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task<TransactionResult> ValidateModelAsync(Transaction transaction, Domain.Enums.TransactionType Action)
        {
            //
            var transactionResult = new TransactionResult
            {
                ExtraResultSet = new ExtraResultSet()
            };
            //
            var errorMessages = new List<ErrorMessages>();
            //
            switch (Action)
            {
                case Domain.Enums.TransactionType.Pay:
                    errorMessages.AddRange(ValidateCustomer(transaction));
                    errorMessages.AddRange(ValidateProducts(transaction));
                    errorMessages.AddRange(await ValidatePaymentAsync(transaction));
                    break;
                case Domain.Enums.TransactionType.Release:
                    break;
                case Domain.Enums.TransactionType.Chargeback:
                    errorMessages.AddRange(ValidateChargeback(transaction));
                    break;
                default:
                    break;
            }

            //
            if (errorMessages.Count > 0)
                transactionResult.Status = Domain.Enums.TransactionStatus.Error;
            //
            transactionResult.ExtraResultSet.ErrorMessages = errorMessages;
            //
            return transactionResult;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="transaction"></param>
        /// <returns></returns>
        private List<ErrorMessages> ValidateCustomer(Transaction transaction)
        {
            //
            var errorMessages = new List<ErrorMessages>();
            //
            if (transaction.Customer == null)
            {
                errorMessages.Add(new ErrorMessages { Code = 10, Message = "Customer is required" });
            }
            else
            {
                //
                if (string.IsNullOrEmpty(transaction.Customer.Name))
                    errorMessages.Add(new ErrorMessages { Code = 11, Message = "Customer name is required" });
                //
                if (!string.IsNullOrEmpty(transaction.Customer.Email) &&
                    !Infra.Core.Extensions.ValidateEmail(transaction.Customer.Email))
                    errorMessages.Add(new ErrorMessages { Code = 12, Message = "Customer email is invalid" });
                //
                if (transaction.Customer.Birthdate == null ||
                    transaction.Customer.Birthdate < new DateTime(1753, 01, 01))
                    errorMessages.Add(new ErrorMessages { Code = 13, Message = "Birthdate is required" });

                //
                if (string.IsNullOrEmpty(transaction.Customer.Identity))
                    errorMessages.Add(new ErrorMessages { Code = 14, Message = "Identity Type is required" });
                else
                {
                    if (!Infra.Core.Validation.Identity.IsValid(transaction.Customer.Identity))
                        errorMessages.Add(new ErrorMessages { Code = 14, Message = "Identity is invalid" });
                }

                if (transaction.Payment?.Type == Domain.Enums.PaymentType.BarCode)
                {
                    if (string.IsNullOrEmpty(transaction.Customer.Address.Street))
                        errorMessages.Add(new ErrorMessages { Code = 15, Message = "Customer Address - street is required" });

                    if (string.IsNullOrEmpty(transaction.Customer.Address.Number))
                        errorMessages.Add(new ErrorMessages { Code = 15, Message = "Customer Address - number is required" });

                    if (string.IsNullOrEmpty(transaction.Customer.Address.District))
                        errorMessages.Add(new ErrorMessages { Code = 15, Message = "Customer Address - district is required" });

                    if (string.IsNullOrEmpty(transaction.Customer.Address.City))
                        errorMessages.Add(new ErrorMessages { Code = 15, Message = "Customer Address - city is required" });

                    if (string.IsNullOrEmpty(transaction.Customer.Address.State))
                        errorMessages.Add(new ErrorMessages { Code = 15, Message = "Customer Address - state is required" });

                    if (string.IsNullOrEmpty(transaction.Customer.Address.ZipCode))
                        errorMessages.Add(new ErrorMessages { Code = 15, Message = "Customer Address - zipCode is required" });
                }
            }
            //
            return errorMessages;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="transaction"></param>
        /// <returns></returns>
        private List<ErrorMessages> ValidateProducts(Transaction transaction)
        {
            try
            {
                var errorMessages = new List<ErrorMessages>();

                if (transaction.Products == null || transaction.Products?.Length == 0)
                {
                    errorMessages.Add(new ErrorMessages { Code = 20, Message = "Product is required" });
                }
                else
                {
                    if (transaction.Products.Where(c => string.IsNullOrEmpty(c.Id)).FirstOrDefault() != null)
                        errorMessages.Add(new ErrorMessages { Code = 21, Message = "Product ID is required" });

                    if (transaction.Products.Where(c => string.IsNullOrEmpty(c.Name)).FirstOrDefault() != null)
                        errorMessages.Add(new ErrorMessages { Code = 22, Message = "Product Name is required" });

                    if (transaction.Products.Where(c => c.Quantity <= 0).FirstOrDefault() != null)
                        errorMessages.Add(new ErrorMessages { Code = 23, Message = "Product Quantity is required" });

                    if (transaction.Products.Where(c => c.UnitPrice <= 0).FirstOrDefault() != null)
                        errorMessages.Add(new ErrorMessages { Code = 24, Message = "Product Unit Price is required" });
                }

                return errorMessages;
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
        private async Task<List<ErrorMessages>> ValidatePaymentAsync(Transaction transaction)
        {
            try
            {
                //
                var errorMessages = new List<ErrorMessages>();
                //
                if (transaction.Payment == null)
                {
                    errorMessages.Add(new ErrorMessages { Code = 30, Message = "Payment is required" });
                }
                else
                {
                    //
                    if (string.IsNullOrEmpty(transaction.OrderId))
                        errorMessages.Add(new ErrorMessages { Code = 31, Message = "Order Id is required" });
                    else
                    {
                        if (RegisterTransactionLog)
                        {
                            var getTransaction = await TransactionDB.GetLogTransactionByOrderIdAsync(transaction.OrderId);
                            if (getTransaction != null)
                                errorMessages.Add(new ErrorMessages { Code = 31, Message = "Order Id already exists" });
                        }
                    }
                    //
                    if (string.IsNullOrEmpty(transaction.Payment.SoftDescriptor))
                        errorMessages.Add(new ErrorMessages { Code = 32, Message = "SoftDescriptor is required" });
                    //
                    if (transaction.Payment.Amount <= 0)
                        errorMessages.Add(new ErrorMessages { Code = 33, Message = "Amount must be greater than zero" });
                }
                //
                return errorMessages;
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
        private List<ErrorMessages> ValidateChargeback(Transaction transaction)
        {
            try
            {
                var errorMessages = new List<ErrorMessages>();
                //
                if (string.IsNullOrEmpty(transaction.OrderId))                
                    errorMessages.Add(new ErrorMessages { Code = 30, Message = "Order Id is required" });
                //
                return errorMessages;
            }
            catch
            {
                throw;
            }
        }
    }
}