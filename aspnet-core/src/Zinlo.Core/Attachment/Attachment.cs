using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Zinlo.Attachment
{
    public class Attachment : CreationAuditedEntity<long>
    {
        public long TypeId { get; set; }
        public AttachmentType Type { get; set; }
        public string FilePath { get; set; }
    }

    public enum AttachmentType
    {
        ClosingChecklist = 1,
        Amortized = 2,
        Itemized = 3
    }
}
