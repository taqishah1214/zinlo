import { Component, Injector, ViewChild } from '@angular/core';
import { Router } from '@angular/router';
import { AppComponentBase } from '@shared/common/app-component-base';
import * as _ from 'lodash';
import { AmortizationServiceProxy ,TimeManagementsServiceProxy, CreateOrEditTimeManagementDto } from '@shared/service-proxies/service-proxies';
import { UserInformation } from '../../CommonFunctions/UserInformation';
import { LazyLoadEvent } from 'primeng/api';
import { Paginator } from 'primeng/paginator';
import { Table } from 'primeng/table';
import { AppConsts } from '@shared/AppConsts';
import * as moment from 'moment';
import { add, subtract } from 'add-subtract-date';

@Component({
  selector: 'app-amortized',
  templateUrl: './amortized.component.html',
  styleUrls: ['./amortized.component.css']
})
export class AmortizedComponent extends AppComponentBase {
  UserProfilePicture: any;
  monthValue: Date = new Date();
  commantBox: boolean;
  accountId: any;
  amortizedItemList: any = []
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
  begininngAmount : any;
  accuredAmount :any;
  netAmount : any;
  itemList : any = [];
  monthFilter = new Date();
  trialBalanceBeginning : any = 0.0;
  trialBalanceAccured: any = 0.0;
  trialBalanceNet:any = 0.0;
  varianceBeginning : any =0.0;
  varianceAccured : any = 0.0;
  varianceNet : any = 0.0;
  AccountNumber : any  = "";
  CreateTimeManagementDto : CreateOrEditTimeManagementDto = new CreateOrEditTimeManagementDto();
  postedCommentList : any =[]
  comment : any = ""
  reconciliedBase : any

  constructor(
    injector: Injector,
    private _router: Router,
    private userInfo: UserInformation,
    private _amortizationService:AmortizationServiceProxy,
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
  RedirectToDetails(amortizedItemId,accured,net) : void {
    this._router.navigate(['/app/main/reconcilliation/amortized/amortized-details'],{ state: { data: { accountId : this.accountId ,accountName :this.accountName ,accountNo: this.accountNo,amortrizedItemId : amortizedItemId,accuredAmount: accured,netAmount:net }} });
  }
  RedirectToAddNewItem()
  {
      this._router.navigate(['/app/main/reconcilliation/amortized/create-edit-amortized'],{ state: { data: { accountId : this.accountId ,accountName :this.accountName ,accountNo: this.accountNo,amortrizedItemId : 0 }} });
  }

  getAllAmortizedList(event?: LazyLoadEvent){
    this.primeNgEvent = event;
    if (this.primengTableHelper.shouldResetPaging(event)) {
      this.paginator.changePage(0);
      return;
    }

  this.primengTableHelper.showLoadingIndicator();
  this._amortizationService.getAll(
    this.filterText,
    this.AccountNumber == "" ? this.accountId : 0,
    moment(this.monthFilter),
    this.AccountNumber,
    this.primengTableHelper.getSorting(this.dataTable),
    this.primengTableHelper.getSkipCount(this.paginator, event),
    this.primengTableHelper.getMaxResultCount(this.paginator, event)
  ).subscribe(result => {
    this.primengTableHelper.totalRecordsCount = result.totalCount;
    this.primengTableHelper.records = result.items;
    this.primengTableHelper.hideLoadingIndicator();
    this.amortizedItemList = result.items;
    this.postedCommentList = this.amortizedItemList[0].comments;
    this.amortizedItemList.forEach(j => {  
    this.begininngAmount = j.totalBeginningAmount;
    this.accuredAmount= j.totalAccuredAmortization;
    this.netAmount = j.totalNetAmount;
    this.reconciliedBase = j.reconciliedBase
    switch (j.reconciliedBase) {
      case 1:
        this.trialBalanceNet = j.totalTrialBalance
        this.varianceNet = j.varianceNetAmount 
        break;
      case 2:
        this.trialBalanceBeginning = j.totalTrialBalance
        this.varianceBeginning = j.varianceBeginningAmount
        this.trialBalanceAccured = j.totalTrialBalance
        this.varianceAccured = j.varianceAccuredAmount
        break;
      case 3:
        this.trialBalanceBeginning = j.totalTrialBalance
        this.varianceBeginning = j.varianceBeginningAmount
        this.trialBalanceAccured = j.totalTrialBalance
        this.varianceAccured = j.varianceAccuredAmount
        break;
      default:
        break;
    }
    this.itemList = j.amortizedListForViewDtos;
    j.amortizedListForViewDtos.forEach(i => {
      i.attachments.forEach(element => {
        var attachmentName = element.attachmentPath.substring(element.attachmentPath.lastIndexOf("/") + 1, element.attachmentPath.lastIndexOf("zinlo"));
        element["attachmentExtension"] = this.getExtensionImagePath(element.attachmentPath)
        element["attachmentName"] = attachmentName
        element["attachmentUrl"] = AppConsts.remoteServiceBaseUrl+"/"+ element.attachmentPath
      });
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
      this.getAllAmortizedList(this.primeNgEvent);
    }
    else
    {
      this.CreateTimeManagementDto.month =  moment(this.monthFilter)
      this.CreateTimeManagementDto.status =  false
      this.message.confirm(
        'You want to define this month.',
        this.l(' Selected month does not Exist'),
        (isConfirmed) => {
          if (isConfirmed) {
            this._timeManagementsServiceProxy.createOrEdit(this.CreateTimeManagementDto).subscribe(() => {
              this.AccountNumber = this.accountNo;
              this.getAllAmortizedList(this.primeNgEvent);
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
    this._amortizationService.postComment(this.comment,this.accountId,5).subscribe((result)=> {
      this.getAllAmortizedList(this.primeNgEvent);
      this.comment = ""
    })
  }
  onCancelComment(): void {
    this.commantBox = true;
  }

  reconciliedAccount() {
    this.message.confirm(
      'The variance is equal to 0. Do you want to reconciled this account',
       "",
      (isConfirmed) => {
        if (isConfirmed) {
          this._timeManagementsServiceProxy.createOrEdit(this.CreateTimeManagementDto).subscribe(() => {
            this.notify.success(this.l('Variance is equal to 0, hence the account is reconciled'));
            this._router.navigate(['/app/main/reconcilliation']);
           })      
        }
      }
    );
  }


  reconciledClick() {
    if (this.varianceBeginning == 0 && this.varianceAccured == 0){
     this.reconciliedAccount()
    }
    else if (this.varianceNet ==0 && this.reconciliedBase== 1)
    {
      this.reconciliedAccount()
    }
    else {
      this.notify.error(this.l('Variance is not equal to 0, hence the account is not reconciled'));

    }

  }



}