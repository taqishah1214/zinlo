import { Component, Injector, ViewChild } from '@angular/core';
import { Router } from '@angular/router';
import { AppComponentBase } from '@shared/common/app-component-base';
import * as _ from 'lodash';
import { AmortizationServiceProxy ,TimeManagementsServiceProxy, CreateOrEditTimeManagementDto, AuditLogServiceProxy, ChartsofAccountServiceProxy } from '@shared/service-proxies/service-proxies';
import { UserInformation } from '../../CommonFunctions/UserInformation';
import { LazyLoadEvent } from 'primeng/api';
import { Paginator } from 'primeng/paginator';
import { Table } from 'primeng/table';
import { AppConsts } from '@shared/AppConsts';
import * as moment from 'moment';
import { add, subtract } from 'add-subtract-date';
import { StoreDateService } from "../../../services/storedate.service";
import * as $ from 'jquery';
@Component({
  selector: 'app-amortized',
  templateUrl: './amortized.component.html',
  styleUrls: ['./amortized.component.css']
})
export class AmortizedComponent extends AppComponentBase {
  commentFiles:File[]=[];
  AllOrActive :boolean = false;
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
  historyOfTask: any = [];
  assigniHistory : any = [];
  statusHistory: any = [];
  users : any = [];
  commentShow = true;
  historyList : any =[];
  AssigniColorBox: any = ["bg-purple", "bg-golden", "bg-sea-green", "bg-gray"," .bg-brown",".bg-blue","bg-magenta"]
  userSignInName: string;
  StatusColorBox: any = ["bg-blue", "bg-sea-green", "bg-gray"]
  buttonColorForComment : any = "bg-grey"
  buttonColorForHistory : any = "bg-lightgrey"
  accountSubypeList : any  = []
  monthStatus : boolean;

  constructor(
    injector: Injector,
    private _router: Router,
    private userInfo: UserInformation,
    private _amortizationService:AmortizationServiceProxy,
    private _timeManagementsServiceProxy :TimeManagementsServiceProxy,
    private _auditLogService : AuditLogServiceProxy,
    private storeData: StoreDateService,
    private _chartOfAccountService: ChartsofAccountServiceProxy

  ) {
    super(injector);
  }
  ngOnInit() {
    if (history.state.navigationId == 1){
      this._router.navigate(['/app/main/reconcilliation']);
    }
      $(document).ready(function(){
        // Show hide popover
            $(".dropdown-menu").on('click', function (e) {
      e.stopPropagation();
      });
    });
    this.monthFilter = history.state.data.selectedDate
    this.storeData.allUsersInformationofTenant.subscribe(userList => this.users = userList)
    this.storeData.allAccountSubTypes.subscribe(accountSubypeList => this.accountSubypeList = accountSubypeList)
    this.accountId = history.state.data.accountId
    this.accountName = history.state.data.accountName
    this.accountNo = history.state.data.accountNo
    this.commantBox = true;
    this.getProfilePicture();
    this.userName = this.appSession.user.name.toString();
  }
  RedirectToDetails(amortizedItemId,accured,net) : void {
    this._router.navigate(['/app/main/reconcilliation/amortized/amortized-details'],{ state: { data: { monthStatus : this.monthStatus , accountId : this.accountId ,accountName :this.accountName ,accountNo: this.accountNo,amortrizedItemId : amortizedItemId,accuredAmount: accured,netAmount:net ,  selectedDate : this.monthFilter}} });
  }
  RedirectToAddNewItem()
  {
      this._router.navigate(['/app/main/reconcilliation/amortized/create-edit-amortized'],{ state: { data: { accountId : this.accountId ,accountName :this.accountName ,accountNo: this.accountNo,amortrizedItemId : 0 , selectedDate : this.monthFilter}} });
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
    this.accountId,
    moment(this.monthFilter),
    this.AllOrActive,
    this.primengTableHelper.getSorting(this.dataTable),
    this.primengTableHelper.getSkipCount(this.paginator, event),
    10
    // this.primengTableHelper.getMaxResultCount(this.paginator, event)
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
    this.monthStatus = j.monthStatus;
    switch (j.reconciliedBase) {
      case 1:
        this.trialBalanceNet = j.totalTrialBalanceNet
        this.varianceNet = j.varianceNetAmount 
        break;
      case 2:
        this.trialBalanceBeginning = j.totalTrialBalanceBegininng
        this.varianceBeginning = j.varianceBeginningAmount
        this.trialBalanceAccured = j.totalTrialBalanceAccured
        this.varianceAccured = j.varianceAccuredAmount
        break;
      case 3:
        this.trialBalanceBeginning = j.totalTrialBalanceBegininng
        this.varianceBeginning = j.varianceBeginningAmount
        this.trialBalanceAccured = j.totalTrialBalanceAccured
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
  this.getAllAmortizedList(this.primeNgEvent)
  // this._timeManagementsServiceProxy.checkManagementExist(moment(this.monthFilter)).subscribe(result => { 
  //   if (result)
  //   {
  //     this.AccountNumber = this.accountNo;
  //     this.getAllAmortizedList(this.primeNgEvent);
  //   }
  //   else
  //   {
  //     this.CreateTimeManagementDto.month =  moment(this.monthFilter)
  //     this.CreateTimeManagementDto.status =  false
  //     this.message.confirm(
  //       'You want to define this month.',
  //       this.l(' Selected month does not Exist'),
  //       (isConfirmed) => {
  //         if (isConfirmed) {
  //           this._timeManagementsServiceProxy.createOrEdit(this.CreateTimeManagementDto).subscribe(() => {
  //             this.AccountNumber = this.accountNo;
  //             this.getAllAmortizedList(this.primeNgEvent);
  //            })      
  //         }
  //       }
  //     );
  //   }  
  // })
}
uploadCommentFile($event) {
  this.commentFiles.push($event.target.files[0]);
}
removeCommentFile(index)
{
  this.commentFiles.splice(index, 1);
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
      '',
      'The variance is equal to 0. Are you sure you want to reconcile this account?',
      (isConfirmed) => {
        if (isConfirmed) {
          if (isConfirmed) {
            
            this._chartOfAccountService.checkAsReconciliedMonthly(this.accountId,moment(this.monthValue)).subscribe(resp => {
              this.notify.success(this.l('Variance is equal to 0, hence the account is reconciled.'));
              this._router.navigate(['/app/main/reconcilliation']);
            })
          }
        }
      }
    );
  }
  reconciledClick() {
    if (this.reconciliedBase == 1) {
      if (this.netAmount - this.trialBalanceNet == 0) {
        this.reconciliedAccount()
      }
      else {
        this.notify.error(this.l('Variance is not equal to 0, hence the account is not reconciled'));
      }
    }
    if (this.reconciliedBase == 2 || this.reconciliedBase == 3 ){
      
      if ((this.accuredAmount -this.trialBalanceBeginning == 0) && (this.begininngAmount -this.trialBalanceBeginning == 0))
      {
        this.reconciliedAccount()
      }
      else {
        this.notify.error(this.l('Variance is not equal to 0, hence the account is not reconciled'));  
      }
    }
    else {
      this.notify.error(this.l('Variance is not equal to 0, hence the account is not reconciled'));
    }
  }
  changeToggleValue():void{
    if(this.AllOrActive)
    {
      this.AllOrActive = false;
    }
    else{
         this.AllOrActive = true;
    }
    this.getAllAmortizedList();

  }





  getAuditLogOfAccount() {
    this._auditLogService.getEntityHistory( history.state.data.accountId.toString(), "Zinlo.ChartofAccounts.ChartofAccounts","",history.state.data.accountBalanceId).subscribe(resp => {
      this.historyOfTask = resp
      this.historyOfTask.forEach((element,index) => {
        switch (element.propertyName) {
          case "AssigneeId":         
            element["result"] =  this.setAssigniHistoryParam(element,index)
            break;
            case "Status":          
            element["result"] = this.setStatusHistoryParam(element)
            break;
            case "AccountName":          
            element["result"] = this.setAccountNameHistoryParam(element)
            break;
            case "AccountType":          
            element["result"] = this.setAccountTypeHistoryParam(element)
            break;
            case "AccountSubTypeId":          
            element["result"] = this.setAccountSubTypeHistoryParam(element)
            break;
            case "Status":          
            element["result"] = this.setStatusHistoryParam(element)
            break;
            default:
            console.log("not found");
            break;
        }
        ;
      });
    })
  }


  
getAccountSubType(id) {
  return this.accountSubypeList.findIndex(x => x.value == id);
}

setAccountSubTypeHistoryParam(item){
  let array : any = []
  array["ChangeOccurUser"] =  this.users[this.findTheUserFromList(item.userId)]; 
  array["NewValue"] =  this.accountSubypeList[this.getAccountSubType(parseInt(item.newValue))]; 
  array["PreviousValue"] = this.accountSubypeList[this.getAccountSubType(parseInt(item.originalValue))]; 
  debugger
  return array

}

  findTheUserFromList(id) : number{
  return this.users.findIndex(x => x.id === id);
  }


  
  
  setAccountTypeHistoryParam(item){
    let array : any = []
    array["ChangeOccurUser"] = this.users[this.findTheUserFromList(item.userId)]; 
    array["NewValue"] = this.findAccountTypeName(parseInt(item.newValue)); 
    array["PreviousValue"] = this.findAccountTypeName(parseInt(item.originalValue)); 
    return array
  }

 setAssigniHistoryParam(item,index){
   let array : any = []
  array["ChangeOccurUser"] = this.users[this.findTheUserFromList(item.userId)]; 
  array["NewValue"] = this.users[this.findTheUserFromList(parseInt(item.newValue))];
  array["colorNewValue"] = "bg-magenta"
  array["PreviousValue"] = this.users[this.findTheUserFromList(parseInt(item.originalValue))]; 
  array["colorPreviousValue"] = "bg-purple"
  return array
 }

 setStatusHistoryParam(item){
  let array : any = []
  array["ChangeOccurUser"] = this.users[this.findTheUserFromList(item.userId)]; 
  array["NewValue"] = this.findStatusName(parseInt(item.newValue)); 
  array["PreviousValue"] = this.findStatusName(parseInt(item.originalValue)); 
  array["PreviousColor"] = this.findStatusColor(parseInt(item.originalValue)); 
  array["NewValueColor"] = this.findStatusColor(parseInt(item.newValue)); 

  return array
}

setAccountNameHistoryParam(item){
  let array : any = []
  array["ChangeOccurUser"] = this.users[this.findTheUserFromList(item.userId)]; 
  array["NewValue"] = item.newValue; 
  array["PreviousValue"] = item.originalValue; 
  return array
}

  findStatusName(value): string {

    switch (value) {
      case 1:
        return "In Process"
      case 2:
        return "Open"
      case 3:
        return "Complete"
      default:
        return ""
    }

  }

  findStatusColor(id) :string {
    if (id == 1) {
      return this.StatusColorBox[0]
    }
    else if (id == 2) {
      return this.StatusColorBox[2]
    }
    else if (id == 3) {
      return this.StatusColorBox[1]
    }
  }

  findAccountTypeName(value) :string
  {


    switch (value) {
      case 1:
        return "Equity"
      case 2:
        return "Assets"
      case 3:
        return "Liability"
      default:
        return ""
    }

  }













}