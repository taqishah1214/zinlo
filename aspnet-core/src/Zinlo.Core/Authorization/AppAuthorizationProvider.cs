using Abp.Authorization;
using Abp.Configuration.Startup;
using Abp.Localization;
using Abp.MultiTenancy;

namespace Zinlo.Authorization
{
    /// <summary>
    /// Application's authorization provider.
    /// Defines permissions for the application.
    /// See <see cref="AppPermissions"/> for all permission names.
    /// </summary>
    public class AppAuthorizationProvider : AuthorizationProvider
    {
        private readonly bool _isMultiTenancyEnabled;

        public AppAuthorizationProvider(bool isMultiTenancyEnabled)
        {
            _isMultiTenancyEnabled = isMultiTenancyEnabled;
        }

        public AppAuthorizationProvider(IMultiTenancyConfig multiTenancyConfig)
        {
            _isMultiTenancyEnabled = multiTenancyConfig.IsEnabled;
        }

        public override void SetPermissions(IPermissionDefinitionContext context)
        {
            //COMMON PERMISSIONS (FOR BOTH OF TENANTS AND HOST)

            var pages = context.GetPermissionOrNull(AppPermissions.Pages) ?? context.CreatePermission(AppPermissions.Pages, L("Pages"));

            var closingChecklist = pages.CreateChildPermission(AppPermissions.Pages_Closing_Checklist, L("ClosingChecklist"), multiTenancySides: MultiTenancySides.Tenant);
            closingChecklist.CreateChildPermission(AppPermissions.Pages_Tasks_Create, L("Create"), multiTenancySides: MultiTenancySides.Tenant);
            closingChecklist.CreateChildPermission(AppPermissions.Pages_Tasks_Edit, L("Edit"), multiTenancySides: MultiTenancySides.Tenant);
            closingChecklist.CreateChildPermission(AppPermissions.Pages_Tasks_Delete, L("Delete"), multiTenancySides: MultiTenancySides.Tenant);
            closingChecklist.CreateChildPermission(AppPermissions.Pages_Tasks_Duplicate, L("Duplicate"), multiTenancySides: MultiTenancySides.Tenant);
            closingChecklist.CreateChildPermission(AppPermissions.Pages_Tasks_Change_Status, L("ChangeStatus"), multiTenancySides: MultiTenancySides.Tenant);
            closingChecklist.CreateChildPermission(AppPermissions.Pages_Tasks_Change_Assignee, L("ChangeAssignee"), multiTenancySides: MultiTenancySides.Tenant);
            closingChecklist.CreateChildPermission(AppPermissions.Pages_Tasks_Attachments, L("Attachments"), multiTenancySides: MultiTenancySides.Tenant);
            closingChecklist.CreateChildPermission(AppPermissions.Pages_Tasks_Comment, L("Comment"), multiTenancySides: MultiTenancySides.Tenant);

            var reports = pages.CreateChildPermission(AppPermissions.Pages_Reports, L("Reports"), multiTenancySides: MultiTenancySides.Tenant);
            reports.CreateChildPermission(AppPermissions.Pages_Tasks_Report, L("TaskReports"), multiTenancySides: MultiTenancySides.Tenant);
            reports.CreateChildPermission(AppPermissions.Pages_Observe_Variance_Reports, L("ObserveVariance"), multiTenancySides: MultiTenancySides.Tenant);
            reports.CreateChildPermission(AppPermissions.Pages_Trial_Balance_Report, L("TrialBalance"), multiTenancySides: MultiTenancySides.Tenant);

            var reconcilliations = pages.CreateChildPermission(AppPermissions.Pages_Reconciliation, L("Reconciliation"), multiTenancySides: MultiTenancySides.Tenant);



            var categories = closingChecklist.CreateChildPermission(AppPermissions.Pages_Categories, L("Categories"), multiTenancySides: MultiTenancySides.Tenant);
            categories.CreateChildPermission(AppPermissions.Pages_Categories_Create, L("Create") ,multiTenancySides: MultiTenancySides.Tenant);
            categories.CreateChildPermission(AppPermissions.Pages_Categories_Edit, L("Edit"), multiTenancySides: MultiTenancySides.Tenant);
            categories.CreateChildPermission(AppPermissions.Pages_Categories_Delete, L("Delete"), multiTenancySides: MultiTenancySides.Tenant);


            var chartofAccounts = pages.CreateChildPermission(AppPermissions.Pages_ChartsofAccounts, L("ChartsofAccounts"), multiTenancySides: MultiTenancySides.Tenant);
            chartofAccounts.CreateChildPermission(AppPermissions.Pages_ChartsofAccounts_Create, L("CreateNewChartsofAccounts"), multiTenancySides: MultiTenancySides.Tenant);
            chartofAccounts.CreateChildPermission(AppPermissions.Pages_ChartsofAccounts_Edit, L("EditChartsofAccounts"), multiTenancySides: MultiTenancySides.Tenant);
            chartofAccounts.CreateChildPermission(AppPermissions.Pages_ChartsofAccounts_Delete, L("DeleteChartsofAccounts"), multiTenancySides: MultiTenancySides.Tenant);


            pages.CreateChildPermission(AppPermissions.Pages_DemoUiComponents, L("DemoUiComponents"));

            var administration = pages.CreateChildPermission(AppPermissions.Pages_Administration, L("Administration"));

            var timeManagements = administration.CreateChildPermission(AppPermissions.Pages_Administration_TimeManagements, L("Management"), multiTenancySides: MultiTenancySides.Tenant);
            timeManagements.CreateChildPermission(AppPermissions.Pages_Administration_TimeManagements_Create, L("DefineClosingMonth"), multiTenancySides: MultiTenancySides.Tenant);
            //timeManagements.CreateChildPermission(AppPermissions.Pages_Administration_TimeManagements_Edit, L("EditTimeManagement"), multiTenancySides: MultiTenancySides.Tenant);
            //timeManagements.CreateChildPermission(AppPermissions.Pages_Administration_TimeManagements_Delete, L("DeleteTimeManagement"), multiTenancySides: MultiTenancySides.Tenant);
            timeManagements.CreateChildPermission(AppPermissions.Pages_Administration_TimeManagements_Status, L("ChangeStatus"), multiTenancySides: MultiTenancySides.Tenant);



            var roles = administration.CreateChildPermission(AppPermissions.Pages_Administration_Roles, L("Roles"));
            roles.CreateChildPermission(AppPermissions.Pages_Administration_Roles_Create, L("CreatingNewRole"));
            roles.CreateChildPermission(AppPermissions.Pages_Administration_Roles_Edit, L("EditingRole"));
            roles.CreateChildPermission(AppPermissions.Pages_Administration_Roles_Delete, L("DeletingRole"));

            var users = administration.CreateChildPermission(AppPermissions.Pages_Administration_Users, L("Users"));
            users.CreateChildPermission(AppPermissions.Pages_Administration_Users_Create, L("CreatingNewUser"));
            users.CreateChildPermission(AppPermissions.Pages_Administration_Users_Edit, L("EditingUser"));
            users.CreateChildPermission(AppPermissions.Pages_Administration_Users_Delete, L("DeletingUser"));
            users.CreateChildPermission(AppPermissions.Pages_Administration_Users_ChangePermissions, L("ChangingPermissions"));
            users.CreateChildPermission(AppPermissions.Pages_Administration_Users_Impersonation, L("LoginForUsers"));
            users.CreateChildPermission(AppPermissions.Pages_Administration_Users_Unlock, L("Unlock"));

            var languages = administration.CreateChildPermission(AppPermissions.Pages_Administration_Languages, L("Languages"));
            languages.CreateChildPermission(AppPermissions.Pages_Administration_Languages_Create, L("CreatingNewLanguage"));
            languages.CreateChildPermission(AppPermissions.Pages_Administration_Languages_Edit, L("EditingLanguage"));
            languages.CreateChildPermission(AppPermissions.Pages_Administration_Languages_Delete, L("DeletingLanguages"));
            languages.CreateChildPermission(AppPermissions.Pages_Administration_Languages_ChangeTexts, L("ChangingTexts"));

            administration.CreateChildPermission(AppPermissions.Pages_Administration_AuditLogs, L("AuditLogs"));

            var organizationUnits = administration.CreateChildPermission(AppPermissions.Pages_Administration_OrganizationUnits, L("OrganizationUnits"));
            organizationUnits.CreateChildPermission(AppPermissions.Pages_Administration_OrganizationUnits_ManageOrganizationTree, L("ManagingOrganizationTree"));
            organizationUnits.CreateChildPermission(AppPermissions.Pages_Administration_OrganizationUnits_ManageMembers, L("ManagingMembers"));
            organizationUnits.CreateChildPermission(AppPermissions.Pages_Administration_OrganizationUnits_ManageRoles, L("ManagingRoles"));

            administration.CreateChildPermission(AppPermissions.Pages_Administration_UiCustomization, L("VisualSettings"));

            //TENANT-SPECIFIC PERMISSIONS

            pages.CreateChildPermission(AppPermissions.Pages_Tenant_Dashboard, L("Dashboard"), multiTenancySides: MultiTenancySides.Tenant);

            administration.CreateChildPermission(AppPermissions.Pages_Administration_Tenant_Settings, L("Settings"), multiTenancySides: MultiTenancySides.Tenant);
            administration.CreateChildPermission(AppPermissions.Pages_Administration_Tenant_SubscriptionManagement, L("Subscription"), multiTenancySides: MultiTenancySides.Tenant);

            //HOST-SPECIFIC PERMISSIONS

            var editions = pages.CreateChildPermission(AppPermissions.Pages_Editions, L("Editions"), multiTenancySides: MultiTenancySides.Host);
            editions.CreateChildPermission(AppPermissions.Pages_Editions_Create, L("CreatingNewEdition"), multiTenancySides: MultiTenancySides.Host);
            editions.CreateChildPermission(AppPermissions.Pages_Editions_Edit, L("EditingEdition"), multiTenancySides: MultiTenancySides.Host);
            editions.CreateChildPermission(AppPermissions.Pages_Editions_Delete, L("DeletingEdition"), multiTenancySides: MultiTenancySides.Host);
            editions.CreateChildPermission(AppPermissions.Pages_Editions_MoveTenantsToAnotherEdition, L("MoveTenantsToAnotherEdition"), multiTenancySides: MultiTenancySides.Host); 

            var tenants = pages.CreateChildPermission(AppPermissions.Pages_Tenants, L("Tenants"), multiTenancySides: MultiTenancySides.Host);
            tenants.CreateChildPermission(AppPermissions.Pages_Tenants_Create, L("CreatingNewTenant"), multiTenancySides: MultiTenancySides.Host);
            tenants.CreateChildPermission(AppPermissions.Pages_Tenants_Edit, L("EditingTenant"), multiTenancySides: MultiTenancySides.Host);
            tenants.CreateChildPermission(AppPermissions.Pages_Tenants_ChangeFeatures, L("ChangingFeatures"), multiTenancySides: MultiTenancySides.Host);
            tenants.CreateChildPermission(AppPermissions.Pages_Tenants_Delete, L("DeletingTenant"), multiTenancySides: MultiTenancySides.Host);
            tenants.CreateChildPermission(AppPermissions.Pages_Tenants_Impersonation, L("LoginForTenants"), multiTenancySides: MultiTenancySides.Host);

            administration.CreateChildPermission(AppPermissions.Pages_Administration_Host_Settings, L("Settings"), multiTenancySides: MultiTenancySides.Host);
            administration.CreateChildPermission(AppPermissions.Pages_Administration_Host_Maintenance, L("Maintenance"), multiTenancySides: _isMultiTenancyEnabled ? MultiTenancySides.Host : MultiTenancySides.Tenant);
            administration.CreateChildPermission(AppPermissions.Pages_Administration_HangfireDashboard, L("HangfireDashboard"), multiTenancySides: _isMultiTenancyEnabled ? MultiTenancySides.Host : MultiTenancySides.Tenant);
            administration.CreateChildPermission(AppPermissions.Pages_Administration_Host_Dashboard, L("Dashboard"), multiTenancySides: MultiTenancySides.Host);
        }

        private static ILocalizableString L(string name)
        {
            return new LocalizableString(name, ZinloConsts.LocalizationSourceName);
        }
    }
}
