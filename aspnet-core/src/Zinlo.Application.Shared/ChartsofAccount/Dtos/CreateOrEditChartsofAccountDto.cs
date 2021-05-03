using Abp.Application.Services.Dto;
using System;

namespace Zinlo.ChartsofAccount.Dtos
{
    public class CreateOrEditChartsofAccountDto : CreationAuditedEntityDto<long>
    {
        public virtual string AccountName { get; set; }
        public virtual string AccountNumber { get; set; }
        public AccountType AccountType { get; set; }
        public ReconciliationType ReconciliationType { get; set; }
        public virtual long AccountSubTypeId { get; set; }
        public virtual long AssigneeId { get; set; }
        public Reconciled Reconciled { get; set; }
        public double Balance { get; set; }
        public DateTime ChangeTime { get; set; }
        public bool IsChange  { get; set; }
        public virtual string LinkedAccountNumber { get; set; }


    }
    public enum ReconciliationType
    {
        Itemized = 1,
        Amortization = 2,
    }
    public enum AccountType
    {
        Equity = 1,
        Asset = 2,
        Liability = 3
    }
    public enum Reconciled
    {
        NetAmount = 1,
        BeginningAmount = 2,
        AccruedAmount = 3,
    }
   
}
