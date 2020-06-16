import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { AppCommonModule } from '@app/shared/common/app-common.module';
import { CategoriesComponent } from './categories/categories.component';

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
import { CreateOrEditCategoryComponent } from './categories/create-or-edit-category/create-or-edit-category.component';
import { CategorieDropDownComponent } from './categories/categorie-drop-down/categorie-drop-down.component';
import { UserListComponentComponent } from './checklist/user-list-component/user-list-component.component';
import { TaskDetailsComponent } from './checklist/task-details/task-details.component'

import { IgxCalendarModule } from 'igniteui-angular';
import { UppyAngularModule } from 'uppy-angular';
import { StatusComponent } from './checklist/status/status.component';
import { EditTaskComponent } from './checklist/edit-task/edit-task.component';
import { Checklist } from './checklist/checklist.component';;
import { CreatetaskComponent } from './checklist/createtask/createtask.component'
import {Formater} from "@shared/customPipe/formatter"
import { CreateOrEditAccountsubtypeComponent } from './account/accountsubtype/create-or-edit-accountsubtype/create-or-edit-accountsubtype.component';
import { AccountsubtypeComponent } from './account/accountsubtype/accountsubtype.component';
import { AccountsComponent } from './account/accounts/accounts.component';
import { CreateEditAccountsComponent } from './account/accounts/create-edit-accounts/create-edit-accounts.component';
import { NgSelectModule } from '@ng-select/ng-select';;
import { ReconcilliationComponent } from './reconcilliation/reconcilliation.component';
import { CreateEditItemizedComponent } from './reconcilliation/itemized/create-edit-itemized/create-edit-itemized.component';
import { CreateEditAmortizedComponent } from './reconcilliation/amortized/create-edit-amortized/create-edit-amortized.component';;
import { ItemizedComponent } from './reconcilliation/itemized/itemized.component'
import { AmortizedComponent } from './reconcilliation/amortized/amortized.component';;
import { ItemizedDetailsComponent } from './reconcilliation/itemized/itemized-details/itemized-details.component';
import { AmortizedDetailsComponent } from './reconcilliation/amortized/amortized-details/amortized-details.component';;
import {VirtualScrollerModule} from 'primeng/virtualscroller';
import { InfiniteScrollModule } from 'ngx-infinite-scroll';
import { SharedModule } from 'primeng/components/common/shared';
import { AppSharedModule } from '@app/app-shared/app-shared.module';
import { TaskReportComponent } from './reports/task-report/task-report.component';;
import { TrialbalanceReportsComponent } from './reports/trialbalance-reports/trialbalance-reports.component'
import { ObserveVarianceComponent } from './reports/observe-variance/observe-variance.component';

import { RichTextEditorAllModule } from '@syncfusion/ej2-angular-richtexteditor';
import { AngularEditorModule } from '@kolkov/angular-editor';
import { NgxDocViewerModule } from 'ngx-doc-viewer';


NgxBootstrapDatePickerConfigService.registerNgxBootstrapDatePickerLocales();

@NgModule({
        imports: [
                NgxDocViewerModule,
                AngularEditorModule,
                RichTextEditorAllModule,
                FileUploadModule,
                AutoCompleteModule,
                PaginatorModule,
                EditorModule,
                InputMaskModule,
                TableModule,
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
                UppyAngularModule,
                NgSelectModule,
                VirtualScrollerModule,
                InfiniteScrollModule,
                SharedModule,
                AppSharedModule
                
        ],
        declarations: [
                CategoriesComponent,
                CategorieDropDownComponent,
                DashboardComponent,
                CreatetaskComponent,
                Checklist,
                CreateOrEditCategoryComponent,
                UserListComponentComponent,
                TaskDetailsComponent,
                StatusComponent,
                EditTaskComponent,
                Formater,             
                CreateOrEditAccountsubtypeComponent,
                AccountsubtypeComponent,
                AccountsComponent ,
                CreateEditAccountsComponent ,
                ReconcilliationComponent,            
                CreateEditItemizedComponent ,
                CreateEditAmortizedComponent ,
                ItemizedComponent,
                AmortizedComponent ,
                ItemizedDetailsComponent ,
                AmortizedDetailsComponent ,
                TaskReportComponent,
                ObserveVarianceComponent
,
                TrialbalanceReportsComponent
        ],
        providers: [
                { provide: BsDatepickerConfig, useFactory: NgxBootstrapDatePickerConfigService.getDatepickerConfig },
                { provide: BsDaterangepickerConfig, useFactory: NgxBootstrapDatePickerConfigService.getDaterangepickerConfig },
                { provide: BsLocaleService, useFactory: NgxBootstrapDatePickerConfigService.getDatepickerLocale }
        ]
})
export class MainModule { }