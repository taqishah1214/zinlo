using Abp.Auditing;
using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.Text;

namespace Zinlo.Reconciliation
{
    [Audited]
    public class Amortization : CreationAuditedEntity<long>, IMustHaveTenant
    {
        [DisableAuditing]
        public int TenantId { get; set; }
        public string InoviceNo { get; set; }
        public string JournalEntryNo { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public double Amount { get; set; }
        public double AccomulateAmount { get; set; }
        public string Description { get; set; }
        [DisableAuditing]
        public virtual ChartofAccounts.ChartofAccounts ChartsofAccount { get; set; }
        [DisableAuditing]
        public long ChartsofAccountId { get; set; }
        public Criteria Criteria { get; set; }
        [DisableAuditing]
        public DateTime ClosingMonth { get; set; }


    }
    public enum Criteria
    {
        Manual = 1,
        Monthly = 2,
        Daily = 3,
    }
}


