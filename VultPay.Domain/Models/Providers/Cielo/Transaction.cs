using System;
using System.Collections.Generic;

using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

using System.Linq;
using System.Web;
using VultPay.Domain.Iterfaces;
using VultPay.Domain.Services;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Bson.Serialization.Attributes;

namespace VultPay.Domain.Models.Providers.Cielo
{
    public partial class ReplyData
    {
        public long Score { get; set; }
        public string BinCountry { get; set; }
        public string CardIssuer { get; set; }
        public string CardScheme { get; set; }
        public long HostSeverity { get; set; }
        public string InternetInfoCode { get; set; }
        public string IpCity { get; set; }
        public string IpCountry { get; set; }
        public string IpRoutingMethod { get; set; }
        public string IpState { get; set; }
        public string ScoreModelUsed { get; set; }
        public string VelocityInfoCode { get; set; }
        public long CasePriority { get; set; }
        public string ProviderTransactionId { get; set; }
    }

    public class FraudAnalysis
    {
        public string Provider { get; set; }
        public string Sequence { get; set; }
        public string SequenceCriteria { get; set; }
        public bool CaptureOnLowRisk { get; set; }
        public bool VoidOnHighRisk { get; set; }
        public long TotalOrderAmount { get; set; }
        public string FingerPrintId { get; set; }
        public ReplyData ReplyData { get; set; }
        public Browser Browser { get; set; }
        public Cart Cart { get; set; }
        public List<MerchantDefinedField> MerchantDefinedFields { get; set; }
        public Shipping Shipping { get; set; }
        public Travel Travel { get; set; }
    }

    public class Browser
    {
        public bool CookiesAccepted { get; set; }
        public string Email { get; set; }
        public string HostName { get; set; }
        public string IpAddress { get; set; }
        public string Type { get; set; }
    }

    public class Cart
    {
        public bool IsGift { get; set; }
        public string Type { get; set; }
        public bool ReturnsAccepted { get; set; }
        public List<Item> Items { get; set; }
    }

    public class Item
    {
        public string GiftCategory { get; set; }
        public string HostHedge { get; set; }
        public string NonSensicalHedge { get; set; }
        public string ObscenitiesHedge { get; set; }
        public string PhoneHedge { get; set; }
        public string Name { get; set; }
        public long Quantity { get; set; }
        public string Sku { get; set; }
        public long UnitPrice { get; set; }
        public string Risk { get; set; }
        public string TimeHedge { get; set; }
        public string Type { get; set; }
        public string VelocityHedge { get; set; }
        public Passenger Passenger { get; set; }
    }

    public class Passenger
    {
        public string Email { get; set; }
        public long Identity { get; set; }
        public string Name { get; set; }
        public string Rating { get; set; }
        public long Phone { get; set; }
        public string Status { get; set; }
    }

    public class MerchantDefinedField
    {
        public long Id { get; set; }
        public string Value { get; set; }
    }

    public class Shipping
    {
        public string Addressee { get; set; }
        public string Method { get; set; }
        public long Phone { get; set; }
    }

    public class Travel
    {
        public DateTimeOffset DepartureTime { get; set; }
        public string JourneyType { get; set; }
        public string Route { get; set; }
        public List<Leg> Legs { get; set; }
    }

    public class Leg
    {
        public string Destination { get; set; }
        public string Origin { get; set; }
    }

    public class Card
    {
        public string CardNumber { get; set; }
        public string Holder { get; set; }
        public string ExpirationDate { get; set; }
        public string SecurityCode { get; set; }
        public string SaveCard { get; set; }
        public string Brand { get; set; }
    }

    public class CieloMessages
    {
        public int Code { get; set; }
        public string ReturnCode { get; set; }
        public string Message { get; set; }
    }

    public class Link
    {
        public string Method { get; set; }
        public string Rel { get; set; }
        public string Href { get; set; }
    }

    public class Address : IAddress
    {
        public string Street { get; set; }
        public string Number { get; set; }
        public string Complement { get; set; }
        public string ZipCode { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public string District { get; set; }
    }

    public class DeliveryAddress : Address, IAddress
    {
    }

    public class Customer
    {
        public string Name { get; set; }
        public string Identity { get; set; }
        public string IdentityType { get; set; }
        public string Email { get; set; }
        public string Birthdate { get; set; }
        [JsonConverter(typeof(ConcreteTypeConverter<Address>))]
        [BsonSerializer(typeof(ImpliedImplementationInterfaceSerializer<Iterfaces.IAddress, Address>))]
        public Iterfaces.IAddress Address { get; set; }
        [JsonConverter(typeof(ConcreteTypeConverter<DeliveryAddress>))]
        [BsonSerializer(typeof(ImpliedImplementationInterfaceSerializer<Iterfaces.IAddress, DeliveryAddress>))]
        public Iterfaces.IAddress DeliveryAddress { get; set; }
    }

    public class CardOnFile
    {
        public string Usage { get; set; }
        public string Reason { get; set; }
    }

    public class CreditCard : Card
    {
        public CardOnFile CardOnFile { get; set; }
    }

    public class DebitCard : Card
    {
    }

    public class Payment
    {
        public string Type { get; set; }
        public long Amount { get; set; }
        public int ServiceTaxAmount { get; set; }
        public int Installments { get; set; }
        public string Interest { get; set; }
        public bool Capture { get; set; }
        public bool Authenticate { get; set; }
        public string SoftDescriptor { get; set; }
        public CreditCard CreditCard { get; set; }
        public DebitCard DebitCard { get; set; }
        public bool IsCryptoCurrencyNegotiation { get; set; }
        public bool Tryautomaticcancellation { get; set; }
        public long ProofOfSale { get; set; }
        public string Tid { get; set; }
        public long AuthorizationCode { get; set; }
        public Guid PaymentId { get; set; }
        public long CapturedAmount { get; set; }
        public string Country { get; set; }
        public string Currency { get; set; }
        public List<object> ExtraDataCollection { get; set; }
        public int Status { get; set; }
        public int ReturnCode { get; set; }
        public string ReturnMessage { get; set; }
        public string Provider { get; set; }
        public string Address { get; set; }
        public string BoletoNumber { get; set; }
        public string Assignor { get; set; }
        public string Demonstrative { get; set; }
        public string ExpirationDate { get; set; }
        public string Identification { get; set; }
        public string Instructions { get; set; }
        public string Url { get; set; }
        public string ReturnUrl { get; set; }
        public string AuthenticationUrl { get; set; }
        public string Number { get; set; }
        public string BarCodeNumber { get; set; }
        public string DigitableLine { get; set; }
        public FraudAnalysis FraudAnalysis { get; set; }
        public List<Link> Links { get; set; }
    }

    public class Transaction
    {
        public string MerchantOrderId { get; set; }
        public Customer Customer { get; set; }
        public Payment Payment { get; set; }
        public List<CieloMessages> CieloMessages { get; set; }
    }
}