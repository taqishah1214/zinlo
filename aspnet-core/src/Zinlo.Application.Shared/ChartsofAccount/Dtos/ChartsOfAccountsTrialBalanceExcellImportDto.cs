using System;
using System.Collections.Generic;
using System.Text;

namespace Zinlo.ChartsofAccount.Dtos
{
    public class ChartsOfAccountsTrialBalanceExcellImportDto
    {
        // public Reconciled Reconciled { get; set; }
        public string AccountName { get; set; }
        public string AccountNumber { get; set; }
        public string TrialBalance { get; set; }
        public string Exception { get; set; }
        public bool CanBeImported()
        {
            return string.IsNullOrEmpty(Exception);
        }
    }
}
