using VultPay.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace VultPay.Domain.ViewModel
{
    public class ApplicationAuth
    {
        public int AppId { get; set; }
        public Guid PublicKey { get; set; }
        public string SecretKey { get; set; }
    }

    public class ApplicationDetails
    {
        public int AppId { get; set; }
        public Guid AppUid { get; set; }
        public Owner Owner { get; set; }       
        public string AppDescription { get; set; }
        public AccountStatus AccountStatus { get; set; }
    }
}