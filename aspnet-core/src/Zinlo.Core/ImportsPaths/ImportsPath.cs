using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.Text;
using Zinlo.Authorization.Users;

namespace Zinlo.ImportsPaths
{
    public class ImportsPath : CreationAuditedEntity<long>, IMustHaveTenant
    {
        public int TenantId { get; set; }
        public string FilePath { get; set; }
        public string Type { get; set; }
        public int FailedRecordsCount { get; set; }
        public int SuccessRecordsCount { get; set;}
        public virtual User User { get; set; }
        public virtual long UserId { get; set; }
        public bool IsRollBacked { get; set; }
    }
}
