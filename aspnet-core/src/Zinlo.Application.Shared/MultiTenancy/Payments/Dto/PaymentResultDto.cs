using System;
using System.Collections.Generic;
using System.Text;

namespace Zinlo.MultiTenancy.Payments.Dto
{
    public class PaymentResultDto
    {
        public string TenancyName { get; set; }
        public bool PaymentStatus { get; set; }
    }
}
