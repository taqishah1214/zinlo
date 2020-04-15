using System;
using System.Collections.Generic;
using System.Text;

namespace Zinlo.Register_User.Dto
{
   public class PaymentDetails
    {
        public string CardNumber { get; set; }
        public string CVVCode { get; set; }
        public DateTime ExpiryDate { get; set; }
        public string Email { get; set; }
        public int Commitment { get; set; }
    }
}
