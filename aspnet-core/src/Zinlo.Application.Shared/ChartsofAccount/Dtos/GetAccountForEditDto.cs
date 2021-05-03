using System;
using System.Collections.Generic;
using System.Text;
using Abp.Domain.Entities.Auditing;

namespace Zinlo.ChartsofAccount.Dtos
{
    public class GetAccountForEditDto : CreationAuditedEntity<long>
    {
        public string  AccountName { get; set; }
        public string AccountNumber { get; set; }
        public string  AccountSubType { get; set; }
        public string AssigniName { get; set; }
        public long AssigniId { get; set; }
        public int ReconcillationType { get; set; }
        public int AccountType { get; set; }
        public long AccountSubTypeId { get; set; }
        //public DateTime ClosingMonth { get; set; }
        public int ReconciledId  { get; set; }
        public string LinkedAccount { get; set; }
        public List<long> SecondaryId { get; set; }
    }
}
