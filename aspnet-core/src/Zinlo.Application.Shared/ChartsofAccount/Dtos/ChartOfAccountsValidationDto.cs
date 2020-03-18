using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Zinlo.ChartsofAccount.Dtos
{
   public class ChartOfAccountsValidationDto
    {
        [EmailAddress]
        public string AssigneeEmail { get; set; }
        public string AccountName { get; set; }
        public string AccountNumber { get; set; }
        public string AccountType { get; set; }
        public string AccountSubType { get; set; }
        public string ReconciliationType { get; set; }


    }
}
