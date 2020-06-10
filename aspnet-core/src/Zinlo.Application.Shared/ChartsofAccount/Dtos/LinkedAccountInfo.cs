using System;
using System.Collections.Generic;
using System.Text;

namespace Zinlo.ChartsofAccount.Dtos
{
   public class LinkedAccountInfo
    {
        public long LinkedAccountId { get; set; }
        public double Balance { get; set; }

        public double TrialBalance { get; set; }
    }
}
