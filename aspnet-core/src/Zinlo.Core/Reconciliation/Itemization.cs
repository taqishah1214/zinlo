using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.Text;

namespace Zinlo.Reconciliation
{
    public class Itemization : CreationAuditedEntity<long>, IMustHaveTenant
    {
        public int TenantId { get; set; }
        public string InoviceNo { get; set; }
        public string JournalEntryNo { get; set; }
        public DateTime Date { get; set; }
        public double Amount { get; set; }
        public string Description { get; set; }
        public virtual ChartsofAccount.ChartsofAccount ChartsofAccount { get; set; }
        public long ChartsofAccountId { get; set; }
        public DateTime ClosingMonth { get; set; }

    }
}
