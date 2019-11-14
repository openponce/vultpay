using System;
using System.Collections.Generic;
using System.Text;
using VultPay.Domain.Enums;
using VultPay.Domain.Models.Application;

namespace VultPay.Domain.Iterfaces
{
    public interface ITransactionResult
    {
        string OrderId { get; set; }
        TransactionStatus Status { get; set; }
        PaymentType Type { get; set; }
        double Amount { get; set; }
        ExtraResultSet ExtraResultSet { get; set; }
    }

    public interface IExtraResultSet
    {
        string ProviderMessage { get; set; }
        string ProviderResponse { get; set; }
        BarCodeResultSet BarCodeResultSet { get; set; }
        List<Link> Links { get; set; }
        List<ErrorMessages> ErrorMessages { get; set; }
    }

    public interface IBarCodeResultSet
    {
        string BarCodeUrl { get; set; }
        string BarCodeNumber { get; set; }
        string DigitableLine { get; set; }
    }
}
