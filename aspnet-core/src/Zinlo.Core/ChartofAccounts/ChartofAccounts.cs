using System;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Auditing;
using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Zinlo.Authorization.Users;

namespace Zinlo.ChartofAccounts
{
    [Table("ChartofAccounts")]
    [Audited]
    public class ChartofAccounts : FullAuditedEntity<long>, IMustHaveTenant
    {
        [DisableAuditing]
        public override DateTime? LastModificationTime { get; set; }
        [DisableAuditing]
        public override long? LastModifierUserId { get; set; }
        [DisableAuditing]
        public int TenantId { get; set; }
        [DisableAuditing]
        public string AccountNumber { get; set; }
        public string AccountName { get; set; }
        public AccountType AccountType { get; set; }
        public ReconciliationType ReconciliationType { get; set; }
        public long AssigneeId { get; set; }
        public virtual User Assignee { get; set; }
        public long AccountSubTypeId { get; set; }
        //public Status Status { get; set; }
        public virtual AccountSubType.AccountSubType AccountSubType { get; set; }
        public Reconciled Reconciled { get; set; }
        public bool IsChange { get; set; }
        public DateTime? ChangeTime { get; set; }
        public bool IsUserDeleted { get; set; }

    }
    public enum ReconciliationType
    {
        Itemized = 1,
        Amortization = 2,
    }
    public enum AccountType
    {
        Equity = 1,
        Assets = 2,
        Liability = 3
    }

    public enum Status
    {
        InProcess = 1,
        Open = 2,
        Complete = 3
    }

    public enum Reconciled
    {
        None = 0,
        NetAmount = 1,
        BeginningAmount = 2,
        AccruedAmount = 3,
    }


    [Table("AccountBalance")]
    public class AccountBalance : CreationAuditedEntity<long>
    {
        public long AccountId { get; set; }
        public virtual ChartofAccounts Account { get; set; }
        public double Balance { get; set; }
        public double TrialBalance { get; set; }
        public DateTime Month { get; set; }
        public bool CheckAsReconcilied { get; set; }
        public Status Status { get; set; }


    }

}
