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
import { CreateEditAccountsComponent } from './account/accounts/create-edit-accounts/create-edit-accounts.component';
import { ReconcilliationComponent } from './reconcilliation/reconcilliation.component';
import { CreateEditItemizedComponent } from './reconcilliation/itemized/create-edit-itemized/create-edit-itemized.component';
import { CreateEditAmortizedComponent } from './reconcilliation/amortized/create-edit-amortized/create-edit-amortized.component';
import { ItemizedComponent } from './reconcilliation/itemized/itemized.component';
import { AmortizedComponent } from './reconcilliation/amortized/amortized.component';
import { ItemizedDetailsComponent } from './reconcilliation/itemized/itemized-details/itemized-details.component';
import { AmortizedDetailsComponent } from './reconcilliation/amortized/amortized-details/amortized-details.component';
import { TaskReportComponent } from './reports/task-report/task-report.component';
import { TrialbalanceReportsComponent } from './reports/trialbalance-reports/trialbalance-reports.component';
import { ObserveVarianceComponent } from './reports/observe-variance/observe-variance.component';
import { ImportLogComponent } from './account/importlog/importlog.component';




@NgModule({
    imports: [
        RouterModule.forChild([
            {
                path: '',
                children: [
                    { path: 'categories/create-or-edit-category', component: CreateOrEditCategoryComponent, data: { permission: 'Pages.Categories.Create', id : 'id' }  },
                    { path: 'checklist/task-details', component: TaskDetailsComponent, data: { permission: '' }  },
                    { path: 'checklist', component: Checklist, data: { permission: 'Pages.ClosingChecklist' }},
                    { path: 'checklist/edit-task', component: EditTaskComponent, data: { permission: 'Pages.Tasks.Edit' }},
                    { path: 'checklist/createtask', component: CreatetaskComponent, data: { permission: 'Pages.Tasks.Create' }  },
                    { path: 'categories', component: CategoriesComponent, data: { permission: 'Pages.Categories' }  },
                    { path: 'dashboard', component: DashboardComponent, data: { permission: 'Pages.Tenant.Dashboard' } },
                    { path: '', redirectTo: 'checklist', pathMatch: 'full' },
                    { path: 'sub/create-or-edit-accountsubtype', component: CreateOrEditAccountsubtypeComponent , data: { permission: 'Pages.AccountSubType.Create' }  },
                    { path: 'sub', component: AccountsubtypeComponent , data: { permission: 'Pages.AccountSubType' }  },
                    { path: 'account', component: AccountsComponent , data: { permission: 'Pages.ChartsofAccounts' }  },
                    { path: 'account/create-edit-accounts', component: CreateEditAccountsComponent , data: { permission: 'Pages.ChartsofAccounts.Create' }  },
                    { path: 'importlog', component: ImportLogComponent , data: { permission: 'Pages.ChartsofAccounts.Upload' }  },
                    { path: 'reconciliation', component: ReconcilliationComponent , data: { permission: 'Pages.Reconciliation' }  },
                    { path: 'reconciliation/itemized/create-edit-itemized', component: CreateEditItemizedComponent, data: { permission: 'Pages.Reconciliation.Create' }},
                    { path: 'reconciliation/amortized/create-edit-amortized', component: CreateEditAmortizedComponent, data: { permission: 'Pages.Reconciliation.Create' }},
                    { path: 'reconciliation/itemized', component: ItemizedComponent, data: { permission: 'Pages.Reconciliation' }},
                    { path: 'reconciliation/amortized', component: AmortizedComponent, data: { permission: 'Pages.Reconciliation' }},
                    { path: 'reconciliation/itemized/itemized-details', component: ItemizedDetailsComponent, data: { permission: 'Pages.Reconciliation' }},
                    { path: 'reconciliation/amortized/amortized-details', component: AmortizedDetailsComponent, data: { permission: 'Pages.Reconciliation' }},
                    { path: 'task-report', component: TaskReportComponent, data: { permission: 'Pages.Task.Reports' }},
                    { path: 'trialbalance-report', component: TrialbalanceReportsComponent , data: { permission: 'Pages.Trial.Reports' }},
                    { path: 'observe-variance', component: ObserveVarianceComponent , data: { permission: 'Pages.Observe.Variance.Reports' }},

                ]
            }
        ])
    ],
    exports: [
        RouterModule
    ]
})
export class MainRoutingModule { }