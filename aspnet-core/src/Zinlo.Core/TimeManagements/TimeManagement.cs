using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using Abp.Domain.Entities;

namespace Zinlo.TimeManagements
{
	[Table("TimeManagements")]
    public class TimeManagement : CreationAuditedEntity<long> , IMustHaveTenant
    {
        public int TenantId { get; set; }
        public virtual DateTime Month { get; set; }
        public virtual bool Status { get; set; }

    }
}