using Abp.Auditing;
using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.Text;

namespace Zinlo.Reconciliation
{
    public class Itemization : FullAuditedEntity<long>, IMustHaveTenant
    [Audited]
    public class Itemization : CreationAuditedEntity<long>, IMustHaveTenant
    {
     
        [DisableAuditing]
        public int TenantId { get; set; }
        public string InoviceNo { get; set; }
        public string JournalEntryNo { get; set; }
        public DateTime Date { get; set; }
        public double Amount { get; set; }
        public string Description { get; set; }
        [DisableAuditing]
        public virtual ChartofAccounts.ChartofAccounts ChartsofAccount { get; set; }
        [DisableAuditing]
        public long ChartsofAccountId { get; set; }
        [DisableAuditing]
        public DateTime ClosingMonth { get; set; }

    }
}
