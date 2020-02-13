import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { CategoriesComponent } from './categories/categories.component';
import { DashboardComponent } from './dashboard/dashboard.component';
import { TaskDetailsComponent } from './checklist/task-details/task-details.component';
import { CreateOrEditCategoryComponent } from './categories/create-or-edit-category/create-or-edit-category.component';
import { EditTaskComponent } from './checklist/edit-task/edit-task.component';
import { Checklist } from './checklist/checklist.component';
import { CreatetaskComponent } from './checklist/createtask/createtask.component';
import { DuplicateTaskComponent } from './duplicate-task/duplicate-task.component';


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
                    { path: 'duplicate-task', component : DuplicateTaskComponent , data: {permission:""}}
                ]
            }
        ])
    ],
    exports: [
        RouterModule
    ]
})
export class MainRoutingModule { }
