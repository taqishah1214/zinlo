import { Component, Injector, ViewEncapsulation, ViewChild, Output, EventEmitter } from '@angular/core';
import { Router } from '@angular/router';
import { AppComponentBase } from '@shared/common/app-component-base';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { Table } from 'primeng/components/table/table';
import { Paginator } from 'primeng/components/paginator/paginator';
import { LazyLoadEvent } from 'primeng/components/common/lazyloadevent';
import * as _ from 'lodash';

import {AccountSubTypeServiceProxy } from '@shared/service-proxies/service-proxies';


@Component({
  selector: 'app-accountsubtype',
  templateUrl: './accountsubtype.component.html',
  styleUrls: ['./accountsubtype.component.css'],
  animations: [appModuleAnimation()]

})
export class AccountsubtypeComponent extends AppComponentBase {

    @ViewChild('dataTable', { static: true }) dataTable: Table;
    @ViewChild('paginator', { static: true }) paginator: Paginator;

    @Output() recordid = new EventEmitter<number>();


    advancedFiltersAreShown = false;
    filterText = '';
    titleFilter = '';
    descriptionFilter = '';
    categoriesList: any;
    public EditRecordId: number = 0;
    constructor(
        injector: Injector,
        private accountSubTypeServiceProxy: AccountSubTypeServiceProxy,
        private _router: Router
    ) {
        super(injector);

    }
    ngOnInit() {
        
    }

    getCategories(event?: LazyLoadEvent) {
      
        if (this.primengTableHelper.shouldResetPaging(event)) {
            this.paginator.changePage(0);
            return;
        }

        this.primengTableHelper.showLoadingIndicator();

        this.accountSubTypeServiceProxy.getAll(
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
        });
    }
    reloadPage(): void {
        this.paginator.changePage(this.paginator.getPage());
    }

    createCategory(): void {
        this.EditRecordId = 0;
        this._router.navigate(['/app/main/account/accountsubtype/create-or-edit-accountsubtype'], { state: { data: { id: 0 } } });
    }

    editCategory(id: any): void {
        this.recordid = id;
        this._router.navigate(['/app/main/account/accountsubtype/create-or-edit-accountsubtype'], { state: { data: { id: this.recordid } } });
    }

    deleteCategory(id: number): void {
        this.message.confirm(
            '',
            this.l('AreYouSure'),
            (isConfirmed) => {
                if (isConfirmed) {
                    this.accountSubTypeServiceProxy.delete(id)
                        .subscribe(() => {
                            this.reloadPage();
                            this.notify.success(this.l('SuccessfullyDeleted'));
                        });
                }
            }
        );
    }
   
}
