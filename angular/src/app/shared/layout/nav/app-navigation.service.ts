import { PermissionCheckerService } from '@abp/auth/permission-checker.service';
import { AppSessionService } from '@shared/common/session/app-session.service';

import { Injectable } from '@angular/core';
import { AppMenu } from './app-menu';
import { AppMenuItem } from './app-menu-item';

@Injectable()
export class AppNavigationService {

    constructor(
        private _permissionCheckerService: PermissionCheckerService,
        private _appSessionService: AppSessionService
    ) {

    }

    getMenu(): AppMenu {
        return new AppMenu('MainMenu', 'MainMenu', [
            
          //  new AppMenuItem('Dashboard', 'Pages.Administration.Host.Dashboard', 'flaticon-line-graph', '/app/admin/hostDashboard'),
           // new AppMenuItem('Dashboard', 'Pages.Tenant.Dashboard', 'flaticon-line-graph', '/app/main/dashboard'),
            //new AppMenuItem('Tenants', 'Pages.Tenants', 'flaticon-list-3', '/app/admin/tenants'),
            new AppMenuItem('Tenants', '', './assets/media/ClosingCheckList/clipboard.svg', '',
            [
                new AppMenuItem('Paid Plan', 'Pages.Tenants', './assets/media/ClosingCheckList/clipboard.svg', '/app/admin/tenants'),
                new AppMenuItem('Custom Plan', 'Pages.Tenants', './assets/media/ClosingCheckList/administration.svg', '/app/admin/custom-tenants'),
            ]),
            new AppMenuItem('Editions', 'Pages.Editions', './assets/media/ClosingCheckList/clipboard.svg', '/app/admin/editions'),


            
            
            new AppMenuItem('Closing Checklist', 'Pages.ClosingChecklist', './assets/media/ClosingCheckList/clipboard.svg', '', [
                new AppMenuItem('Checklist', 'Pages.ClosingChecklist','./assets/media/ClosingCheckList/dot.png','/app/main/checklist'),
                new AppMenuItem('Task Categories', 'Pages.Categories', './assets/media/ClosingCheckList/dot.png', '/app/main/categories'),
            ]),
            
            new AppMenuItem('Chart of Accounts', 'Pages.ChartsofAccounts', './assets/media/ClosingCheckList/chartofaccount.svg', '', [
                new AppMenuItem('Chart of Accounts', 'Pages.ChartsofAccounts','./assets/media/ClosingCheckList/dot.png','/app/main/account'),
                new AppMenuItem('Account Sub Type', 'Pages.AccountSubType', './assets/media/ClosingCheckList/dot.png', '/app/main/sub'),
                
            ]),

            new AppMenuItem('Reconciliation', 'Pages.Reconciliation', './assets/media/ClosingCheckList/reconcilliation.svg', '/app/main/reconcilliation', [ 
            ]),
            new AppMenuItem('Reports', 'Pages.Reports', './assets/media/ClosingCheckList/MaskGroup30.svg', '', [
                new AppMenuItem('TaskReport', 'Pages.Task.Reports','./assets/media/ClosingCheckList/dot.png','/app/main/task-report'),
                new AppMenuItem('TrialBalanceReport', 'Pages.Trial.Reports','./assets/media/ClosingCheckList/dot.png','/app/main/trialbalance-report'),
                new AppMenuItem('ObserveVariance', 'Pages.Observe.Variance.Reports','./assets/media/ClosingCheckList/dot.png','/app/main/observe-variance'),

            ]),
           // new AppMenuItem('Tests', 'Pages.Tests', 'flaticon-more', '/app/main/tests/tests'),
             new AppMenuItem('Administration', '', './assets/media/ClosingCheckList/administration.svg', '', [
                //new AppMenuItem('OrganizationUnits', 'Pages.Administration.OrganizationUnits', 'flaticon-map', '/app/admin/organization-units'),
                new AppMenuItem('Roles', 'Pages.Administration.Roles', './assets/media/ClosingCheckList/dot.png', '/app/admin/roles'),
                new AppMenuItem('Users', 'Pages.Administration.Users', './assets/media/ClosingCheckList/dot.png', '/app/admin/users'),
            new AppMenuItem('Management', 'Pages.Administration.TimeManagements', './assets/media/ClosingCheckList/dot.png', '/app/admin/management'),
            new AppMenuItem('ImportLog', 'Pages.ImportLog', './assets/media/ClosingCheckList/dot.png', '/app/admin/importlog', [ ]),
                //new AppMenuItem('Languages', 'Pages.Administration.Languages', 'flaticon-tabs', '/app/admin/languages'),
                new AppMenuItem('AuditLogs', 'Pages.Administration.AuditLogs', './assets/media/ClosingCheckList/dot.png', '/app/admin/auditLogs'),
                //new AppMenuItem('Maintenance', 'Pages.Administration.Host.Maintenance', 'flaticon-lock', '/app/admin/maintenance'),
                new AppMenuItem('Subscription', 'Pages.Administration.Tenant.SubscriptionManagement', './assets/media/ClosingCheckList/dot.png', '/app/admin/subscription-management'),
                //new AppMenuItem('VisualSettings', 'Pages.Administration.UiCustomization', 'flaticon-medical', '/app/admin/ui-customization'),
                new AppMenuItem('Settings', 'Pages.Administration.Host.Settings', './assets/media/ClosingCheckList/dot.png', '/app/admin/hostSettings'),
                new AppMenuItem('Settings', 'Pages.Administration.Tenant.Settings', './assets/media/ClosingCheckList/dot.png', '/app/admin/tenantSettings'),
               ///new AppMenuItem('Users', 'Pages.Administration.Tenant.Settings', 'flaticon-settings', '/app/main/users')

            ]),
            //new AppMenuItem('DemoUiComponents', 'Pages.DemoUiComponents', 'flaticon-shapes', '/app/admin/demo-ui-components')
        ]);
    }

    checkChildMenuItemPermission(menuItem): boolean {

        for (let i = 0; i < menuItem.items.length; i++) {
            let subMenuItem = menuItem.items[i];

            if (subMenuItem.permissionName === '' || subMenuItem.permissionName === null || subMenuItem.permissionName && this._permissionCheckerService.isGranted(subMenuItem.permissionName)) {
                return true;
            } else if (subMenuItem.items && subMenuItem.items.length) {
                return this.checkChildMenuItemPermission(subMenuItem);
            }
        }

        return false;
    }

    showMenuItem(menuItem: AppMenuItem): boolean {
        if (menuItem.permissionName === 'Pages.Administration.Tenant.SubscriptionManagement' && this._appSessionService.tenant && !this._appSessionService.tenant.edition) {
            return false;
        }

        let hideMenuItem = false;

        if (menuItem.requiresAuthentication && !this._appSessionService.user) {
            hideMenuItem = true;
        }

        if (menuItem.permissionName && !this._permissionCheckerService.isGranted(menuItem.permissionName)) {
            hideMenuItem = true;
        }

        if (this._appSessionService.tenant || !abp.multiTenancy.ignoreFeatureCheckForHostUsers) {
            if (menuItem.hasFeatureDependency() && !menuItem.featureDependencySatisfied()) {
                hideMenuItem = true;
            }
        }

        if (!hideMenuItem && menuItem.items && menuItem.items.length) {
            return this.checkChildMenuItemPermission(menuItem);
        }

        return !hideMenuItem;
    }

    /**
     * Returns all menu items recursively
     */
    getAllMenuItems(): AppMenuItem[] {
        let menu = this.getMenu();
        let allMenuItems: AppMenuItem[] = [];
        menu.items.forEach(menuItem => {
            allMenuItems = allMenuItems.concat(this.getAllMenuItemsRecursive(menuItem));
        });

        return allMenuItems;
    }

    private getAllMenuItemsRecursive(menuItem: AppMenuItem): AppMenuItem[] {
        if (!menuItem.items) {
            return [menuItem];
        }

        let menuItems = [menuItem];
        menuItem.items.forEach(subMenu => {
            menuItems = menuItems.concat(this.getAllMenuItemsRecursive(subMenu));
        });

        return menuItems;
    }
}
