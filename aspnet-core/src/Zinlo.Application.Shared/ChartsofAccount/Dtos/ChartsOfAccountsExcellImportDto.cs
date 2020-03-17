using System;
using System.Collections.Generic;
using System.Text;

namespace Zinlo.ChartsofAccount.Dtos
{
 public   class ChartsOfAccountsExcellImportDto
    {
       // public Reconciled Reconciled { get; set; }
        public string AccountName { get; set; }
        public string AccountNumber { get; set; }
        public string AccountType { get; set; }
        public string AssignedUser { get; set; }
        public string AccountSubType{ get; set; }
        public string ReconciliationType{ get; set; }
        public string Exception { get; set; }
        public bool CanBeImported()
        {
            return string.IsNullOrEmpty(Exception);
        }
    }
   
}
