using System;
using System.Collections.Generic;
using System.Text;

namespace VultPay.Domain.Iterfaces
{
    public interface IErrorMessages
    {
        int Code { get; set; }
        string ReturnCode { get; set; }
        string Message { get; set; }
    }
}
