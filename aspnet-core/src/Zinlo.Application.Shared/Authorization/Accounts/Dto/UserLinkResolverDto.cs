using System;
using System.Collections.Generic;
using System.Text;

namespace Zinlo.Authorization.Accounts.Dto
{
   public class UserLinkResolverDto
    {
        public string Email { get; set; }
        public int? TenantId { get; set; }
    }
}
