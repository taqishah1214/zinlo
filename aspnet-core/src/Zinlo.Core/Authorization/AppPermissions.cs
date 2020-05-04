namespace Zinlo.Authorization
{
    /// <summary>
    /// Defines string constants for application's permission names.
    /// <see cref="AppAuthorizationProvider"/> for permission definitions.
    /// </summary>
    public static class AppPermissions
    {
        public const string Pages_Administration_TimeManagements = "Pages.Administration.TimeManagements";
        public const string Pages_Administration_TimeManagements_Create = "Pages.Administration.TimeManagements.Create";
        public const string Pages_Administration_TimeManagements_Edit = "Pages.Administration.TimeManagements.Edit";
        public const string Pages_Administration_TimeManagements_Delete = "Pages.Administration.TimeManagements.Delete";
        public const string Pages_Administration_TimeManagements_Status = "Pages.Administration.TimeManagements.Status";

        public const string Pages_ChartsofAccounts = "Pages.ChartsofAccounts";
        public const string Pages_ChartsofAccounts_Create = "Pages.ChartsofAccounts.Create";
        public const string Pages_ChartsofAccounts_Edit = "Pages.ChartsofAccounts.Edit";
        public const string Pages_ChartsofAccounts_Delete = "Pages.ChartsofAccounts.Delete";

        public const string Pages_Reconciliation = "Pages.Reconciliation";


        public const string Pages_Tasks = "Pages.Tasks";
        public const string Pages_Tasks_Create = "Pages.Tasks.Create";
        public const string Pages_Tasks_Edit = "Pages.Tasks.Edit";
        public const string Pages_Tasks_Delete = "Pages.Tasks.Delete";
        public const string Pages_Tasks_Duplicate = "Pages.Tasks.Duplicate";
        public const string Pages_Tasks_Check_Report = "Pages.Tasks.Check.Reports";
        public const string Pages_Tasks_Comment = "Pages.Tasks.Comment";
        public const string Pages_Tasks_Change_Status = "Pages.Tasks.Change.Status";
        public const string Pages_Tasks_Change_Assignee = "Pages.Tasks.Change.Assignee";
        public const string Pages_Tasks_Attachments = "Pages.Tasks.Attachments";


        public const string Pages_Categories = "Pages.Categories";
        public const string Pages_Categories_Create = "Pages.Categories.Create";
        public const string Pages_Categories_Edit = "Pages.Categories.Edit";
        public const string Pages_Categories_Delete = "Pages.Categories.Delete";

        //COMMON PERMISSIONS (FOR BOTH OF TENANTS AND HOST)

        public const string Pages = "Pages";

        public const string Pages_DemoUiComponents= "Pages.DemoUiComponents";
        public const string Pages_Administration = "Pages.Administration";

        public const string Pages_Administration_Roles = "Pages.Administration.Roles";
        public const string Pages_Administration_Roles_Create = "Pages.Administration.Roles.Create";
        public const string Pages_Administration_Roles_Edit = "Pages.Administration.Roles.Edit";
        public const string Pages_Administration_Roles_Delete = "Pages.Administration.Roles.Delete";

        public const string Pages_Administration_Users = "Pages.Administration.Users";
        public const string Pages_Administration_Users_Create = "Pages.Administration.Users.Create";
        public const string Pages_Administration_Users_Edit = "Pages.Administration.Users.Edit";
        public const string Pages_Administration_Users_Delete = "Pages.Administration.Users.Delete";
        public const string Pages_Administration_Users_ChangePermissions = "Pages.Administration.Users.ChangePermissions";
        public const string Pages_Administration_Users_Impersonation = "Pages.Administration.Users.Impersonation";
        public const string Pages_Administration_Users_Unlock = "Pages.Administration.Users.Unlock";

        public const string Pages_Administration_Languages = "Pages.Administration.Languages";
        public const string Pages_Administration_Languages_Create = "Pages.Administration.Languages.Create";
        public const string Pages_Administration_Languages_Edit = "Pages.Administration.Languages.Edit";
        public const string Pages_Administration_Languages_Delete = "Pages.Administration.Languages.Delete";
        public const string Pages_Administration_Languages_ChangeTexts = "Pages.Administration.Languages.ChangeTexts";

        public const string Pages_Administration_AuditLogs = "Pages.Administration.AuditLogs";

        public const string Pages_Administration_OrganizationUnits = "Pages.Administration.OrganizationUnits";
        public const string Pages_Administration_OrganizationUnits_ManageOrganizationTree = "Pages.Administration.OrganizationUnits.ManageOrganizationTree";
        public const string Pages_Administration_OrganizationUnits_ManageMembers = "Pages.Administration.OrganizationUnits.ManageMembers";
        public const string Pages_Administration_OrganizationUnits_ManageRoles = "Pages.Administration.OrganizationUnits.ManageRoles";

        public const string Pages_Administration_HangfireDashboard = "Pages.Administration.HangfireDashboard";

        public const string Pages_Administration_UiCustomization = "Pages.Administration.UiCustomization";

        //TENANT-SPECIFIC PERMISSIONS

        public const string Pages_Tenant_Dashboard = "Pages.Tenant.Dashboard";

        public const string Pages_Administration_Tenant_Settings = "Pages.Administration.Tenant.Settings";

        public const string Pages_Administration_Tenant_SubscriptionManagement = "Pages.Administration.Tenant.SubscriptionManagement";

        //HOST-SPECIFIC PERMISSIONS

        public const string Pages_Editions = "Pages.Editions";
        public const string Pages_Editions_Create = "Pages.Editions.Create";
        public const string Pages_Editions_Edit = "Pages.Editions.Edit";
        public const string Pages_Editions_Delete = "Pages.Editions.Delete";
        public const string Pages_Editions_MoveTenantsToAnotherEdition = "Pages.Editions.MoveTenantsToAnotherEdition";

        public const string Pages_Tenants = "Pages.Tenants";
        public const string Pages_Tenants_Create = "Pages.Tenants.Create";
        public const string Pages_Tenants_Edit = "Pages.Tenants.Edit";
        public const string Pages_Tenants_ChangeFeatures = "Pages.Tenants.ChangeFeatures";
        public const string Pages_Tenants_Delete = "Pages.Tenants.Delete";
        public const string Pages_Tenants_Impersonation = "Pages.Tenants.Impersonation";

        public const string Pages_Custom_Tenants = "Pages.Tenants.Custom";
        public const string Pages_Custom_Tenants_Create = "Pages.Tenants.Custom.Create";
        public const string Pages_Custom_Tenants_Edit = "Pages.Tenants.Custom.Edit";
        public const string Pages_Custom_Tenants_ChangeFeatures = "Pages.Tenants.Custom.ChangeFeatures";
        public const string Pages_Custom_Tenants_Delete = "Pages.Tenants.Custom.Delete";
        public const string Pages_Custom_Tenants_Impersonation = "Pages.Tenants.Custom.Impersonation";




        public const string Pages_Administration_Host_Maintenance = "Pages.Administration.Host.Maintenance";
        public const string Pages_Administration_Host_Settings = "Pages.Administration.Host.Settings";
        public const string Pages_Administration_Host_Dashboard = "Pages.Administration.Host.Dashboard";

    }
}