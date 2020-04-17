using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.Text;

namespace Zinlo.Authorization.Users
{
    public class ContactUs : FullAuditedEntity<long>, IMustHaveTenant
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public string CompanyName { get; set; }
        public int NumberOfUsers { get; set; }
        public string Description { get; set; }
        public int TenantId { get; set; }
        public int Commitment { get; set; }
        public decimal Pricing { get; set; }
        public bool IsAccepted { get; set; }
    }
}
