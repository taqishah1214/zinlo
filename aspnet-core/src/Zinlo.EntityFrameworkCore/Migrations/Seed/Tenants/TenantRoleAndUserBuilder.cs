using System;
using System.Linq;
using System.Text;
using Abp;
using Abp.Authorization;
using Abp.Authorization.Roles;
using Abp.Authorization.Users;
using Abp.MultiTenancy;
using Abp.Notifications;
using Bogus;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Zinlo.Authorization;
using Zinlo.Authorization.Roles;
using Zinlo.Authorization.Users;
using Zinlo.EntityFrameworkCore;
using Zinlo.Notifications;

namespace Zinlo.Migrations.Seed.Tenants
{
    public class TenantRoleAndUserBuilder
    {
        private readonly ZinloDbContext _context;
        private readonly int _tenantId;
        private readonly string _tenantName;
        private readonly string _tenantPrimaryAdmin;

        public TenantRoleAndUserBuilder(ZinloDbContext context, int tenantId)
        {
            _context = context;
            _tenantId = tenantId;
        }

        public TenantRoleAndUserBuilder(ZinloDbContext context, int tenantId,string tenantName,string tenantPrimaryAdmin)
        {
            _context = context;
            _tenantId = tenantId;
            _tenantName = tenantName;
            _tenantPrimaryAdmin = tenantPrimaryAdmin;
        }


        public void Create()
        {
            CreateRolesAndUsers();
        }

        public void CreateSeederAdmins ()
        {
            GenerateAdminRolesUser();
        }

        public string RandomName(int size, bool lowerCase)
        {
            StringBuilder builder = new StringBuilder();
            Random random = new Random();
            char ch;
            for (int i = 0; i < size; i++)
            {
                ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65)));
                builder.Append(ch);
            }
            if (lowerCase)
                return builder.ToString().ToLower();
            return builder.ToString();
        }



        private void GenerateAdminRolesUser ()
        {

            //Admin role

            var adminRole = _context.Roles.IgnoreQueryFilters().FirstOrDefault(r => r.TenantId == _tenantId && r.Name == StaticRoleNames.Tenants.Admin);
            if (adminRole == null)
            {
                adminRole = _context.Roles.Add(new Role(_tenantId, StaticRoleNames.Tenants.Admin, StaticRoleNames.Tenants.Admin) { IsStatic = true }).Entity;
                _context.SaveChanges();
            }

            //User role

            var userRole = _context.Roles.IgnoreQueryFilters().FirstOrDefault(r => r.TenantId == _tenantId && r.Name == StaticRoleNames.Tenants.User);
            if (userRole == null)
            {
                userRole = _context.Roles.Add(new Role(_tenantId, StaticRoleNames.Tenants.User, StaticRoleNames.Tenants.User) { IsStatic = true }).Entity;
                _context.SaveChanges();
            }

            //admin user

            var adminUser = _context.Users.IgnoreQueryFilters().FirstOrDefault(u => u.TenantId == _tenantId && u.UserName == AbpUserBase.AdminUserName);
            if (adminUser == null)
            {
                adminUser = User.CreateTenantAdminUser(_tenantId, _tenantPrimaryAdmin);
                adminUser.Password = new PasswordHasher<User>(new OptionsWrapper<PasswordHasherOptions>(new PasswordHasherOptions())).HashPassword(adminUser, "123qwe");
                adminUser.IsEmailConfirmed = true;
                adminUser.ShouldChangePasswordOnNextLogin = false;
                adminUser.IsActive = true;

                _context.Users.Add(adminUser);
                _context.SaveChanges();

                //Assign Admin role to admin user
                _context.UserRoles.Add(new UserRole(_tenantId, adminUser.Id, adminRole.Id));
                _context.SaveChanges();

                if (_tenantName.Equals(null))
                {
                   // Do Nothing
                }
                else
                {
                    for (int i = 0; i <= 2; i++)
                    {                   
                    var user = new User();
                    user.Name = RandomName(5,false);
                    user.EmailAddress = RandomName(5, false) + "@gmail.com";
                    user.NormalizedEmailAddress = RandomName(5, false) + "@gmail.com";
                    user.NormalizedUserName = RandomName(5, false);
                    user.Surname = RandomName(5, false);
                    user.UserName = RandomName(5, false);
                    user.TenantId = _tenantId;
                    user.Password = new PasswordHasher<User>(new OptionsWrapper<PasswordHasherOptions>(new PasswordHasherOptions())).HashPassword(adminUser, "123qwe");
                    _context.Users.Add(user);
                    _context.SaveChanges();

                    _context.UserRoles.Add(new UserRole(_tenantId, user.Id, userRole.Id));
                    _context.SaveChanges();
      
                    }     
                }
               
                //Notification subscription
                _context.NotificationSubscriptions.Add(new NotificationSubscriptionInfo(SequentialGuidGenerator.Instance.Create(), _tenantId, adminUser.Id, AppNotificationNames.NewUserRegistered));
                _context.SaveChanges();
            }

        }


        private void CreateRolesAndUsers()
        {
            //Admin role

            var adminRole = _context.Roles.IgnoreQueryFilters().FirstOrDefault(r => r.TenantId == _tenantId && r.Name == StaticRoleNames.Tenants.Admin);
            if (adminRole == null)
            {
                adminRole = _context.Roles.Add(new Role(_tenantId, StaticRoleNames.Tenants.Admin, StaticRoleNames.Tenants.Admin) { IsStatic = true }).Entity;
                _context.SaveChanges();
            }

            //User role

            var userRole = _context.Roles.IgnoreQueryFilters().FirstOrDefault(r => r.TenantId == _tenantId && r.Name == StaticRoleNames.Tenants.User);
            if (userRole == null)
            {
                _context.Roles.Add(new Role(_tenantId, StaticRoleNames.Tenants.User, StaticRoleNames.Tenants.User) { IsStatic = true, IsDefault = true });
                _context.SaveChanges();
            }

            //admin user

            var adminUser = _context.Users.IgnoreQueryFilters().FirstOrDefault(u => u.TenantId == _tenantId && u.UserName == AbpUserBase.AdminUserName);
            if (adminUser == null)
            {
                adminUser = User.CreateTenantAdminUser(_tenantId, "admin@defaulttenant.com");
                adminUser.Password = new PasswordHasher<User>(new OptionsWrapper<PasswordHasherOptions>(new PasswordHasherOptions())).HashPassword(adminUser, "123qwe");
                adminUser.IsEmailConfirmed = true;
                adminUser.ShouldChangePasswordOnNextLogin = false;
                adminUser.IsActive = true;

                _context.Users.Add(adminUser);
                _context.SaveChanges();

                //Assign Admin role to admin user
                _context.UserRoles.Add(new UserRole(_tenantId, adminUser.Id, adminRole.Id));
                _context.SaveChanges();

                //User account of admin user
                if (_tenantId == 1)
                {
                    _context.UserAccounts.Add(new UserAccount
                    {
                        TenantId = _tenantId,
                        UserId = adminUser.Id,
                        UserName = AbpUserBase.AdminUserName,
                        EmailAddress = adminUser.EmailAddress
                    });
                    _context.SaveChanges();
                }

                //Notification subscription
                _context.NotificationSubscriptions.Add(new NotificationSubscriptionInfo(SequentialGuidGenerator.Instance.Create(), _tenantId, adminUser.Id, AppNotificationNames.NewUserRegistered));
                _context.SaveChanges();
            }
        }
    }
}
