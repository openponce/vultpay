using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VultPay.Models.MercadoPago
{

    public class Payer
    {
        public string email { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public Identification identification { get; set; }
        public Address address { get; set; }
    }

    public class Identification
    {
        public string type { get; set; }
        public string number { get; set; }
    }

    public class Metadata
    {
        public string key1 { get; set; }
        public string key2 { get; set; }
    }

    public class Item
    {
        public string id { get; set; }
        public string title { get; set; }
        public string picture_url { get; set; }
        public string description { get; set; }
        public string category_id { get; set; }
        public int quantity { get; set; }
        public int unit_price { get; set; }
    }

    public class Phone
    {
        public string area_code { get; set; }
        public string number { get; set; }
    }

    public class Address
    {
        public string street_name { get; set; }
        public string street_number { get; set; }
        public string zip_code { get; set; }
        public string city { get; set; }

        public string federal_unit { get; set; }
    }

    public class Payer2
    {
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string registration_date { get; set; }
        public Phone phone { get; set; }
        public Address address { get; set; }
    }

    public class ReceiverAddress
    {
        public string zip_code { get; set; }
        public string street_name { get; set; }
        public int street_number { get; set; }
        public int floor { get; set; }
        public string apartment { get; set; }
    }

    public class Shipments
    {
        public ReceiverAddress receiver_address { get; set; }
    }

    public class AdditionalInfo
    {
        public List<Item> items { get; set; }
        public Payer2 payer { get; set; }
        public Shipments shipments { get; set; }
    }

    public class MercadoPagoModel
    {
        public float transaction_amount { get; set; }
        public string token { get; set; }
        public string description { get; set; }
        public int installments { get; set; }
        public string payment_method_id { get; set; }
        public Payer payer { get; set; }
        public string external_reference { get; set; }
        public Metadata metadata { get; set; }
        public string statement_descriptor { get; set; }
        public string notification_url { get; set; }
        public AdditionalInfo additional_info { get; set; }
    }
}