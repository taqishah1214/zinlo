using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Zinlo.Comment
{
    [Table("Comments")]
    public class Comment : CreationAuditedEntity, IMustHaveTenant
    {
        public string Body { get; set; }
        public int TenantId { get; set; }
        [Required]
        public CommentType Type { get; set; }
        [Required]
        public long TypeId { get; set; }
    }
    public enum CommentType
    {
        ClosingChecklist = 1,
    }
}
