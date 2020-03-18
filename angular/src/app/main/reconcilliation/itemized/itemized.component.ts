import { Component, Injector, ViewChild } from '@angular/core';
import { Router } from '@angular/router';
import { AppComponentBase } from '@shared/common/app-component-base';
import * as _ from 'lodash';
import {  ItemizationServiceProxy } from '@shared/service-proxies/service-proxies';
import { UserInformation } from '../../CommonFunctions/UserInformation';
import { LazyLoadEvent } from 'primeng/api';
import { Paginator } from 'primeng/paginator';
import { Table } from 'primeng/table';
import { AppConsts } from '@shared/AppConsts';
@Component({
  selector: 'app-itemized',
  templateUrl: './itemized.component.html',
  styleUrls: ['./itemized.component.css']
})
export class ItemizedComponent extends AppComponentBase {
  UserProfilePicture: any;
  monthValue: Date = new Date();
  commantBox: boolean;
  accountId: any;
  ItemizedList: any = []
  accountName ;
  accountNo
  primeNgEvent : any;
  @ViewChild('dataTable', { static: true }) dataTable: Table;
  @ViewChild('paginator', { static: true }) paginator: Paginator;
  advancedFiltersAreShown = false;
  filterText = '';
  collapsibleRowId : number;
  collapsibleRow : boolean = false;
  userName : any;
  TotalAmount : any;
  accuredAmount :any;
  netAmount : any;
  itemList : any = [];
  monthFilter = '100/2000';
  remainingAttachmentList: any = []
  lock : boolean = false;
  trialBalance : any;
  variance:any;
  


  constructor(
    injector: Injector,
    private _router: Router,
    private userInfo: UserInformation,
    private _itemizedService:ItemizationServiceProxy

  ) {
    super(injector);
  }
  ngOnInit() {
    this.accountId = history.state.data.accountId
    this.accountName = history.state.data.accountName
    this.accountNo = history.state.data.accountNo
    this.commantBox = true;
    this.lock = history.state.data.lock;
    this.getProfilePicture();
    this.userName = this.appSession.user.name.toString();


  }
  RedirectToAddNew() : void {
    this._router.navigate(['/app/main/reconcilliation/itemized/create-edit-itemized'],{ state: { data: { accountId : this.accountId ,accountName :this.accountName ,accountNo: this.accountNo,ItemizedItemId : 0 }} });
  }
  RedirectToDetail(ItemizedItemId) : void {
    if (this.lock == false)
    {
      this._router.navigate(['/app/main/reconcilliation/itemized/itemized-details'],{ state: { data: { accountId : this.accountId ,accountName :this.accountName ,accountNo: this.accountNo,ItemizedItemId : ItemizedItemId }} });

    }
    else {
      this.notify.error("Reconciliation Type is change. So accounts is Lock");
    }

  }
  getAllAmortizedList(event?: LazyLoadEvent){
    this.primeNgEvent = event;
    if (this.primengTableHelper.shouldResetPaging(event)) {
      this.paginator.changePage(0);
      return;
    }

  this.primengTableHelper.showLoadingIndicator();
  this._itemizedService.getAll(
    this.filterText,
    this.accountId,
    this.primengTableHelper.getSorting(this.dataTable),
    this.primengTableHelper.getSkipCount(this.paginator, event),
    this.primengTableHelper.getMaxResultCount(this.paginator, event)
  ).subscribe(result => {
    this.primengTableHelper.totalRecordsCount = result.totalCount;
    this.primengTableHelper.records = result.items[0].itemizedListForViewDto;
    this.primengTableHelper.hideLoadingIndicator();
    this.ItemizedList = result.items;
      this.TotalAmount = this.ItemizedList[0].totalAmount;
      this.trialBalance = this.ItemizedList[0].totalTrialBalance;
      this.variance = this.ItemizedList[0].variance;
      this.itemList = this.ItemizedList[0].itemizedListForViewDto;
      this.itemList.forEach(i => {
        debugger
        i.attachments.forEach((element,index) => {
            var attachmentName = element.attachmentPath.substring(element.attachmentPath.lastIndexOf("/") + 1, element.attachmentPath.lastIndexOf("zinlo"));
            element["attachmentExtension"] = this.getExtensionImagePath(element.attachmentPath)
            element["attachmentName"] = attachmentName
            element["attachmentUrl"] = AppConsts.remoteServiceBaseUrl+"/"+ element.attachmentPath
              
        });
      });  
  });
}

filterByMonth(event): void {
  var month = event.getMonth() + 1;
  this.monthFilter = month + "/" + event.getFullYear()
  this.getAllAmortizedList(this.primeNgEvent);
}

getExtensionImagePath(str) {

  var extension = str.split('.')[1];
  extension = extension + ".svg";
  return extension;
}

collapsibleRowClick(id) {
  this.collapsibleRowId = id;
  this.collapsibleRow = !this.collapsibleRow;
} 

BackToReconcileList() {
  this._router.navigate(['/app/main/reconcilliation']);

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