﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Zinlo.ChartsofAccount.Dtos
{
    public class ChartsOfAccountsTrialBalanceExcellImportDto
    {
        // public Reconciled Reconciled { get; set; }
        public string AccountName { get; set; }
        public string AccountNumber { get; set; }
        public string Balance { get; set; }
        public string Exception { get; set; }
        public long VersionId { get; set; }
        public bool CanBeImported()
        {
            return string.IsNullOrEmpty(Exception);
        }
    }
    public enum FileTypes
    {
        ChartOfAccounts,
        TrialBalance
    }
}
