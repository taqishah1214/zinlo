import { Component, Injector, ViewChild } from '@angular/core';
import { Router } from '@angular/router';
import { AppComponentBase } from '@shared/common/app-component-base';
import * as _ from 'lodash';
import { CategoriesServiceProxy, ItemizationServiceProxy } from '@shared/service-proxies/service-proxies';
import { UserInformation } from '../../CommonFunctions/UserInformation';
import { Paginator } from 'primeng/paginator';
import { Table } from 'primeng/table';
import { LazyLoadEvent } from 'primeng/api';
@Component({
  selector: 'app-itemized',
  templateUrl: './itemized.component.html',
  styleUrls: ['./itemized.component.css']
})
export class ItemizedComponent extends AppComponentBase {
  filterText:any;
  items:any;
UserProfilePicture: any;
monthValue: Date = new Date();
commantBox: boolean;
@ViewChild('dataTable', { static: true }) dataTable: Table;
@ViewChild('paginator', { static: true }) paginator: Paginator;
constructor(
injector: Injector,
private _itemizationServiceProxy : ItemizationServiceProxy,
private _router: Router,
private userInfo: UserInformation

) {
super(injector);
}
ngOnInit() {
this.commantBox = true;
this.getProfilePicture();

}

getAllItems(event?: LazyLoadEvent) {
  if (this.primengTableHelper.shouldResetPaging(event)) {
      this.paginator.changePage(0);
      return;
  }
  this.primengTableHelper.showLoadingIndicator();
  this._itemizationServiceProxy.getAll(
      this.filterText,
      this.primengTableHelper.getSorting(this.dataTable),
      this.primengTableHelper.getSkipCount(this.paginator, event),
      this.primengTableHelper.getMaxResultCount(this.paginator, event)
  ).subscribe(result => {
      this.primengTableHelper.totalRecordsCount = result.totalCount;
      this.primengTableHelper.records = result.items;
      this.primengTableHelper.hideLoadingIndicator();
      this.items = result.items;
      console.log(this.items);
  });
}


onOpenCalendar(container) {
container.monthSelectHandler = (event: any): void => {
container._store.dispatch(container._actions.select(event.date));
};
container.setViewMode('month');
}
getProfilePicture() {
this.userInfo.getProfilePicture();
this.userInfo.profilePicture.subscribe(
data => {
this.UserProfilePicture = data.valueOf();
});
if (this.UserProfilePicture == undefined) {
this.UserProfilePicture = "";
}
}


commentClick(): void {
  this.commantBox = false;
  }
  
  onComment(): void {
  this.commantBox = true;
  }
  onCancelComment(): void {
  this.commantBox = true;
  }
  RedirectToAddNew(){
    this._router.navigate(['/app/main/reconcilliation/itemized/create-edit-itemized']);
  }
  

}

