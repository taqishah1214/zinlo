import { Component, Injector, ViewEncapsulation, ViewChild, Output, EventEmitter } from '@angular/core';
import { Router } from '@angular/router';
import { AppComponentBase } from '@shared/common/app-component-base';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { Table } from 'primeng/components/table/table';
import { Paginator } from 'primeng/components/paginator/paginator';
import { LazyLoadEvent } from 'primeng/components/common/lazyloadevent';
import * as _ from 'lodash';
import { CategoriesServiceProxy, TimeManagementsServiceProxy } from '@shared/service-proxies/service-proxies';
import { StoreDateService } from "../../services/storedate.service";
import * as moment from 'moment';

@Component({
    templateUrl: './categories.component.html',
    styleUrls: ['./categories.component.css'],
    encapsulation: ViewEncapsulation.None,
    animations: [appModuleAnimation()]
})
export class CategoriesComponent extends AppComponentBase {

    @ViewChild('dataTable', { static: true }) dataTable: Table;
    @ViewChild('paginator', { static: true }) paginator: Paginator;
    @Output() recordid = new EventEmitter<number>();
    advancedFiltersAreShown = false;
    filterText = '';
    titleFilter = '';
    descriptionFilter = '';
    categoriesList: any;
    actionButtonPermission
    users: any;
    monthStatus;
    currentMonth = new Date ();

    public EditRecordId: number = 0;
    constructor(
        injector: Injector,
        private _categoriesServiceProxy: CategoriesServiceProxy,
        private _router: Router,
        private userDate: StoreDateService,
        private _timeManagementsServiceProxy: TimeManagementsServiceProxy
    ) {
        super(injector);
    }
    ngOnInit() {
        this.userDate.allUsersInformationofTenant.subscribe(userList => this.users = userList)
        this.actionButtonPermission = this.isGranted('Pages.Categories.Edit');
        this._timeManagementsServiceProxy.getMonthStatus(moment(this.currentMonth)).subscribe(resp =>  {
            this.monthStatus = resp
        })

    }

    getCategories(event?: LazyLoadEvent) {
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
            this.categoriesList.forEach(i => {
                   i["createdBy"] =  this.users[this.getUserIndex(i.userId)].name;
                   i["profilePicture"] =  this.users[this.getUserIndex(i.userId)].profilePicture;
            });
        });
    }
    reloadPage(): void {
        this.paginator.changePage(this.paginator.getPage());
    }
    createCategory(): void {
        this.EditRecordId = 0;
        this._router.navigate(['/app/main/categories/create-or-edit-category'], { state: { data: { id: 0 } } });
    }
    getUserIndex(id) {
        return this.users.findIndex(x => x.id === id);
      }
    editCategory(id: any): void {
        this.recordid = id;
        this._router.navigate(['/app/main/categories/create-or-edit-category'], { state: { data: { id: this.recordid } } });
    }
    deleteCategory(id: number): void {
        this.message.confirm(
            '',
            this.l('AreYouSure'),
            (isConfirmed) => {
                if (isConfirmed) {
                    this._categoriesServiceProxy.delete(id)
                        .subscribe(() => {
                            this.reloadPage();
                            this.notify.success(this.l('SuccessfullyDeleted'));
                        });
                }
            }
        );
    }
}
