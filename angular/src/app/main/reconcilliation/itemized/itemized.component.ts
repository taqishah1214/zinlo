import { Component, Injector, ViewChild } from '@angular/core';
import { Router } from '@angular/router';
import { AppComponentBase } from '@shared/common/app-component-base';
import * as _ from 'lodash';
import {  ItemizationServiceProxy,TimeManagementsServiceProxy, CreateOrEditTimeManagementDto } from '@shared/service-proxies/service-proxies';
import { UserInformation } from '../../CommonFunctions/UserInformation';
import { LazyLoadEvent } from 'primeng/api';
import { Paginator } from 'primeng/paginator';
import { Table } from 'primeng/table';
import { AppConsts } from '@shared/AppConsts';
import * as moment from 'moment';
import { add, subtract } from 'add-subtract-date';

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
  monthFilter = new Date();
  remainingAttachmentList: any = []
  trialBalance : any;
  variance:any;
  AccountNumber : any  = "";
  CreateTimeManagementDto : CreateOrEditTimeManagementDto = new CreateOrEditTimeManagementDto()
  comment : any = ""
  postedCommentList : any =[]
  


  constructor(
    injector: Injector,
    private _router: Router,
    private userInfo: UserInformation,
    private _itemizedService:ItemizationServiceProxy,
    private _timeManagementsServiceProxy :TimeManagementsServiceProxy,

  ) {
    super(injector);
  }
  ngOnInit() {
    this.accountId = history.state.data.accountId
    this.accountName = history.state.data.accountName
    this.accountNo = history.state.data.accountNo
    this.commantBox = true;
    this.getProfilePicture();
    this.userName = this.appSession.user.name.toString();


  }
  RedirectToAddNew() : void {
    this._router.navigate(['/app/main/reconcilliation/itemized/create-edit-itemized'],{ state: { data: { accountId : this.accountId ,accountName :this.accountName ,accountNo: this.accountNo,ItemizedItemId : 0 }} });
  }
  RedirectToDetail(ItemizedItemId) : void {   
      this._router.navigate(['/app/main/reconcilliation/itemized/itemized-details'],{ state: { data: { accountId : this.accountId ,accountName :this.accountName ,accountNo: this.accountNo,ItemizedItemId : ItemizedItemId }} });
  }
  getAllItemizedList(event?: LazyLoadEvent){
    this.primeNgEvent = event;
    if (this.primengTableHelper.shouldResetPaging(event)) {
      this.paginator.changePage(0);
      return;
    }

  this.primengTableHelper.showLoadingIndicator();
  this._itemizedService.getAll(
    this.filterText,
    this.AccountNumber == "" ? this.accountId : 0,
    moment(this.monthFilter),
    this.AccountNumber,
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
      this.postedCommentList = this.ItemizedList[0].comments;
      this.itemList.forEach(i => {
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
  this.monthFilter = new Date(add(event, 2, "day"));;
  this._timeManagementsServiceProxy.checkManagementExist(moment(this.monthFilter)).subscribe(result => { 
    if (result)
    {
      this.AccountNumber = this.accountNo;
      this.getAllItemizedList(this.primeNgEvent);
    }
    else
    {
      this.CreateTimeManagementDto.month =  moment(this.monthFilter)
      this.CreateTimeManagementDto.status =  false
      this.message.confirm(
        'Are you want to define this month as closing month.',
        this.l(' Selected Month Does not Exist'),
        (isConfirmed) => {
          if (isConfirmed) {
            this._timeManagementsServiceProxy.createOrEdit(this.CreateTimeManagementDto).subscribe(() => {
              this.AccountNumber = this.accountNo;
              this.getAllItemizedList(this.primeNgEvent);
             })      
          }
        }
      );
    }  
  })
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
    this._itemizedService.postComment(this.comment,this.accountId,4).subscribe((result)=> {
      this.getAllItemizedList(this.primeNgEvent);
      this.comment = ""
    })
  }
  onCancelComment(): void {
    this.commantBox = true;
    this.comment = false;
  }

}