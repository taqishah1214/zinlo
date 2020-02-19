using System;
using System.Collections.Generic;
using System.Text;

namespace Zinlo.ChartsofAccount.Dtos
{
    public class GetAccountForEditDto
    {
        public long Id { get; set; }
        public string  AccountName { get; set; }
        public string AccountNumber { get; set; }
        public string  AccountSubType { get; set; }
        public string AssigniName { get; set; }
        public long AssigniId { get; set; }
        public int ReconcillationType { get; set; }
        public int AccountType { get; set; }
        public long AccountSubTypeId { get; set; }
    }
}
