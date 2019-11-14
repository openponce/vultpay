using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace VultPay.Models.MercadoPago
{
    public class MercadoPagoRequest
    {
        public string ExternalId { get; set; }
        public string Amount { get; set; }
        public string PayerEmail { get; set; }
        public string MercadoPagoPublicKey { get; set; }
        public string MercadoPagoAccessToken { get; set; }
        public string Status { get; set; }
        public string StatusDetails { get; set; }

    }
}