using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using VultPay.DataBase.Log;
using VultPay.Domain.Constants;
using Salaros.Configuration;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace VultPay.Application.Adapters.Providers
{
    public class Cielo
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
        ConfigParser FraudAnalysisConfig { get; set; }

        /// <summary>
        /// 
        /// </summary>
        ConfigParser ConfigCielo { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool UseFraudAnalysis { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="configuration"></param>
        public Cielo(IConfiguration configuration)
        {
            Configuration = configuration;
            //
            ConfigGlobal = new ConfigParser(Configuration.GetSection("GlobalConfiguration").Value, new ConfigParserSettings
            {
                MultiLineValues = MultiLineValues.Simple | MultiLineValues.AllowValuelessKeys | MultiLineValues.QuoteDelimitedValues | MultiLineValues.AllowEmptyTopSection,
                Culture = new CultureInfo("en-US")
            });
            //
            ConfigCielo = new ConfigParser(ConfigGlobal.GetValue("Provider.Configuration.FileLocation", "CieloConfigLocation"), new ConfigParserSettings
            {
                MultiLineValues = MultiLineValues.Simple | MultiLineValues.AllowValuelessKeys | MultiLineValues.QuoteDelimitedValues | MultiLineValues.AllowEmptyTopSection,
                Culture = new CultureInfo("en-US")
            });
            //
            FraudAnalysisConfig = new ConfigParser(ConfigGlobal.GetValue("FraudAnalysis", "ConfigLocation"), new ConfigParserSettings
            {
                MultiLineValues = MultiLineValues.Simple | MultiLineValues.AllowValuelessKeys | MultiLineValues.QuoteDelimitedValues | MultiLineValues.AllowEmptyTopSection,
                Culture = new CultureInfo("en-US")
            });
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
        public Domain.Models.Providers.Cielo.Transaction GetTransaction(Domain.Models.Application.Transaction transaction, bool useFraudAnalysis = false)
        {
            try
            {
                //
                UseFraudAnalysis = useFraudAnalysis;
                //
                return new Domain.Models.Providers.Cielo.Transaction
                {
                    Payment = GetPayment(transaction),
                    Customer = GetCustomer(transaction),
                    MerchantOrderId = transaction.OrderId
                };
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="transactionCielo"></param>
        /// <returns></returns>
        public Domain.Models.Application.TransactionResult GetTransactionResult(Domain.Models.Providers.Cielo.Transaction transactionCielo)
        {
            try
            {
                return new Domain.Models.Application.TransactionResult
                {
                    Type = GetPaymentType(transactionCielo.Payment.Type),
                    Status = GetPaymentStatus(transactionCielo.Payment.Status),
                    Amount = ConvertAmount(transactionCielo.Payment.Amount),
                    OrderId = transactionCielo.MerchantOrderId,
                    ExtraResultSet = GetExtraResultSet(transactionCielo),
                };
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cieloMessages"></param>
        /// <returns></returns>
        public Domain.Models.Application.TransactionResult GetTransactionError(Domain.Models.Application.Transaction transaction,
                                                                               List<Domain.Models.Providers.Cielo.CieloMessages> cieloMessages)
        {
            try
            {
                //
                var _errorMessages = GetErrorMessages(cieloMessages);
                //
                return new Domain.Models.Application.TransactionResult
                {
                    OrderId = transaction.OrderId,
                    Status = Domain.Enums.TransactionStatus.Error,
                    Amount = transaction.Payment.Amount,
                    ExtraResultSet = new Domain.Models.Application.ExtraResultSet
                    {
                        ProviderResponse = JsonConvert.SerializeObject(_errorMessages),
                        ErrorMessages = _errorMessages
                    }
                };
                //
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
        public Domain.Models.Providers.Cielo.Customer GetCustomer(Domain.Models.Application.Transaction transaction)
        {
            try
            {
                if (transaction.Customer == null)
                    return null;
                //
                return GetCustomer(transaction.Customer);
            }
            catch
            {
                throw;
            }
        }

        public Domain.Models.Providers.Cielo.Customer GetCustomer(Domain.Models.Application.Customer transaction)
        {
            try
            {
                return new Domain.Models.Providers.Cielo.Customer
                {
                    Name = transaction.Name,
                    Email = transaction.Email,
                    Address = GetAddress(transaction.Address),
                    Identity = NumericOnly(transaction.Identity).ToString(),
                    Birthdate = transaction.Birthdate.ToString("yyyy-MM-dd"),
                    IdentityType = transaction.IdentityType,
                    DeliveryAddress = GetAddress(transaction.DeliveryAddress)
                };
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
        private Domain.Models.Providers.Cielo.Payment GetPayment(Domain.Models.Application.Transaction transaction)
        {
            try
            {
                if (transaction.Payment == null)
                    return null;
                //
                switch (transaction.Payment.Type)
                {
                    case Domain.Enums.PaymentType.CreditCard:
                        return CreateCreditCardTransaction(transaction);
                    case Domain.Enums.PaymentType.DebitCard:
                        //TODO
                        return CreateDebitCardTransaction(transaction);
                    case Domain.Enums.PaymentType.BarCode:
                        //TODO
                        return CreateBarCodeTransaction(transaction);
                    default:
                        throw new Exception();
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
        private Domain.Models.Providers.Cielo.Payment CreateCreditCardTransaction(Domain.Models.Application.Transaction transaction)
        {
            try
            {
                return new Domain.Models.Providers.Cielo.Payment
                {
                    Type = "CreditCard",
                    Amount = GetAmount(transaction.Payment.Amount),
                    Capture = true,
                    Interest = GetInterest(transaction.Payment.Interest),
                    CreditCard = GetCreditCard(transaction.Payment.CreditCard),
                    Authenticate = transaction.Payment.Authenticate,
                    Installments = GetInstallments(transaction.Payment.Installments),
                    FraudAnalysis = GetFraudAnalysis(transaction),
                    SoftDescriptor = transaction.Payment.SoftDescriptor,
                    IsCryptoCurrencyNegotiation = transaction.Payment.IsCryptoCurrencyNegotiation,
                };
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
        private Domain.Models.Providers.Cielo.Payment CreateDebitCardTransaction(Domain.Models.Application.Transaction transaction)
        {
            try
            {
                return new Domain.Models.Providers.Cielo.Payment
                {
                    Type = "DebitCard",
                    Authenticate = transaction.Payment.Authenticate,
                };
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
        private Domain.Models.Providers.Cielo.Payment CreateBarCodeTransaction(Domain.Models.Application.Transaction transaction)
        {
            try
            {
                return new Domain.Models.Providers.Cielo.Payment
                {
                    Type = "Boleto",
                    Amount = GetAmount(transaction.Payment.Amount),
                    Address = transaction.Customer.Address.ToString(),
                    Provider = ConfigCielo.GetValue("Provider.Configuration.FileLocation", "BarCodeProvider"),
                    Assignor = transaction.Payment.SoftDescriptor,
                    BoletoNumber = transaction.Payment.BarCode.Identity.ToString(),
                    Instructions = transaction.Payment.BarCode.Instructions,
                    Demonstrative = transaction.Payment.BarCode.Demonstrative,
                    ExpirationDate = transaction.Payment.BarCode.ExpirationDate.ToString("yyyy-MM-dd"),
                    Identification = NumericOnly(transaction.Customer.Identity).ToString()
                };
            }
            catch
            {

                throw;
            }
        }

        private Domain.Iterfaces.IAddress GetAddress(Domain.Iterfaces.IAddress address)
        {
            try
            {
                if (address == null)
                    return null;
                return new Domain.Models.Providers.Cielo.Address
                {
                    City = address.City,
                    State = address.State,
                    Number = address.Number,
                    Street = address.Street,
                    ZipCode = address.ZipCode,
                    Country = address.Country,
                    District = address.District,
                    Complement = address.Complement
                };
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public Domain.Enums.PaymentType GetPaymentType(string type)
        {
            switch (type)
            {
                case "CreditCard":
                    return Domain.Enums.PaymentType.CreditCard;
                case "DebitCard":
                    return Domain.Enums.PaymentType.DebitCard;
                case "Boleto":
                    return Domain.Enums.PaymentType.BarCode;
                default:
                    throw new Exception("PaymentType not set");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="creditCard"></param>
        /// <returns></returns>
        private Domain.Models.Providers.Cielo.CreditCard GetCreditCard(Domain.Models.Application.CreditCard creditCard)
        {
            try
            {
                return new Domain.Models.Providers.Cielo.CreditCard
                {
                    Brand = creditCard.Brand,
                    Holder = creditCard.Holder,
                    SaveCard = creditCard.SaveCard,
                    CardNumber = NumericOnly(creditCard.CardNumber).ToString(),
                    SecurityCode = creditCard.SecurityCode,
                    ExpirationDate = $"{creditCard.ExpirationMonth}/{creditCard.ExpirationYear}"
                };
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="installments"></param>
        /// <returns></returns>
        private int GetInstallments(int installments)
        {
            return installments > 0 ? installments : 1;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="interest"></param>
        /// <returns></returns>
        private string GetInterest(Domain.Enums.Interest interest)
        {
            switch (interest)
            {
                case Domain.Enums.Interest.ByMerchant:
                    return "ByMerchant";
                case Domain.Enums.Interest.ByIssuer:
                    return "ByIssuer";
                default:
                    return "ByMerchant";
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        private Domain.Enums.TransactionStatus GetPaymentStatus(int status)
        {
            switch (status)
            {
                case 0:
                    return Domain.Enums.TransactionStatus.NotFinished;
                case 1:
                    return Domain.Enums.TransactionStatus.Authorized;
                case 2:
                    return Domain.Enums.TransactionStatus.PaymentConfirmed;
                case 3:
                    return Domain.Enums.TransactionStatus.Denied;
                case 10:
                    return Domain.Enums.TransactionStatus.Voided;
                case 11:
                    return Domain.Enums.TransactionStatus.Refunded;
                case 12:
                    return Domain.Enums.TransactionStatus.Pending;
                case 13:
                    return Domain.Enums.TransactionStatus.Aborted;
                case 20:
                    return Domain.Enums.TransactionStatus.Scheduled;
                default:
                    throw new Exception("Status not set in GetPaymentStatus");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="transaction"></param>
        /// <returns></returns>
        private Domain.Models.Providers.Cielo.FraudAnalysis GetFraudAnalysis(Domain.Models.Application.Transaction transaction)
        {
            try
            {
                //
                if (UseFraudAnalysis &&
                    transaction.Payment.Type == Domain.Enums.PaymentType.CreditCard)
                {
                    return new Domain.Models.Providers.Cielo.FraudAnalysis
                    {
                        Cart = GetCart(transaction, FraudAnalysisConfig),
                        Browser = GetBrowser(transaction.Browser),
                        Sequence = FraudAnalysisConfig.GetValue("FraudAnalysis", "Sequence"),
                        Provider = FraudAnalysisConfig.GetValue("FraudAnalysis", "Provider"),
                        FingerPrintId = transaction.Browser.FingerPrintId ?? Guid.NewGuid().ToString(),
                        VoidOnHighRisk = bool.Parse(FraudAnalysisConfig.GetValue("FraudAnalysis", "VoidOnHighRisk")),
                        TotalOrderAmount = GetAmount(transaction.Payment.Amount),
                        SequenceCriteria = FraudAnalysisConfig.GetValue("FraudAnalysis", "SequenceCriteria"),
                        CaptureOnLowRisk = bool.Parse(FraudAnalysisConfig.GetValue("FraudAnalysis", "CaptureOnLowRisk"))
                    };
                }
                else
                {
                    return null;
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
        /// <param name="configParser"></param>
        /// <returns></returns>
        private Domain.Models.Providers.Cielo.Cart GetCart(Domain.Models.Application.Transaction transaction, ConfigParser configParser)
        {
            try
            {
                return new Domain.Models.Providers.Cielo.Cart
                {
                    Type = configParser.GetValue("FraudAnalysis.Cart", "Type"),
                    Items = GetItems(transaction, configParser)
                };
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
        /// <param name="configParser"></param>
        /// <returns></returns>
        private List<Domain.Models.Providers.Cielo.Item> GetItems(Domain.Models.Application.Transaction transaction, ConfigParser configParser)
        {
            try
            {
                var item = new List<Domain.Models.Providers.Cielo.Item>();
                //
                for (int index = 0; index < transaction.Products.Length; index++)
                {
                    item.Add(new Domain.Models.Providers.Cielo.Item
                    {
                        Sku = transaction.Products[index].Id,
                        Name = transaction.Products[index].Name,
                        Type = configParser.GetValue("FraudAnalysis.Cart.Items", "Type"),
                        Risk = configParser.GetValue("FraudAnalysis.Cart.Items", "Risk"),
                        Quantity = transaction.Products[index].Quantity,
                        UnitPrice = GetAmount(transaction.Products[index].UnitPrice),
                        TimeHedge = configParser.GetValue("FraudAnalysis.Cart.Items", "TimeHedge"),
                        HostHedge = configParser.GetValue("FraudAnalysis.Cart.Items", "HostHedge"),
                        PhoneHedge = configParser.GetValue("FraudAnalysis.Cart.Items", "PhoneHedge"),
                        GiftCategory = configParser.GetValue("FraudAnalysis.Cart.Items", "GiftCategory"),
                        VelocityHedge = configParser.GetValue("FraudAnalysis.Cart.Items", "VelocityHedge"),
                        NonSensicalHedge = configParser.GetValue("FraudAnalysis.Cart.Items", "NonSensicalHedge"),
                        ObscenitiesHedge = configParser.GetValue("FraudAnalysis.Cart.Items", "ObscenitiesHedge"),
                    });
                }
                //
                return item;
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Browser"></param>
        /// <returns></returns>
        private Domain.Models.Providers.Cielo.Browser GetBrowser(Domain.Models.Application.Browser Browser)
        {
            try
            {
                if (Browser == null)
                    return null;
                //
                return new Domain.Models.Providers.Cielo.Browser
                {
                    CookiesAccepted = Browser.CookiesAccepted,
                    Email = Browser.Email,
                    IpAddress = Browser.IpAddress,
                    Type = Browser.Type
                };
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
        private Domain.Models.Application.BarCodeResultSet GetBarCodeResultSet(Domain.Models.Providers.Cielo.Transaction transaction)
        {
            try
            {
                if (transaction.Payment.Type.ToLower() != "boleto")
                    return null;
                //
                return new Domain.Models.Application.BarCodeResultSet
                {
                    BarCodeUrl = transaction.Payment.Url,
                    BarCodeNumber = transaction.Payment.BarCodeNumber,
                    DigitableLine = transaction.Payment.DigitableLine
                };
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
        /// <param name="analysisResult"></param>
        /// <returns></returns>
        private Domain.Models.Application.ExtraResultSet GetExtraResultSet(Domain.Models.Providers.Cielo.Transaction transaction)
        {
            try
            {
                return new Domain.Models.Application.ExtraResultSet
                {
                    Links = GetLinks(transaction),
                    ErrorMessages = GetErrorMessages(transaction.CieloMessages),
                    ProviderMessage = transaction.Payment.ReturnMessage,
                    ProviderResponse = JsonConvert.SerializeObject(transaction, Formatting.Indented),
                    BarCodeResultSet = GetBarCodeResultSet(transaction)
                };
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
        private List<Domain.Models.Application.Link> GetLinks(Domain.Models.Providers.Cielo.Transaction transaction)
        {
            try
            {
                var links = new List<Domain.Models.Application.Link>();
                foreach (var link in transaction.Payment.Links)
                {
                    links.Add(new Domain.Models.Application.Link
                    {
                        Rel = link.Rel,
                        Href = link.Href,
                        Method = link.Method
                    });
                }

                return links;
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
        private List<Domain.Models.Application.ErrorMessages> GetErrorMessages(List<Domain.Models.Providers.Cielo.CieloMessages> cieloMessages)
        {
            try
            {
                if (cieloMessages == null)
                    return null;
                //
                var errorMessages = new List<Domain.Models.Application.ErrorMessages>();
                foreach (var messages in cieloMessages)
                {
                    errorMessages.Add(new Domain.Models.Application.ErrorMessages
                    {
                        Code = messages.Code,
                        Message = messages.Message,
                        ReturnCode = messages.ReturnCode
                    });
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
        /// <param name="amount"></param>
        /// <returns></returns>
        private double ConvertAmount(long amount)
        {
            try
            {
                var rx = new Regex("^(?<integer>[0-9]+)(?<decimal>[0-9]{2})$");
                if (rx.IsMatch(amount.ToString()))
                {
                    var values = rx.Match(amount.ToString());
                    var partInt = values.Groups["integer"];
                    var partDec = values.Groups["decimal"];
                    return double.Parse($"{partInt}.{partDec}", new CultureInfo("en"));
                }
                return 0.0;
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="amount"></param>
        /// <returns></returns>
        public long GetAmount(double amount)
        {
            try
            {
                return Convert.ToInt64(amount.ToString("0.00", new CultureInfo("en")).Replace(".", ""));
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private long NumericOnly(string value)
        {
            try
            {
                return long.Parse(new Regex("[^0-9]").Replace(value, ""));
            }
            catch
            {
                throw;
            }
        }

    }
}