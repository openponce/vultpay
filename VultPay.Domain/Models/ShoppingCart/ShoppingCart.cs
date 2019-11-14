using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.Serializers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using VultPay.Domain.Iterfaces;
using VultPay.Domain.Services;

namespace VultPay.Domain.Models.Application.ShoppingCart
{
    public class ShoppingCart
    {
        public ShoppingCart()
        {
            CreateAt = DateTime.Now;
            Customer = new Customer();
            ShopItems = new List<ShoppingCartItem>();            
        }

        [BsonId]
        public ObjectId Id { get; set; }

        [BsonElement("CreateAt")]

        public DateTime CreateAt { get; set; }
        public Customer Customer { get; set; }
        public List<ShoppingCartItem> ShopItems { get; set; }
    }

    public class ShoppingCartItem
    {
        public ShoppingCartItem()
        {
            CreateAt = DateTime.Now;
        }

        public string Id { get; set; }
        public DateTime CreateAt { get; set; }
        public bool IsGift { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public string Thumb { get; set; }
        public double UnitPrice { get; set; }
        public int Quantity { get; set; }
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
        public Customer()
        {
            Address = new Address();
            DeliveryAddress = new DeliveryAddress();
        }

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

}