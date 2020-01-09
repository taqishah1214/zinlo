using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using Abp.Domain.Entities;

namespace Zinlo.Categories
{
    [Table("Categories")]
    public class Category : CreationAuditedEntity, IMustHaveTenant
    {
        public int TenantId { get; set; }
        [Required]
        public virtual string Title { get; set; }
        [Required]
        public virtual string Description { get; set; }
    }
}