using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace VultPay.Domain.ViewModel
{
    [BsonIgnoreExtraElements]
    public class LogTransaction
    {

        public LogTransaction()
        {
            TransactionDate = DateTime.Now;
        }

        public string TransactionID { get; set; }
        public DateTime TransactionDate { get; set; }
        public string PaymentType { get; set; }
        public string Status { get; set; }
        public double Amount { get; set; }
        public Models.Application.Customer Customer { get; set; }
        public Models.Application.Product[] Products { get; set; }
        public Models.Application.TransactionResult TransactionResult { get; set; }
    }
}
