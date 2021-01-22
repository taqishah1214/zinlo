using System;
using System.Collections.Generic;
using System.Text;
using Zinlo.Contactus.Dto;

namespace Zinlo.Register_User.Dto
{
   public class RegisterUserInput
    {
        public BusinessInfo BusinessInfo { get; set; }
        public PaymentDetails PaymentDetails { get; set; }
        public PersonalInfoDto PersonalInfo { get; set; }
        public SubscriptionPlansDto SubscriptionPlans { get; set; }
        public CreateOrUpdateContactusInput ContactUs { get; set; }
        public string Link { get; set; }


    }
}
