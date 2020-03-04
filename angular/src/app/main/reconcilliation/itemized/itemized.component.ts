import { Component, Injector } from '@angular/core';
import { Router } from '@angular/router';
import { AppComponentBase } from '@shared/common/app-component-base';
import * as _ from 'lodash';
import { CategoriesServiceProxy } from '@shared/service-proxies/service-proxies';
import { UserInformation } from '../../CommonFunctions/UserInformation';
import { LazyLoadEvent } from 'primeng/api';
import { Paginator } from 'primeng/paginator';
import { Table } from 'primeng/table';
@Component({
  selector: 'app-itemized',
  templateUrl: './itemized.component.html',
  styleUrls: ['./itemized.component.css']
})
export class ItemizedComponent extends AppComponentBase {
UserProfilePicture: any;
monthValue: Date = new Date();
commantBox: boolean;
constructor(
injector: Injector,
private _categoriesServiceProxy: CategoriesServiceProxy,
private _router: Router,
private userInfo: UserInformation

) {
super(injector);
}
ngOnInit() {
this.commantBox = true;
this.getProfilePicture();

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

}