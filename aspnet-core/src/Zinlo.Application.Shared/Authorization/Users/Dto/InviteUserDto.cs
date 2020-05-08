using System;
using System.Collections.Generic;
using System.Text;

namespace Zinlo.Authorization.Users.Dto
{
    public class InviteUserDto
    {
        public long Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Title { get; set; }
        public int? RoleId { get; set; }
        public string ReportingRelationship { get; set; }
        public int TenantId { get; set; }
    }
}
