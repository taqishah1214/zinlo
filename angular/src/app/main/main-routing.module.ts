import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { CategoriesComponent } from './categories/categories/categories.component';
import { DashboardComponent } from './dashboard/dashboard.component';
import { TasksComponent } from './TasksCheckList/tasks/tasks.component';
import { CreateOrEditTaskComponent } from './TasksCheckList/create-or-edit-task/create-or-edit-task.component';

@NgModule({
    imports: [
        RouterModule.forChild([
            {
                path: '',
                children: [
                    { path: 'TasksCheckList/tasks', component: TasksComponent, data: { permission: '' }  },
                    { path: 'TasksCheckList/create-or-edit-task', component: CreateOrEditTaskComponent, data: { permission: '' }  },
                    { path: 'categories/categories', component: CategoriesComponent, data: { permission: 'Pages.Categories' }  },
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
