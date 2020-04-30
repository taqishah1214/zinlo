using System;
using System.Collections.Generic;
using System.Text;

namespace Zinlo.Contactus.Dto
{
    public class CustomTenantRequestLinkResolverDto
    {
        public int? TenantId { get; set; }
        public int? EditionId { get; set; }
        public int? SubscriptionStartType { get; set; }
        public int? EditionPaymentType { get; set; }
        public double Price { get; set; }
    }
}
