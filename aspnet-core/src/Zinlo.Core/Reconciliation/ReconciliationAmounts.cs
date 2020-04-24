using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.Text;

namespace Zinlo.Reconciliation
{
   public class ReconciliationAmounts : FullAuditedEntity<long>
    {
        public DateTime ChangeDateTime { get; set; }
        public double Amount { get; set; }
        public AmountType AmountType { get; set; }
        public bool isChanged { get; set; }
        public long itemId { get; set; }

        public long ChartsofAccountId { get; set; }


    }
      public enum AmountType
        {
            Itemized = 1,
            Amortized = 2,   
        }

}
