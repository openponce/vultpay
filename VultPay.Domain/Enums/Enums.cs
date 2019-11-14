using System;
using System.Collections.Generic;
using System.Text;

namespace VultPay.Domain.Enums
{

    public enum TransactionRisk
    {
        Undefined,
        Low,
        Medium,
        High
    }

    public enum Provider
    {
        Cielo,
        MercadoPago,
        PagSeguro,
        PayPal,
        PicPay        
    }

    public enum Interest
    {
        ByMerchant,
        ByIssuer
    }

    public enum PaymentType
    {
        CreditCard = 1,
        DebitCard = 2,
        BarCode = 3
    }

    public enum TransactionType
    {
        Pay,
        Release,
        Chargeback,
    }

    public enum TransactionStatus
    {
        NotFinished = 0,
        Authorized = 1,
        PaymentConfirmed = 2,
        Denied = 3,
        Voided = 4,
        Refunded = 5,
        Pending = 6,
        Aborted = 7,
        Scheduled = 8,
        Error = 99
    }

    public enum Collections
    {
        None,
        Application,
        Transaction,
        Log,
    };

    public enum Owner
    {
        Default
    }

    public enum AccountStatus
    {
        Blocked = 0,
        Active = 1,
        Pendent = 2,
        Suspended = 3
    }

}
