using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Zinlo.ChartofAccounts
{
    [Table("ItemPermissions")]
    public class ItemPermissions : FullAuditedEntity<long>, IMustHaveTenant
    {
        public int TenantId { get; set; }
        public long ItemId { get; set; }
        public int UserId { get; set; }
        public ItemType Type { get; set; }
        public bool IsPrimary { get; set; }
    }
    public enum ItemType
    {
        Checklist =1,
        ChartOfAccount=2,
    }
}
