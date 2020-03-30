using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Zinlo.AccountSubType
{
    [Table("AccountSubTypes")]
    public class AccountSubType : CreationAuditedEntity<long>, IMustHaveTenant
    {
        public int TenantId { get; set;}

        [Required]
        public virtual string Title { get; set; }
       
        public virtual string Description { get; set; }

    }
}
