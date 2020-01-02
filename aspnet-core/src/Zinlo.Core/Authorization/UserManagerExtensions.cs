using System.Threading.Tasks;
using Abp.Authorization.Users;
using Zinlo.Authorization.Users;

namespace Zinlo.Authorization
{
    public static class UserManagerExtensions
    {
        public static async Task<User> GetAdminAsync(this UserManager userManager)
        {
            return await userManager.FindByNameAsync(AbpUserBase.AdminUserName);
        }
    }
}
