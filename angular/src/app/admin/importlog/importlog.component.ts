import { Component, Injector, ViewChild, Output, EventEmitter } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { Table } from 'primeng/components/table/table';
import { Paginator } from 'primeng/components/paginator/paginator';
import { LazyLoadEvent } from 'primeng/components/common/lazyloadevent';
import * as _ from 'lodash';
import { ImportLogServiceProxy } from '@shared/service-proxies/service-proxies';
import { AppConsts } from '@shared/AppConsts';
import { appModuleAnimation } from '@shared/animations/routerTransition';

@Component({
  selector: 'app-importlog',
  templateUrl: './importlog.component.html',
  styleUrls: ['./importlog.component.css'],
  animations: [appModuleAnimation()]
})
export class ImportLogComponent extends AppComponentBase {

    @ViewChild('dataTable', { static: true }) dataTable: Table;
    @ViewChild('paginator', { static: true }) paginator: Paginator;
    @Output() recordid = new EventEmitter<number>();
    advancedFiltersAreShown = false;
    filterText = '';
    fileUrl;
    exceptionsList: any;
   
    constructor(
        injector: Injector,
        private _importlogServiceProxy: ImportLogServiceProxy) {
        super(injector);
    }
    ngOnInit() {
    }

    getAllImportLog(event?: LazyLoadEvent) {
        if (this.primengTableHelper.shouldResetPaging(event)) {
            this.paginator.changePage(0);
            return;
        }
        this.primengTableHelper.showLoadingIndicator();
        this._importlogServiceProxy.getAll(
            this.filterText,
            this.primengTableHelper.getSorting(this.dataTable),
            this.primengTableHelper.getSkipCount(this.paginator, event),
            this.primengTableHelper.getMaxResultCount(this.paginator, event)
        ).subscribe(result => {
            this.primengTableHelper.totalRecordsCount = result.totalCount;
            this.primengTableHelper.records = result.items;
            this.primengTableHelper.hideLoadingIndicator();
            this.exceptionsList = result.items;
            this.exceptionsList.array.forEach(element => {
              element.filePath = AppConsts.remoteServiceBaseUrl + element.filePath;
            });
            this.primengTableHelper.records = [];
            this.primengTableHelper.records = this.exceptionsList;
        });
    }
    downloadFile(path){
        window.open(AppConsts.remoteServiceBaseUrl+"/"+path);
    }
    reloadPage(): void {
        this.paginator.changePage(this.paginator.getPage());
    }
    rollBack(id) : void{
        this._importlogServiceProxy.rollBackTrialBalance(id).subscribe(x=>{
           this.getAllImportLog();
        })
        }   
}

