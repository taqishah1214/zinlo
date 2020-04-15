using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.Text;

namespace Zinlo.Authorization.Users
{
    public class UserPaymentDetails: FullAuditedEntity<long>, IMustHaveTenant
    {
        public string CardNumber { get; set; }
        public string CVVCode { get; set; }
        public DateTime ExpiryDate { get; set; }
        public string Email { get; set; }
        public int Commitment { get; set; }
        public virtual User User { get; set; }
        public long? UserId { get; set; }
        public int TenantId { get; set; }
    }
}
