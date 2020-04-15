using System;
using System.Collections.Generic;
using System.Text;

namespace Zinlo.Register_User.Dto
{
   public class RegisterUserInput
    {
        public BusinessInfo BusinessInfo { get; set; }
        public PaymentDetails PaymentDetails { get; set; }
        public PersonalInfoDto PersonalInfo { get; set; }
        public SubscriptionPlansDto SubscriptionPlans { get; set; }

    }
}
