using Abp;
using System;
using System.Collections.Generic;
using System.Text;
using Zinlo.ChartsofAccount.Dtos;

namespace Zinlo.ChartsofAccount.Importing
{
   public class ImportChartsOfAccountFromExcelJobArgs
    {
        public int? TenantId { get; set; }
        public Guid BinaryObjectId { get; set; }
        public UserIdentifier User { get; set; }
    }
   
}
