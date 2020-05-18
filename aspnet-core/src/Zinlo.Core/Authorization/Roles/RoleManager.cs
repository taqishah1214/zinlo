using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.Authorization;
using Abp.Authorization.Roles;
using Abp.Authorization.Users;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Localization;
using Abp.Organizations;
using Abp.Runtime.Caching;
using Abp.UI;
using Abp.Zero.Configuration;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Zinlo.Authorization.Users;

namespace Zinlo.Authorization.Roles
{
    /// <summary>
    /// Role manager.
    /// Used to implement domain logic for roles.
    /// </summary>
    public class RoleManager : AbpRoleManager<Role, User>
    {
        private readonly ILocalizationManager _localizationManager;
        private readonly IRepository<UserRole, long> _userRoleRepository;

        public RoleManager(
            RoleStore store,
            IEnumerable<IRoleValidator<Role>> roleValidators,
            ILookupNormalizer keyNormalizer,
            IdentityErrorDescriber errors,
            ILogger<RoleManager> logger,
            IPermissionManager permissionManager,
            IRoleManagementConfig roleManagementConfig,
            ICacheManager cacheManager,
            IUnitOfWorkManager unitOfWorkManager,
            ILocalizationManager localizationManager,
            IRepository<OrganizationUnit, long> organizationUnitRepository,
            IRepository<OrganizationUnitRole, long> organizationUnitRoleRepository, IRepository<UserRole, long> userRoleRepository)
            : base(
                store,
                roleValidators,
                keyNormalizer,
                errors,
                logger,
                permissionManager,
                cacheManager,
                unitOfWorkManager,
                roleManagementConfig,
                organizationUnitRepository,
                organizationUnitRoleRepository)
        {
            _localizationManager = localizationManager;
            _userRoleRepository = userRoleRepository;
        }

        public override Task SetGrantedPermissionsAsync(Role role, IEnumerable<Permission> permissions)
        {
            CheckPermissionsToUpdate(role, permissions);

            return base.SetGrantedPermissionsAsync(role, permissions);
        }

        public virtual async Task<Role> GetRoleByIdAsync(long userId)
        {
            var role = await FindByIdAsync(userId.ToString());
            if (role == null)
            {
                throw new ApplicationException("There is no user with id: " + userId);
            }

            return role;
        }

        public virtual async Task<string> GetRoleNameByUserId(long userId)
        {
            var userRole = await _userRoleRepository.FirstOrDefaultAsync(p=>p.UserId == userId);
            var role =  await Roles.Where(p => p.Id == userRole.RoleId).FirstOrDefaultAsync();
            return role.Name;
        }
        private void CheckPermissionsToUpdate(Role role, IEnumerable<Permission> permissions)
        {
            if (role.Name == StaticRoleNames.Host.Admin &&
                (!permissions.Any(p => p.Name == AppPermissions.Pages_Administration_Roles_Edit) ||
                 !permissions.Any(p => p.Name == AppPermissions.Pages_Administration_Users_ChangePermissions)))
            {
                throw new UserFriendlyException(L("YouCannotRemoveUserRolePermissionsFromAdminRole"));
            }
        }

        private new string L(string name)
        {
            return _localizationManager.GetString(ZinloConsts.LocalizationSourceName, name);
        }
    }
}