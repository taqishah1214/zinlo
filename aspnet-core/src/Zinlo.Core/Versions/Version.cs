using System;
using System.Collections.Generic;
using System.Text;
using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;

namespace Zinlo.Versions
{
    public class Version : CreationAuditedEntity<long>,IMustHaveTenant
    {
        public string Body { get; set; }
        public bool  Active { get; set; }
        public Type Type { get; set; }
        public long TypeId { get; set; }
        public int TenantId { get; set; }
    }

    public enum Type
    {
        ClosingChecklist =1,
        ChartOfAccount =2,
    }
}
