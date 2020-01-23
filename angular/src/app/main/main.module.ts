
import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { AppCommonModule } from '@app/shared/common/app-common.module';
import { CategoriesComponent } from './categories/categories/categories.component';

import { AutoCompleteModule } from 'primeng/autocomplete';
import { PaginatorModule } from 'primeng/paginator';
import { EditorModule } from 'primeng/editor';
import { InputMaskModule } from 'primeng/inputmask'; import { FileUploadModule } from 'primeng/fileupload';
import { TableModule } from 'primeng/table';

import { UtilsModule } from '@shared/utils/utils.module';
import { CountoModule } from 'angular2-counto';
import { ModalModule, TabsModule, TooltipModule, BsDropdownModule, PopoverModule } from 'ngx-bootstrap';
import { DashboardComponent } from './dashboard/dashboard.component';
import { MainRoutingModule } from './main-routing.module';
import { NgxChartsModule } from '@swimlane/ngx-charts';

import { BsDatepickerModule, BsDatepickerConfig, BsDaterangepickerConfig, BsLocaleService } from 'ngx-bootstrap/datepicker';
import { NgxBootstrapDatePickerConfigService } from 'assets/ngx-bootstrap/ngx-bootstrap-datepicker-config.service';;
import { CreateOrEditTaskComponent } from './TasksCheckList/create-or-edit-task/create-or-edit-task.component';
import { TasksComponent } from './TasksCheckList/tasks/tasks.component';
import { CreateOrEditCategoryComponent } from './categories/create-or-edit-category/create-or-edit-category.component'
import { CategorieDropDownComponent } from './categories/categorie-drop-down/categorie-drop-down.component';;
import { UserListComponentComponent } from './TasksCheckList/user-list-component/user-list-component.component'
        ;
import { TaskDetailsComponent } from './TasksCheckList/task-details/task-details.component'

import { IgxCalendarModule } from 'igniteui-angular';
import { UppyAngularModule } from 'uppy-angular';;
import { StatusComponent } from './TasksCheckList/status/status.component';
import { EditTaskComponent } from './TasksCheckList/edit-task/edit-task.component'


NgxBootstrapDatePickerConfigService.registerNgxBootstrapDatePickerLocales();

@NgModule({
        imports: [
                FileUploadModule,
                AutoCompleteModule,
                PaginatorModule,
                EditorModule,
                InputMaskModule, TableModule,

                CommonModule,
                FormsModule,
                ModalModule,
                TabsModule,
                TooltipModule,
                AppCommonModule,
                UtilsModule,
                MainRoutingModule,
                CountoModule,
                NgxChartsModule,
                BsDatepickerModule.forRoot(),
                BsDropdownModule.forRoot(),
                PopoverModule.forRoot(),
                IgxCalendarModule,
                UppyAngularModule
        ],
        declarations: [
                CategoriesComponent,
                CategorieDropDownComponent,
                DashboardComponent,
                CreateOrEditTaskComponent,
                TasksComponent,
                CreateOrEditCategoryComponent,
                UserListComponentComponent,
                TaskDetailsComponent
,
                StatusComponent,
                EditTaskComponent
        ],
        providers: [
                { provide: BsDatepickerConfig, useFactory: NgxBootstrapDatePickerConfigService.getDatepickerConfig },
                { provide: BsDaterangepickerConfig, useFactory: NgxBootstrapDatePickerConfigService.getDaterangepickerConfig },
                { provide: BsLocaleService, useFactory: NgxBootstrapDatePickerConfigService.getDatepickerLocale }
        ]
})
export class MainModule { }