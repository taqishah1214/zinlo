using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using Zinlo.Authorization.Users;

namespace Zinlo.ChartofAccounts
{
    [Table("SecondaryUserAssignee")]
    public class SecondaryUserAssignee : FullAuditedEntity<long>
    {
        public User Primary { get; set; }
        public long PrimaryId { get; set; }
        public User Secondary { get; set; }
        public long SecondaryId { get; set; }
    }

}
