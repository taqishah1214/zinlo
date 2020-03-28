using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Zinlo.ChartsofAccount.Dtos
{
   public class ChartOfAccountsValidationDto
    {
      
        public bool isTrue { get; set; }
        public string Exception { get; set; }
    }
}
