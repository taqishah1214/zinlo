using System;
using System.Collections.Generic;
using System.Text;

namespace Zinlo.Register_User.Dto
{
    public class SubscriptionPlansDto
    {
        public int SubscriptionStartType { get; set; }
        public int EditionId { get; set; }
        public int EditionPaymentType { get; set; }
    }
}
