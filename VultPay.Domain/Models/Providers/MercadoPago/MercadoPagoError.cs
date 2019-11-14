using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VultPay.Models
{

    public class Cause
    {
        public int code { get; set; }
        public string description { get; set; }
        public object data { get; set; }
    }

    public class MercadoPagoError
    {
        public string message { get; set; }
        public string error { get; set; }
        public int status { get; set; }
        public List<Cause> cause { get; set; }
    }
}