using System;
using System.Collections.Generic;
using System.Text;

namespace VultPay.Domain.Iterfaces
{
    public interface ILink
    {
        string Method { get; set; }
        string Rel { get; set; }
        string Href { get; set; }
    }
}
