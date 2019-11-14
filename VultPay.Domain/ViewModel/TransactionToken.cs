using VultPay.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace VultPay.Domain.ViewModel
{
    public class TransactionToken
    {
        public Int64 TransactionId { get; set; }
        public string TransactionHash { get; set; }
        public Owner Owner { get; set; }
    }
}