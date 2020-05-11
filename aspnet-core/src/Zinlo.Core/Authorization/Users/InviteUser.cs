using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.Text;

namespace Zinlo.Authorization.Users
{
    public class InviteUser : FullAuditedEntity<long>, IMustHaveTenant
    {

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Title { get; set; }
        public string RoleId { get; set; }
        public string UserName { get; set; }
        public string ReportingRelationship { get; set; }
        public int TenantId { get; set; }
    }
}
