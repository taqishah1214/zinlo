using System;
using System.Collections.Generic;
using System.Text;

namespace Zinlo.MultiTenancy.Payments.Stripe.Dto
{
    public class StripeGetPaymentInput
    {
        public string StripeSessionId { get; set; }
    }
}
