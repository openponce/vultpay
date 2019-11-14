using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.Serializers;
using Newtonsoft.Json;
using VultPay.Domain.Enums;
using VultPay.Domain.Models.Providers.Cielo;
using VultPay.Domain.Services;
using System;
using System.Collections.Generic;
using VultPay.Domain.Iterfaces;

namespace VultPay.Domain.Models.Application
{
    public class Browser
    {
        public string FingerPrintId { get; set; }
        public bool CookiesAccepted { get; set; }
        public string Email { get; set; }
        public string IpAddress { get; set; }
        public string Type { get; set; }
    }

    public class Card
    {
        public string Token { get; set; }
        public string CardNumber { get; set; }
        public string Holder { get; set; }
        public string ExpirationMonth { get; set; }
        public string ExpirationYear { get; set; }
        public string SecurityCode { get; set; }
        public string SaveCard { get; set; }
        public string Brand { get; set; }
    }

    public class Address : Iterfaces.IAddress
    {
        public string Street { get; set; }
        public string Number { get; set; }
        public string Complement { get; set; }
        public string ZipCode { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public string District { get; set; }

        public override string ToString()
        {
            return $"{Street} {Number}, {District} - {City}/{State} - {Complement}";
        }
    }


    public class DeliveryAddress : Address, Iterfaces.IAddress
    {
    }

    public class Customer
    {
        public string Name { get; set; }
        public string Identity { get; set; }
        public string IdentityType { get; set; }
        public string Email { get; set; }
        public DateTime Birthdate { get; set; }
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

    public class DebitCard : Card { }

    public class BarCode
    {
        public int Identity { get; set; }
        public string Demonstrative { get; set; }
        public DateTime ExpirationDate { get; set; }
        public string Instructions { get; set; }
    }

    public class Payment
    {
        public Payment()
        {
            Installments = 1;
            Capture = true;
            Authenticate = false;
            IsCryptoCurrencyNegotiation = false;
        }

        public PaymentType Type { get; set; }
        public double Amount { get; set; }
        public int ServiceTaxAmount { get; set; }
        public int Installments { get; set; }
        public Interest Interest { get; set; }
        public bool Capture { get; set; }
        public bool Authenticate { get; set; }
        public string SoftDescriptor { get; set; }        
        public CreditCard CreditCard { get; set; }
        public DebitCard DebitCard { get; set; }
        public BarCode BarCode { get; set; }
        public bool IsCryptoCurrencyNegotiation { get; set; }
    }

    public class Product
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Url { get; set; }
        public long Quantity { get; set; }
        public double UnitPrice { get; set; }
    }

    public class ErrorMessages : IErrorMessages
    {
        public int Code { get; set; }
        public string ReturnCode { get; set; }
        public string Message { get; set; }
    }

    [BsonIgnoreExtraElements]

    public class Transaction
    {
        public Transaction()
        {

        }

        public string OrderId { get; set; }        
        public Browser Browser { get; set; }
        public Customer Customer { get; set; }
        public Payment Payment { get; set; }
        public Product[] Products { get; set; }
    }

    public class Link : ILink
    {
        public string Method { get; set; }
        public string Rel { get; set; }
        public string Href { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class BarCodeResultSet : IBarCodeResultSet
    {
        public string BarCodeUrl { get; set; }
        public string BarCodeNumber { get; set; }
        public string DigitableLine { get; set; }
    }

    public class ExtraResultSet : IExtraResultSet
    {
        public string ProviderMessage { get; set; }
        public string ProviderResponse { get; set; }
        public BarCodeResultSet BarCodeResultSet { get; set; }
        public List<Link> Links { get; set; }
        public List<ErrorMessages> ErrorMessages { get; set; }        
    }

    [BsonIgnoreExtraElements]

    public class TransactionResult : ITransactionResult
    {
        public string OrderId { get; set; }
        public TransactionStatus Status { get; set; }
        public PaymentType Type { get; set; }
        public double Amount { get; set; }
        public ExtraResultSet ExtraResultSet { get; set; }
    }

}