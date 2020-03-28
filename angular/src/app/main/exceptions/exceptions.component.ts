import { Component, Injector, ViewEncapsulation, ViewChild, Output, EventEmitter } from '@angular/core';
import { Router } from '@angular/router';
import { AppComponentBase } from '@shared/common/app-component-base';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { Table } from 'primeng/components/table/table';
import { Paginator } from 'primeng/components/paginator/paginator';
import { LazyLoadEvent } from 'primeng/components/common/lazyloadevent';
import * as _ from 'lodash';
import { CategoriesServiceProxy, ExceptionLoggerServiceProxy } from '@shared/service-proxies/service-proxies';
import { AppConsts } from '@shared/AppConsts';
@Component({
  selector: 'app-exceptions',
  templateUrl: './exceptions.component.html',
  styleUrls: ['./exceptions.component.css']
})
export class ExceptionsComponent extends AppComponentBase {

    @ViewChild('dataTable', { static: true }) dataTable: Table;
    @ViewChild('paginator', { static: true }) paginator: Paginator;
    @Output() recordid = new EventEmitter<number>();
    advancedFiltersAreShown = false;
    filterText = '';
    exceptionsList: any;
    constructor(
        injector: Injector,
        private _exceptionServiceProxy: ExceptionLoggerServiceProxy,
        private _router: Router
    ) {
        super(injector);
    }
    ngOnInit() {
    }

    getAllExceptions(event?: LazyLoadEvent) {
        if (this.primengTableHelper.shouldResetPaging(event)) {
            this.paginator.changePage(0);
            return;
        }
        this.primengTableHelper.showLoadingIndicator();
        this._exceptionServiceProxy.getAll(
            this.filterText,
            this.primengTableHelper.getSorting(this.dataTable),
            this.primengTableHelper.getSkipCount(this.paginator, event),
            this.primengTableHelper.getMaxResultCount(this.paginator, event)
        ).subscribe(result => {
            this.primengTableHelper.totalRecordsCount = result.totalCount;
            this.primengTableHelper.records = result.items;
            console.log(result.items)
            this.primengTableHelper.hideLoadingIndicator();
            this.exceptionsList = result.items;
            this.exceptionsList.array.forEach(element => {
              element.filePath = AppConsts.remoteServiceBaseUrl + element.filePath;
            });
         
        });
    }
    reloadPage(): void {
        this.paginator.changePage(this.paginator.getPage());
    }   
    RollBack(id) : void{
    this._exceptionServiceProxy.rollBackTrialBalance(id).subscribe(x=>{
       this.getAllExceptions();
    })
    }
}

