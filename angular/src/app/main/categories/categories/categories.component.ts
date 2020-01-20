import { Component, Injector, ViewEncapsulation, ViewChild, Output, EventEmitter } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { CategoriesServiceProxy, CategoryDto, GetCategoryForViewDto  } from '@shared/service-proxies/service-proxies';
import { NotifyService } from '@abp/notify/notify.service';
import { AppComponentBase } from '@shared/common/app-component-base';
import { TokenAuthServiceProxy } from '@shared/service-proxies/service-proxies';
import { CreateOrEditCategoryModalComponent } from './create-or-edit-category-modal.component';
import { ViewCategoryModalComponent } from './view-category-modal.component';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { Table } from 'primeng/components/table/table';
import { Paginator } from 'primeng/components/paginator/paginator';
import { LazyLoadEvent } from 'primeng/components/common/lazyloadevent';
import { FileDownloadService } from '@shared/utils/file-download.service';
import * as _ from 'lodash';
import * as moment from 'moment';
import { CreatOrEditCategoryComponent } from '../creat-or-edit-category/creat-or-edit-category.component';

@Component({
    templateUrl: './categories.component.html',
    encapsulation: ViewEncapsulation.None,
    animations: [appModuleAnimation()]
})
export class CategoriesComponent extends AppComponentBase {

    @ViewChild('viewCategoryModalComponent', { static: true }) viewCategoryModal: ViewCategoryModalComponent;
    @ViewChild('dataTable', { static: true }) dataTable: Table;
    @ViewChild('paginator', { static: true }) paginator: Paginator;

    @Output() recordid = new EventEmitter<number>();

   // RId: EventEmitter<number> = new EventEmitter<number>();

   

    advancedFiltersAreShown = false;
    filterText = '';
    titleFilter = '';
    descriptionFilter = '';
 categoriesList :any;
   public EditRecordId :number = 0;



    constructor(
        injector: Injector,
        private _categoriesServiceProxy: CategoriesServiceProxy,
        private _notifyService: NotifyService,
        private _tokenAuth: TokenAuthServiceProxy,
        private _activatedRoute: ActivatedRoute,
        private _fileDownloadService: FileDownloadService,
        private _router:Router
    ) {
        super(injector);
        
    }
    ngOnInit() {
      //  this.loadGrid();
    }

    getCategories(event?: LazyLoadEvent) {
        debugger;
        if (this.primengTableHelper.shouldResetPaging(event)) {
            this.paginator.changePage(0);
            return;
        }

        this.primengTableHelper.showLoadingIndicator();

        this._categoriesServiceProxy.getAll(
            this.filterText,
            this.titleFilter,
            this.descriptionFilter,
            this.primengTableHelper.getSorting(this.dataTable),
            this.primengTableHelper.getSkipCount(this.paginator, event),
            this.primengTableHelper.getMaxResultCount(this.paginator, event)
        ).subscribe(result => {
            this.primengTableHelper.totalRecordsCount = result.totalCount;
            this.primengTableHelper.records = result.items;
            this.primengTableHelper.hideLoadingIndicator();
            this.categoriesList = result.items;
            console.log(this.categoriesList);
        });
    }
    
    // loadGrid(event?: LazyLoadEvent){
    //     debugger;

    //     if (this.primengTableHelper.shouldResetPaging(event)) {
    //                 this.paginator.changePage(0);
    //                  return;
    //              }
        
    //              this.primengTableHelper.showLoadingIndicator();


    //     this._categoriesServiceProxy.getAll(

    //         this.filterText,
    //                  this.titleFilter,
    //                 this.descriptionFilter,
    //                 this.primengTableHelper.getSorting(this.dataTable),
    //                  this.primengTableHelper.getSkipCount(this.paginator, event),
    //                  this.primengTableHelper.getMaxResultCount(this.paginator, event)

    //     ).subscribe(result=>{

    //         this.primengTableHelper.totalRecordsCount = result.totalCount;
    //         this.primengTableHelper.records = result.items;
    //          this.primengTableHelper.hideLoadingIndicator();

    //         this.categoriesList = result.items;
    //     });
    // }

    reloadPage(): void {
        this.paginator.changePage(this.paginator.getPage());
    }

    // RedirectToCreateCategory() :void {
    //     this._router.navigate(['/app/main/TasksCheckList/create-or-edit-task']);   
    // }

    createCategory(): void {
        this.EditRecordId = 0;
        this._router.navigate(['/app/main/categories/creat-or-edit-category']);
    }

    editCategory(id:any): void {
        debugger;
        this.recordid = id;
        this._router.navigate(['/app/main/categories/creat-or-edit-category',{ id : id}]);
    }

    deleteCategory(category: CategoryDto): void {
        this.message.confirm(
            '',
            this.l('AreYouSure'),
            (isConfirmed) => {
                if (isConfirmed) {
                    this._categoriesServiceProxy.delete(category.id)
                        .subscribe(() => {
                            this.reloadPage();
                            this.notify.success(this.l('SuccessfullyDeleted'));
                        });
                }
            }
        );
    }

    exportToExcel(): void {
        this._categoriesServiceProxy.getCategoriesToExcel(
        this.filterText,
            this.titleFilter,
            this.descriptionFilter,
        )
        .subscribe(result => {
            this._fileDownloadService.downloadTempFile(result);
         });
    }
}
