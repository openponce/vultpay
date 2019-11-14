using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VultPay.Models.MercadoPago.Response
{
    public class Payer
    {
        public string type { get; set; }
        public object id { get; set; }      
        public object entity_type { get; set; }
        public string email { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public Identification identification { get; set; }
        public Address address { get; set; }
        public Phone phone { get; set; }
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

        public int net_received_amount { get; set; }
        public int total_paid_amount { get; set; }
        public int overpaid_amount { get; set; }
        public string external_resource_url { get; set; }
        public int installment_amount { get; set; }
        public object financial_institution { get; set; }
        public string payment_method_reference_id { get; set; }

        public List<Item> items { get; set; }
        public Payer2 payer { get; set; }
        public Shipments shipments { get; set; }

    }

    public class TransactionDetails
    {
        public int net_received_amount { get; set; }
        public int total_paid_amount { get; set; }
        public int overpaid_amount { get; set; }
        public string external_resource_url { get; set; }
        public int installment_amount { get; set; }
        public object financial_institution { get; set; }
        public string payment_method_reference_id { get; set; }
    }

    public class Barcode
    {
    }

    public class Card
    {
    }
    public class Order
    {
    }

    public class MercadoPagoResponse
    {
        public int transaction_amount { get; set; }
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
        public int id { get; set; }
        public string date_created { get; set; }
        public object date_approved { get; set; }
        public string date_last_updated { get; set; }
        public object money_release_date { get; set; }
        public string operation_type { get; set; }
        public object issuer_id { get; set; }
        public string payment_type_id { get; set; }
        public string status { get; set; }
        public string status_detail { get; set; }
        public string currency_id { get; set; }
        public bool live_mode { get; set; }
        public object sponsor_id { get; set; }
        public object authorization_code { get; set; }
        public int collector_id { get; set; }

        public Order order { get; set; }

        public int transaction_amount_refunded { get; set; }
        public int coupon_amount { get; set; }
        public object differential_pricing_id { get; set; }
        public object deduction_schema { get; set; }
        public TransactionDetails transaction_details { get; set; }
        public List<object> fee_details { get; set; }
        public bool captured { get; set; }
        public bool binary_mode { get; set; }
        public object call_for_authorize_id { get; set; }
        public Barcode barcode { get; set; }
        public Card card { get; set; }
        public List<object> refunds { get; set; }
        public object processing_mode { get; set; }
        public object merchant_account_id { get; set; }
        public object acquirer { get; set; }
        public object merchant_number { get; set; }

    }
}