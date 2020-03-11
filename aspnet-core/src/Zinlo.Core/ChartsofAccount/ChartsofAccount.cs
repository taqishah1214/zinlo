using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using Zinlo.Authorization.Users;

namespace Zinlo.ChartsofAccount
{
    [Table("ChartsofAccount")]
    public class ChartsofAccount : CreationAuditedEntity<long>, IMustHaveTenant
    {
        public int TenantId { get; set; }
        public string AccountNumber { get; set; }
        public string AccountName { get; set; }
        public AccountType AccountType { get; set; }
        public ReconciliationType ReconciliationType { get; set; }
        public long AssigneeId { get; set; }
        public virtual User Assignee { get; set; }
        public long AccountSubTypeId { get; set; }
        public Status Status { get; set; }
        public virtual AccountSubType.AccountSubType AccountSubType { get; set; }
        public Reconciled Reconciled { get; set; }
        public double Balance { get; set; }

    }
    public enum ReconciliationType
    {
        Itemized = 1,
        Amortization = 2,
    }
    public enum AccountType
    {
        Fixed = 1,
        Assets = 2,
        Liability = 3
    }

    public enum  Status
    {
        InProcess = 1,
        Open = 2 , 
        Complete = 3
    }

    public enum Reconciled
    {
        NetAmount = 1,
        BeginningAmount = 2,
        AccruedAmount = 3,
    }
}
