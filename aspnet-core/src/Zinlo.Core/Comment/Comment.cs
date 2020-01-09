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
        public int TenantId { get; set; }
        [Required]
        public string Type { get; set; }
        [Required]
        public int TypeId { get; set; }
        public string Body { get; set; }


    }
}
