using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Zinlo.Attachment
{
    [Table("Attachments")]
    public class Attachment : CreationAuditedEntity, IMustHaveTenant
    {
        public int TenantId { get; set; }
        public string Type { get; set; }
        public int TypeId { get; set; }
        public string FilePath { get; set; }
    }
}
