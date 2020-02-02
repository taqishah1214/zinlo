﻿import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { CategoriesComponent } from './categories/categories.component';
import { DashboardComponent } from './dashboard/dashboard.component';
import { CreateOrEditTaskComponent } from './TasksCheckList/create-or-edit-task/create-or-edit-task.component';
import { TaskDetailsComponent } from './TasksCheckList/task-details/task-details.component';
import { CreateOrEditCategoryComponent } from './categories/create-or-edit-category/create-or-edit-category.component';
import { EditTaskComponent } from './TasksCheckList/edit-task/edit-task.component';
import { TasksComponent } from './TasksCheckList/tasks.component';

@NgModule({
    imports: [
        RouterModule.forChild([
            {
                path: '',
                children: [
                    { path: 'categories/create-or-edit-category', component: CreateOrEditCategoryComponent, data: { permission: 'Pages.Categories.Create', id : 'id' }  },
                    { path: 'TasksCheckList/task-details', component: TaskDetailsComponent, data: { permission: 'Pages.ClosingChecklist.Create' }  },
                    { path: 'TasksCheckList', component: TasksComponent, data: { permission: 'Pages.ClosingChecklist' }},
                    { path: 'TasksCheckList/edit-task', component: EditTaskComponent, data: { permission: 'Pages.ClosingChecklist' }},
                    { path: 'TasksCheckList/create-or-edit-task', component: CreateOrEditTaskComponent, data: { permission: 'Pages.ClosingChecklist.Create' }  },
                    { path: 'categories', component: CategoriesComponent, data: { permission: 'Pages.Categories' }  },
                    { path: 'dashboard', component: DashboardComponent, data: { permission: 'Pages.Tenant.Dashboard' } },
                    { path: '', redirectTo: 'dashboard', pathMatch: 'full' }
                ]
            }
        ])
    ],
    exports: [
        RouterModule
    ]
})
export class MainRoutingModule { }
