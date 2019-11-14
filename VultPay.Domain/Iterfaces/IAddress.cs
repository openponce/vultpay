using System;
using System.Collections.Generic;
using System.Text;

namespace VultPay.Domain.Iterfaces
{
    public interface IAddress
    {
        string Street { get; set; }
        string Number { get; set; }
        string Complement { get; set; }
        string ZipCode { get; set; }
        string City { get; set; }
        string State { get; set; }
        string Country { get; set; }
        string District { get; set; }
    }

}
