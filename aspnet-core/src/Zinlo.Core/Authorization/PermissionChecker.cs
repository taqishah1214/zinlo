using Abp.Authorization;
using Zinlo.Authorization.Roles;
using Zinlo.Authorization.Users;

namespace Zinlo.Authorization
{
    public class PermissionChecker : PermissionChecker<Role, User>
    {
        public PermissionChecker(UserManager userManager)
            : base(userManager)
        {

        }
    }
}
