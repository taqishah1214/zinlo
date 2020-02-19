using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace Zinlo.ChartsofAccount.Dtos
{
   public class CreateOrEditChartsofAccountDto : EntityDto<int?>
    {
        public virtual string AccountName { get; set; }
        public virtual string AccountNumber { get; set; }
        public AccountType AccountType { get; set; }
        public ReconciliationType ReconciliationType { get; set; }
        public virtual long AccountSubTypeId { get; set; }
        public virtual long AssigneeId { get; set; }
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
}
