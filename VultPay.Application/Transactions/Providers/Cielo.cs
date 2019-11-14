using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using VultPay.Domain.Constants;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Salaros.Configuration;
using System.Globalization;

namespace VultPay.Application.Transactions.Providers
{
    public class Cielo : Inheritance.BaseApplication
    {
        private ConfigParser ConfigCielo { get; set; }

        /// <summary>
        /// 
        /// </summary>
        private DataBase.Transactions.TransactionDB TransactionDB { get; set; }

        private bool UseFraudAnalysis
        {
            get => bool.Parse(ConfigGlobal.GetValue("FraudAnalysis", "UseFraudAnalysis"));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="configuration"></param>
        public Cielo(IConfiguration configuration) : base(configuration)
        {
            //
            ConfigCielo = new ConfigParser(ConfigGlobal.GetValue("Provider.Configuration.FileLocation", "CieloConfigLocation"), new ConfigParserSettings
            {
                MultiLineValues = MultiLineValues.Simple | MultiLineValues.AllowValuelessKeys | MultiLineValues.QuoteDelimitedValues | MultiLineValues.AllowEmptyTopSection,
                Culture = new CultureInfo("en-US")
            });

            TransactionDB = new DataBase.Transactions.TransactionDB(Configuration);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static Cielo Create(IConfiguration configuration)
        {
            return new Cielo(configuration);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="transaction"></param>
        /// <returns></returns>
        public async Task<Domain.Models.Application.TransactionResult> ExecuteTransactionAsync(Domain.Models.Application.Transaction transaction,
                                                                                        Domain.Enums.TransactionType action)
        {
            try
            {
                //Cria uma instância da classe
                var CieloAdapter = Adapters.Providers.Cielo.Create(Configuration);
                //Obtem um objeto de transação da Cielo
                var CieloTransaction = CieloAdapter.GetTransaction(transaction, UseFraudAnalysis);
                //Inicia o cliente rest
                var client = CreateRestClient();
                //Configura o destino da requisição
                var request = await CreateRequestAsync(CieloTransaction, action);
                //Executa a requisiação
                var cieloRequest = await client.ExecuteTaskAsync(request);
                //Cria uma nova transação
                return await CreateTransactionAsync(cieloRequest, transaction);
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        private async Task<Domain.Models.Application.TransactionResult> CreateTransactionAsync(IRestResponse request, Domain.Models.Application.Transaction transaction)
        {
            //
            if (request.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return await GetTransactionOkAsync(request, transaction);
            }
            else if (request.StatusCode == System.Net.HttpStatusCode.Created)
            {
                return await GetTransactionCreatedAsync(request, transaction);
            }
            else if (request.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                return await GetTransactionBadRequestAsync(request, transaction);
            }
            else if (request.StatusCode == System.Net.HttpStatusCode.InternalServerError)
            {
                throw new Exception(request.Content);
            }
            else
            {
                throw new Exception("Status Code not set");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private RestClient CreateRestClient()
        {
            try
            {
                var environment = ConfigGlobal.GetValue("Application", "Environment");
                var restClient = new RestClient(ConfigCielo.GetValue($"Environment.{environment}", "Request"));
                restClient.AddDefaultHeader("MerchantId", ConfigCielo.GetValue($"Environment.{environment}", "MerchantId"));
                restClient.AddDefaultHeader("MerchantKey", ConfigCielo.GetValue($"Environment.{environment}", "MerchantKey"));
                return restClient;
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
        private async Task<RestRequest> CreateRequestAsync(Domain.Models.Providers.Cielo.Transaction transaction, Domain.Enums.TransactionType action)
        {
            try
            {
                switch (action)
                {
                    case Domain.Enums.TransactionType.Pay:
                        return CreatePaymentRequest(transaction);
                    case Domain.Enums.TransactionType.Release:
                        return await CreateReleaseRequestAsync(transaction);
                    case Domain.Enums.TransactionType.Chargeback:
                        return await CreateChargeBackRequestAsync(transaction);
                    default:
                        throw new Exception("TransactionType not set");
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
        private RestRequest CreatePaymentRequest(Domain.Models.Providers.Cielo.Transaction transaction)
        {
            try
            {
                var request = new RestRequest("/1/sales/", Method.POST);
                request.AddJsonBody(transaction);
                return request;
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
        private async Task<RestRequest> CreateReleaseRequestAsync(Domain.Models.Providers.Cielo.Transaction transaction)
        {
            try
            {
                var cielo = await GetOrderAsync(transaction);
                var request = new RestRequest("/1/sales/{PaymentId}/capture", Method.PUT);
                request.AddUrlSegment("PaymentId", cielo.Payment.PaymentId);
                return request;
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
        private async Task<RestRequest> CreateChargeBackRequestAsync(Domain.Models.Providers.Cielo.Transaction transaction, long amount = 0)
        {
            try
            {
                var cielo = await GetOrderAsync(transaction);
                var request = new RestRequest("/1/sales/{PaymentId}/void", Method.PUT);
                request.AddUrlSegment("PaymentId", cielo.Payment.PaymentId);
                //
                if (amount > 0)
                    request.AddParameter("amount", amount);
                //
                return request;
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        private async Task<Domain.Models.Application.TransactionResult> GetTransactionOkAsync(IRestResponse request, Domain.Models.Application.Transaction transaction)
        {
            try
            {
                var cieloAdapter = Adapters.Providers.Cielo.Create(Configuration);
                var order = await TransactionDB.GetLogTransactionByOrderIdAsync(transaction.OrderId);
                var cieloResponse = JsonConvert.DeserializeObject<Domain.Models.Providers.Cielo.Payment>(request.Content);
                cieloResponse.Type = order.PaymentType;
                cieloResponse.Amount = cieloAdapter.GetAmount(order.Amount);
                //
                transaction.OrderId = order.TransactionID;
                transaction.Products = order.Products;
                transaction.Customer = order.Customer;
                //
                transaction.Payment = new Domain.Models.Application.Payment
                {
                    Type = cieloAdapter.GetPaymentType(order.PaymentType),
                    Amount = order.Amount
                };
                //
                var result = cieloAdapter.GetTransactionResult(new Domain.Models.Providers.Cielo.Transaction
                {
                    Payment = cieloResponse,
                    Customer = cieloAdapter.GetCustomer(new Domain.Models.Application.Transaction { Customer = order.Customer }),
                    MerchantOrderId = transaction.OrderId,
                });
                //
                await Infra
                        .Utilities
                        .Logger
                        .Create(Configuration)
                        .RegisterLogTransactionAsync(result.Status.ToString(), transaction, result);
                //
                return result;
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        private async Task<Domain.Models.Application.TransactionResult> GetTransactionCreatedAsync(IRestResponse request, Domain.Models.Application.Transaction transaction)
        {
            try
            {
                var cieloAdapter = Adapters.Providers.Cielo.Create(Configuration);
                var cieloResponse = JsonConvert.DeserializeObject<Domain.Models.Providers.Cielo.Transaction>(request.Content);
                var result = cieloAdapter.GetTransactionResult(cieloResponse);
                //
                await Infra
                        .Utilities
                        .Logger
                        .Create(Configuration)
                        .RegisterLogTransactionAsync(result.Status.ToString(), transaction, result);
                //
                return result;
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        private async Task<Domain.Models.Application.TransactionResult> GetTransactionBadRequestAsync(IRestResponse request, Domain.Models.Application.Transaction transaction)
        {
            try
            {
                //
                var cieloAdapter = Adapters.Providers.Cielo.Create(Configuration);
                var cieloResponse = JsonConvert.DeserializeObject<List<Domain.Models.Providers.Cielo.CieloMessages>>(request.Content);
                var result = cieloAdapter.GetTransactionError(transaction, cieloResponse);
                //
                await Infra
                        .Utilities
                        .Logger
                        .Create(Configuration)
                        .RegisterLogTransactionAsync(result.Status.ToString(), transaction, result);
                //
                return result;
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
        private async Task<Domain.Models.Providers.Cielo.Transaction> GetOrderAsync(Domain.Models.Providers.Cielo.Transaction transaction)
        {
            var order = await TransactionDB.GetLogTransactionByOrderIdAsync(transaction.MerchantOrderId);
            return JsonConvert.DeserializeObject<Domain.Models.Providers.Cielo.Transaction>(order.TransactionResult.ExtraResultSet.ProviderResponse);
        }

    }
}