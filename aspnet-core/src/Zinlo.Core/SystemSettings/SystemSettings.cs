using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.Text;

namespace Zinlo.SystemSettings
{
   public class SystemSettings : FullAuditedEntity<long>, IMustHaveTenant
    {
        public int TenantId { get; set; }
        public SettingType SettingType { get; set; }
        public DateTime Month { get; set; }
        public bool IsWeekEndEnable { get; set; }



    }

    public enum SettingType
    {
        DefaultMonth = 1,      
    }
}
