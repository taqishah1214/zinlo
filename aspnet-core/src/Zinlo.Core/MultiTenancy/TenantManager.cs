using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using Abp;
using Abp.Application.Features;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.IdentityFramework;
using Abp.MultiTenancy;
using Zinlo.Authorization.Roles;
using Zinlo.Authorization.Users;
using Zinlo.Editions;
using Zinlo.MultiTenancy.Demo;
using Abp.Extensions;
using Abp.Notifications;
using Abp.Runtime.Security;
using Microsoft.AspNetCore.Identity;
using Zinlo.Notifications;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Abp.Authorization;
using Abp.Runtime.Session;
using Abp.UI;
using Zinlo.Authorization;
using Zinlo.MultiTenancy.Payments;

namespace Zinlo.MultiTenancy
{
    /// <summary>
    /// Tenant manager.
    /// </summary>
    public class TenantManager : AbpTenantManager<Tenant, User>
    {
        public IAbpSession AbpSession { get; set; }

        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly RoleManager _roleManager;
        private readonly UserManager _userManager;
        private readonly IUserEmailer _userEmailer;
        private readonly TenantDemoDataBuilder _demoDataBuilder;
        private readonly INotificationSubscriptionManager _notificationSubscriptionManager;
        private readonly IAppNotifier _appNotifier;
        private readonly IAbpZeroDbMigrator _abpZeroDbMigrator;
        private readonly IPermissionManager _permissionManager;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly IRepository<SubscribableEdition> _subscribableEditionRepository;

        public TenantManager(
            IRepository<Tenant> tenantRepository,
            IRepository<TenantFeatureSetting, long> tenantFeatureRepository,
            EditionManager editionManager,
            IUnitOfWorkManager unitOfWorkManager,
            RoleManager roleManager,
            IUserEmailer userEmailer,
            TenantDemoDataBuilder demoDataBuilder,
            UserManager userManager,
            INotificationSubscriptionManager notificationSubscriptionManager,
            IAppNotifier appNotifier,
            IAbpZeroFeatureValueStore featureValueStore,
            IAbpZeroDbMigrator abpZeroDbMigrator,
            IPasswordHasher<User> passwordHasher,
            IRepository<SubscribableEdition> subscribableEditionRepository, IPermissionManager permissionManager) : base(
                tenantRepository,
                tenantFeatureRepository,
                editionManager,
                featureValueStore
            )
        {
            AbpSession = NullAbpSession.Instance;

            _unitOfWorkManager = unitOfWorkManager;
            _roleManager = roleManager;
            _userEmailer = userEmailer;
            _demoDataBuilder = demoDataBuilder;
            _userManager = userManager;
            _notificationSubscriptionManager = notificationSubscriptionManager;
            _appNotifier = appNotifier;
            _abpZeroDbMigrator = abpZeroDbMigrator;
            _passwordHasher = passwordHasher;
            _subscribableEditionRepository = subscribableEditionRepository;
            _permissionManager = permissionManager;
        }

        public async Task<int> CreateWithAdminUserAsync(
            string tenancyName,
            string name,
            string adminPassword,
            string adminEmailAddress,
            string connectionString,
            bool isActive,
            int? editionId,
            bool shouldChangePasswordOnNextLogin,
            bool sendActivationEmail,
            DateTime? subscriptionEndDate,
            bool isInTrialPeriod,
            string emailActivationLink)
        {
            int newTenantId;
            long newAdminId;
            List<string> permissions = new List<string>();
            await CheckEditionAsync(editionId, isInTrialPeriod);

            using (var uow = _unitOfWorkManager.Begin(TransactionScopeOption.RequiresNew))
            {
                //Create tenant
                var tenant = new Tenant(tenancyName, name)
                {
                    IsActive = isActive,
                    EditionId = editionId,
                    SubscriptionEndDateUtc = subscriptionEndDate?.ToUniversalTime(),
                    IsInTrialPeriod = isInTrialPeriod,
                    ConnectionString = connectionString.IsNullOrWhiteSpace() ? null : SimpleStringCipher.Instance.Encrypt(connectionString)
                };

                await CreateAsync(tenant);
                await _unitOfWorkManager.Current.SaveChangesAsync(); //To get new tenant's id.

                //Create tenant database
                _abpZeroDbMigrator.CreateOrMigrateForTenant(tenant);

                //We are working entities of new tenant, so changing tenant filter
                using (_unitOfWorkManager.Current.SetTenantId(tenant.Id))
                {
                    //Create static roles for new tenant
                    CheckErrors(await _roleManager.CreateStaticRoles(tenant.Id));
                    await _unitOfWorkManager.Current.SaveChangesAsync(); //To get static role ids

                    //grant all permissions to admin role
                    var adminRole = _roleManager.Roles.Single(r => r.Name == StaticRoleNames.Tenants.Admin);
                    await _roleManager.GrantAllPermissionsAsync(adminRole);

                    //grant all permissions to primary admin role
                    var primaryAdminRole = _roleManager.Roles.Single(r => r.Name == StaticRoleNames.Tenants.PrimaryAdmin);
                    await _roleManager.GrantAllPermissionsAsync(primaryAdminRole);

                    //grant all permissions to manager role
                    var managerRole = _roleManager.Roles.Single(r => r.Name == StaticRoleNames.Tenants.Manager);
                    
                    permissions.Add(AppPermissions.Pages_Closing_Checklist);
                    permissions.Add(AppPermissions.Pages_Tasks_Create);
                    permissions.Add(AppPermissions.Pages_Tasks_Edit);
                    permissions.Add(AppPermissions.Pages_Tasks_Delete);
                    permissions.Add(AppPermissions.Pages_Tasks_Duplicate);
                    permissions.Add(AppPermissions.Pages_Tasks_Change_Assignee);
                    permissions.Add(AppPermissions.Pages_Tasks_Change_Status);
                    permissions.Add(AppPermissions.Pages_Tasks_Comment);
                    permissions.Add(AppPermissions.Pages_Tasks_Attachments);

                    ///////////////////////////////////////////////
                    permissions.Add(AppPermissions.Pages_Reports);
                    permissions.Add(AppPermissions.Pages_Tasks_Report);
                    permissions.Add(AppPermissions.Pages_Trial_Balance_Report);
                    permissions.Add(AppPermissions.Pages_Observe_Variance_Reports);

                    ///////////////////
                    permissions.Add(AppPermissions.Pages_Reconciliation);
                    permissions.Add(AppPermissions.Pages_Reconciliation_Create);
                    permissions.Add(AppPermissions.Pages_Reconciliation_Edit);
                    permissions.Add(AppPermissions.Pages_Reconciliation_Delete);
                    permissions.Add(AppPermissions.Pages_Reconciliation_Change_Status);
                    permissions.Add(AppPermissions.Pages_Reconciliation_Change_Assignee);
                    permissions.Add(AppPermissions.Pages_Reconciliation_Comment);
                    permissions.Add(AppPermissions.Pages_Reconciliation_Attachments);
                    //////////////////

                    permissions.Add(AppPermissions.Pages_ChartsofAccounts);
                    permissions.Add(AppPermissions.Pages_ChartsofAccounts_Create);
                    permissions.Add(AppPermissions.Pages_ChartsofAccounts_Edit);
                    permissions.Add(AppPermissions.Pages_ChartsofAccounts_Delete);
                    permissions.Add(AppPermissions.Pages_ChartsofAccounts_Upload);
                    permissions.Add(AppPermissions.Pages_ChartsofAccounts_Upload_TrialBalance);
                    permissions.Add(AppPermissions.Pages_ChartsofAccounts_Download);
                    permissions.Add(AppPermissions.Pages_ChartsofAccounts_Download_TrialBalance);
                    ///////////////////////////////////////////////////////////////
                    permissions.Add(AppPermissions.Pages_AccountSubType);
                    permissions.Add(AppPermissions.Pages_AccountSubType_Create);
                    permissions.Add(AppPermissions.Pages_AccountSubType_Edit);
                    permissions.Add(AppPermissions.Pages_AccountSubType_Delete);

                  

                    foreach (var permission in permissions)
                    {
                        await _roleManager.GrantPermissionAsync(managerRole, _permissionManager.GetPermission(permission));
                    }
                    permissions.Clear();

                    //User role should be default
                    var userRole = _roleManager.Roles.Single(r => r.Name == StaticRoleNames.Tenants.User);
                    permissions.Add(AppPermissions.Pages_Closing_Checklist);
                    permissions.Add(AppPermissions.Pages_Tasks_Create);
                    permissions.Add(AppPermissions.Pages_Tasks_Edit);
                    permissions.Add(AppPermissions.Pages_Tasks_Change_Status);
                    permissions.Add(AppPermissions.Pages_Tasks_Comment);
                    permissions.Add(AppPermissions.Pages_Tasks_Attachments);


                    permissions.Add(AppPermissions.Pages_Reconciliation);
                    permissions.Add(AppPermissions.Pages_Reconciliation_Create);
                    permissions.Add(AppPermissions.Pages_Reconciliation_Edit);
                    permissions.Add(AppPermissions.Pages_Reconciliation_Delete);
                    permissions.Add(AppPermissions.Pages_Reconciliation_Change_Status);
                    permissions.Add(AppPermissions.Pages_Reconciliation_Comment);

                    userRole.IsDefault = true;
                    foreach (var permission in permissions)
                    {
                        await _roleManager.GrantPermissionAsync(userRole, _permissionManager.GetPermission(permission));
                    }
                    CheckErrors(await _roleManager.UpdateAsync(userRole));

                    //Create admin user for the tenant
                    var adminUser = User.CreateTenantAdminUser(tenant.Id, adminEmailAddress);
                    adminUser.ShouldChangePasswordOnNextLogin = shouldChangePasswordOnNextLogin;
                    adminUser.IsActive = true;

                    if (adminPassword.IsNullOrEmpty())
                    {
                        adminPassword = await _userManager.CreateRandomPassword();
                    }
                    else
                    {
                        await _userManager.InitializeOptionsAsync(AbpSession.TenantId);
                        foreach (var validator in _userManager.PasswordValidators)
                        {
                            CheckErrors(await validator.ValidateAsync(_userManager, adminUser, adminPassword));
                        }

                    }

                    adminUser.Password = _passwordHasher.HashPassword(adminUser, adminPassword);

                    CheckErrors(await _userManager.CreateAsync(adminUser));
                    await _unitOfWorkManager.Current.SaveChangesAsync(); //To get admin user's id

                    //Assign admin user to admin role!
                    CheckErrors(await _userManager.AddToRoleAsync(adminUser, adminRole.Name));

                    //Notifications
                    await _appNotifier.WelcomeToTheApplicationAsync(adminUser);

                    //Send activation email
                    if (sendActivationEmail)
                    {
                        adminUser.SetNewEmailConfirmationCode();
                        await _userEmailer.SendEmailActivationLinkAsync(adminUser, emailActivationLink, adminPassword);
                    }

                    await _unitOfWorkManager.Current.SaveChangesAsync();

                    await _demoDataBuilder.BuildForAsync(tenant);

                    newTenantId = tenant.Id;
                    newAdminId = adminUser.Id;
                }

                await uow.CompleteAsync();
            }

            //Used a second UOW since UOW above sets some permissions and _notificationSubscriptionManager.SubscribeToAllAvailableNotificationsAsync needs these permissions to be saved.
            using (var uow = _unitOfWorkManager.Begin(TransactionScopeOption.RequiresNew))
            {
                using (_unitOfWorkManager.Current.SetTenantId(newTenantId))
                {
                    await _notificationSubscriptionManager.SubscribeToAllAvailableNotificationsAsync(new UserIdentifier(newTenantId, newAdminId));
                    await _unitOfWorkManager.Current.SaveChangesAsync();
                    await uow.CompleteAsync();
                }
            }

            return newTenantId;
        }

        public async Task CheckEditionAsync(int? editionId, bool isInTrialPeriod)
        {
            if (!editionId.HasValue || !isInTrialPeriod)
            {
                return;
            }

            var edition = await _subscribableEditionRepository.GetAsync(editionId.Value);
            if (!edition.IsFree)
            {
                return;
            }

            var error = LocalizationManager.GetSource(ZinloConsts.LocalizationSourceName).GetString("FreeEditionsCannotHaveTrialVersions");
            throw new UserFriendlyException(error);
        }

        protected virtual void CheckErrors(IdentityResult identityResult)
        {
            identityResult.CheckErrors(LocalizationManager);
        }

        public decimal GetUpgradePrice(SubscribableEdition currentEdition, SubscribableEdition targetEdition, int totalRemainingHourCount, PaymentPeriodType paymentPeriodType)
        {
            int numberOfHoursPerDay = 24;

            var totalRemainingDayCount = totalRemainingHourCount / numberOfHoursPerDay;
            var unusedPeriodCount = totalRemainingDayCount / (int)paymentPeriodType;
            var unusedHoursCount = totalRemainingHourCount % ((int)paymentPeriodType * numberOfHoursPerDay);

            decimal currentEditionPriceForUnusedPeriod = 0;
            decimal targetEditionPriceForUnusedPeriod = 0;

            var currentEditionPrice = currentEdition.GetPaymentAmount(paymentPeriodType);
            var targetEditionPrice = targetEdition.GetPaymentAmount(paymentPeriodType);

            if (currentEditionPrice > 0)
            {
                currentEditionPriceForUnusedPeriod = currentEditionPrice * unusedPeriodCount;
                currentEditionPriceForUnusedPeriod += (currentEditionPrice / (int)paymentPeriodType) / numberOfHoursPerDay * unusedHoursCount;
            }

            if (targetEditionPrice > 0)
            {
                targetEditionPriceForUnusedPeriod = targetEditionPrice * unusedPeriodCount;
                targetEditionPriceForUnusedPeriod += (targetEditionPrice / (int)paymentPeriodType) / numberOfHoursPerDay * unusedHoursCount;
            }

            return targetEditionPriceForUnusedPeriod - currentEditionPriceForUnusedPeriod;
        }

        public async Task<Tenant> UpdateTenantAsync(int tenantId, bool isActive, bool? isInTrialPeriod, PaymentPeriodType? paymentPeriodType, int editionId, EditionPaymentType editionPaymentType)
        {
            var tenant = await FindByIdAsync(tenantId);

            tenant.IsActive = isActive;

            if (isInTrialPeriod.HasValue)
            {
                tenant.IsInTrialPeriod = isInTrialPeriod.Value;
            }

            tenant.EditionId = editionId;

            if (paymentPeriodType.HasValue)
            {
                tenant.UpdateSubscriptionDateForPayment(paymentPeriodType.Value, editionPaymentType);
            }

            return tenant;
        }

        public async Task<EndSubscriptionResult> EndSubscriptionAsync(Tenant tenant, SubscribableEdition edition, DateTime nowUtc)
        {
            if (tenant.EditionId == null || tenant.HasUnlimitedTimeSubscription())
            {
                throw new Exception($"Can not end tenant {tenant.TenancyName} subscription for {edition.DisplayName} tenant has unlimited time subscription!");
            }

            Debug.Assert(tenant.SubscriptionEndDateUtc != null, "tenant.SubscriptionEndDateUtc != null");

            var subscriptionEndDateUtc = tenant.SubscriptionEndDateUtc.Value;
            if (!tenant.IsInTrialPeriod)
            {
                subscriptionEndDateUtc = tenant.SubscriptionEndDateUtc.Value.AddDays(edition.WaitingDayAfterExpire ?? 0);
            }

            if (subscriptionEndDateUtc >= nowUtc)
            {
                throw new Exception($"Can not end tenant {tenant.TenancyName} subscription for {edition.DisplayName} since subscription has not expired yet!");
            }

            if (!tenant.IsInTrialPeriod && edition.ExpiringEditionId.HasValue)
            {
                tenant.EditionId = edition.ExpiringEditionId.Value;
                tenant.SubscriptionEndDateUtc = null;

                await UpdateAsync(tenant);

                return EndSubscriptionResult.AssignedToAnotherEdition;
            }

            tenant.IsActive = false;
            tenant.IsInTrialPeriod = false;

            await UpdateAsync(tenant);

            return EndSubscriptionResult.TenantSetInActive;
        }
    }
}
