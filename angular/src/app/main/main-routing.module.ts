import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { CategoriesComponent } from './categories/categories.component';
import { DashboardComponent } from './dashboard/dashboard.component';
import { TaskDetailsComponent } from './checklist/task-details/task-details.component';
import { CreateOrEditCategoryComponent } from './categories/create-or-edit-category/create-or-edit-category.component';
import { EditTaskComponent } from './checklist/edit-task/edit-task.component';
import { Checklist } from './checklist/checklist.component';
import { CreatetaskComponent } from './checklist/createtask/createtask.component';
import { CreateOrEditAccountsubtypeComponent } from './account/accountsubtype/create-or-edit-accountsubtype/create-or-edit-accountsubtype.component';
import { AccountsubtypeComponent } from './account/accountsubtype/accountsubtype.component';
import { AccountsComponent } from './account/accounts/accounts.component';
import { DuplicateTaskComponent } from './checklist/duplicate-task/duplicate-task.component';
import { CreateEditAccountsComponent } from './account/accounts/create-edit-accounts/create-edit-accounts.component';
import { CreateditAmartizationComponent } from './createdit-amartization/createdit-amartization.component';

@NgModule({
    imports: [
        RouterModule.forChild([
            {
                path: '',
                children: [
                    { path: 'categories/create-or-edit-category', component: CreateOrEditCategoryComponent, data: { permission: 'Pages.Categories.Create', id : 'id' }  },
                    { path: 'checklist/task-details', component: TaskDetailsComponent, data: { permission: 'Pages.ClosingChecklist.Create' }  },
                    { path: 'checklist', component: Checklist, data: { permission: 'Pages.ClosingChecklist' }},
                    { path: 'checklist/edit-task', component: EditTaskComponent, data: { permission: 'Pages.ClosingChecklist' }},
                    { path: 'checklist/createtask', component: CreatetaskComponent, data: { permission: 'Pages.ClosingChecklist.Create' }  },
                    { path: 'categories', component: CategoriesComponent, data: { permission: 'Pages.Categories' }  },
                    { path: 'dashboard', component: DashboardComponent, data: { permission: 'Pages.Tenant.Dashboard' } },
                    { path: '', redirectTo: 'dashboard', pathMatch: 'full' },
                    { path: 'account/accountsubtype/create-or-edit-accountsubtype', component: CreateOrEditAccountsubtypeComponent , data: { permission: '' }  },
                    { path: 'account/accountsubtype', component: AccountsubtypeComponent , data: { permission: '' }  },
                    { path: 'checklist/duplicate-task', component : DuplicateTaskComponent , data: {permission:""}},
                    { path: 'account/accounts', component: AccountsComponent , data: { permission: '' }  },
                    { path: 'account/accounts/create-edit-accounts', component: CreateEditAccountsComponent , data: { permission: '' }  },
                    { path: 'createdit-amartization', component: CreateditAmartizationComponent, data: { permission: 'Pages.ClosingChecklist' }},
                ]
            }
        ])
    ],
    exports: [
        RouterModule
    ]
})
export class MainRoutingModule { }
