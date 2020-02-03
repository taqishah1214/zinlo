import { Component, Injector, ViewEncapsulation, ViewChild } from '@angular/core';
import { TimeManagementsServiceProxy, TimeManagementDto  } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import { CreateOrEditTimeManagementModalComponent } from './create-or-edit-timeManagement-modal.component';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { Table } from 'primeng/components/table/table';
import { Paginator } from 'primeng/components/paginator/paginator';
import { LazyLoadEvent } from 'primeng/components/common/lazyloadevent';
import * as _ from 'lodash';

@Component({
    templateUrl: './management.component.html',
    encapsulation: ViewEncapsulation.None,
    animations: [appModuleAnimation()],
    styleUrls: ['./management.component.css'],
})
export class ManagementComponent extends AppComponentBase {

    @ViewChild('createOrEditTimeManagementModal', { static: true }) createOrEditTimeManagementModal: CreateOrEditTimeManagementModalComponent;
    @ViewChild('dataTable', { static: true }) dataTable: Table;
    @ViewChild('paginator', { static: true }) paginator: Paginator;

    filterText = '';




    constructor(
        injector: Injector,
        private _timeManagementsServiceProxy: TimeManagementsServiceProxy    ) {
        super(injector);
    }

    getTimeManagements(event?: LazyLoadEvent) {
        if (this.primengTableHelper.shouldResetPaging(event)) {
            this.paginator.changePage(0);
            return;
        }

        this.primengTableHelper.showLoadingIndicator();

        this._timeManagementsServiceProxy.getAll(
            this.filterText,
            this.primengTableHelper.getSorting(this.dataTable),
            this.primengTableHelper.getSkipCount(this.paginator, event),
            this.primengTableHelper.getMaxResultCount(this.paginator, event)
        ).subscribe(result => {
            this.primengTableHelper.totalRecordsCount = result.totalCount;
            this.primengTableHelper.records = result.items;
            this.primengTableHelper.hideLoadingIndicator();
        });
    }

    reloadPage(): void {
        this.paginator.changePage(this.paginator.getPage());
    }

    createTimeManagement(): void {
        this.createOrEditTimeManagementModal.show();
    }

    changeStatus(id: number){
        this._timeManagementsServiceProxy.changeStatus(id).subscribe(() => {
            this.reloadPage();
            this.notify.info(this.l('SavedSuccessfully'));
        });
    }

    deleteTimeManagement(timeManagement: TimeManagementDto): void {
        this.message.confirm(
            '',
            this.l('AreYouSure'),
            (isConfirmed) => {
                if (isConfirmed) {
                    this._timeManagementsServiceProxy.delete(timeManagement.id)
                        .subscribe(() => {
                            this.reloadPage();
                            this.notify.success(this.l('SuccessfullyDeleted'));
                        });
                }
            }
        );
    }
}
