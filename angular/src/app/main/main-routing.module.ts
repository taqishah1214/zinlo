import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { CategoriesComponent } from './categories/categories/categories.component';
import { DashboardComponent } from './dashboard/dashboard.component';
import { TasksComponent } from './TasksCheckList/tasks/tasks.component';
import { CreateOrEditTaskComponent } from './TasksCheckList/create-or-edit-task/create-or-edit-task.component';
import { CreatOrEditCategoryComponent } from './categories/creat-or-edit-category/creat-or-edit-category.component';
import { TaskDetailsComponent } from './TasksCheckList/task-details/task-details.component';


@NgModule({
    imports: [
        RouterModule.forChild([
            {
                path: '',
                children: [
                    { path: 'categories/creat-or-edit-category', component: CreatOrEditCategoryComponent, data: { permission: 'Pages.Categories.Create', id : 'id' }  },
                    { path: 'TasksCheckList/task-details', component: TaskDetailsComponent, data: { permission: 'Pages.ClosingChecklist.Create' }  },

                    { path: 'TasksCheckList/tasks', component: TasksComponent, data: { permission: 'Pages.ClosingChecklist' }  },
                    { path: 'TasksCheckList/create-or-edit-task', component: CreateOrEditTaskComponent, data: { permission: 'Pages.ClosingChecklist.Create' }  },
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
