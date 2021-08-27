import { Component, Injector, ViewChild } from '@angular/core';
import { Router } from '@angular/router';
import { AppComponentBase } from '@shared/common/app-component-base';
import * as _ from 'lodash';
import {  ItemizationServiceProxy,TimeManagementsServiceProxy, CreateOrEditTimeManagementDto ,AuditLogServiceProxy, ChartsofAccountServiceProxy, } from '@shared/service-proxies/service-proxies';
import { UserInformation } from '../../CommonFunctions/UserInformation';
import { LazyLoadEvent } from 'primeng/api';
import { Paginator } from 'primeng/paginator';
import { Table } from 'primeng/table';
import { AppConsts } from '@shared/AppConsts';
import * as moment from 'moment';
import { add, subtract } from 'add-subtract-date';
import { StoreDateService } from "../../../services/storedate.service";
import { UppyConfig } from 'uppy-angular';
import { HttpClient } from '@angular/common/http';


import * as $ from 'jquery';

@Component({
  selector: 'app-itemized',
  templateUrl: './itemized.component.html',
  styleUrls: ['./itemized.component.css']
})
export class ItemizedComponent extends AppComponentBase {
  AllOrActive : boolean = false;
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
  commentFiles:File[]=[];
  //
  selectedDate : any = new Date ();
  checkActiveMonth:boolean=true;
  activeSaveButton:boolean=false;
  attachmentPathsItemized: any = [];
  itemizedFileUrl : string = "";
  uploadItemizedUrl = AppConsts.remoteServiceBaseUrl + '/ReconciliationExcel/ImportItemizedItems';



  constructor(
    injector: Injector,
    private _router: Router,
    private userInfo: UserInformation,
    private _itemizedService:ItemizationServiceProxy,
    private _auditLogService : AuditLogServiceProxy,
    private storeData: StoreDateService,
    private _chartOfAccountService: ChartsofAccountServiceProxy,
    private _managementService: TimeManagementsServiceProxy,
    private _httpClient: HttpClient
  ) {
    super(injector);
  }
  uploadCommentFile($event) {
    this.commentFiles.push($event.target.files[0]);
  }
  removeCommentFile(index)
  {
    this.commentFiles.splice(index, 1);
  }
  ngOnInit() {
    if (history.state.navigationId == 1){
      this._router.navigate(['/app/main/reconciliation']);
    }
    $(document).ready(function(){
      // Show hide popover
          $(".dropdown-menu").on('click', function (e) {
    e.stopPropagation();
    });
    });
    
    //checking history.state.data & re-routing if undefined
        
    if (!history.state.data) {
      this._router.navigate(['/app/main/reconciliation']);
    }
    //modified

    this.monthValue = history.state.data.selectedDate
    this.userSignInName = this.appSession.user.name.toString().toUpperCase();
    this.storeData.allUsersInformationofTenant.subscribe(userList => this.users = userList)
    this.storeData.allAccountSubTypes.subscribe(accountSubypeList => this.accountSubypeList = accountSubypeList)
    this.accountId = history.state.data.accountId
    this.accountName = history.state.data.accountName
    this.accountNo = history.state.data.accountNo
    this.commantBox = true;
    this.getProfilePicture();
    this.userName = this.appSession.user.name.toString();
        
  }
  RedirectToAddNew() : void {
    this._router.navigate(['/app/main/reconciliation/itemized/create-edit-itemized'],{ state: { data: { accountId : this.accountId ,accountName :this.accountName ,accountNo: this.accountNo,ItemizedItemId : 0 , selectedDate : this.monthValue }} });
  }
  RedirectToDetail(ItemizedItemId,amount) : void {   
      this._router.navigate(['/app/main/reconciliation/itemized/itemized-details'],{ state: { data: { monthStatus : this.monthStatus ,accountId : this.accountId ,accountName :this.accountName ,accountNo: this.accountNo,ItemizedItemId : ItemizedItemId ,selectedDate :this.monthValue,amount : amount }} });
  }
  getAllItemizedList(event?: LazyLoadEvent){
    this.primeNgEvent = event;
    if (this.primengTableHelper.shouldResetPaging(event)) {
      this.paginator.changePage(0);
      return;
    }
    this.primengTableHelper.getMaxResultCount(this.paginator, event)
  this.primengTableHelper.showLoadingIndicator();
  this._itemizedService.getAll(
    this.filterText,
    this.accountId,
    moment(this.monthValue),
    this.AllOrActive,
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
      this.monthStatus = this.ItemizedList[0].monthStatus;
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
  this.monthValue = new Date(add(event, 2, "day"));
  this.getAllItemizedList(this.primeNgEvent)
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
  this._router.navigate(['/app/main/reconciliation']);

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
    if(this.comment){
    var index=this.comment.indexOf("</p>");
  }
  var i;
  for (i=3;i<index;i++)
  {
    if(this.comment[i]==' ')
    {
    }
    else{
      break;
    }
  }
  if(i!=index && this.comment){
    this._itemizedService.postComment(this.comment,this.accountId,4).subscribe((result)=> {
      this.getAllItemizedList(this.primeNgEvent);
      this.comment = ""
    })
  }
  }
  onCancelComment(): void {
    this.commantBox = true;
    this.comment = "";
  }

  reconciledClick() {
    if (this.TotalAmount - this.trialBalance == 0) {

      this.message.confirm(
        '',
        'The variance is equal to 0. Are you sure you want to reconcile this account?',
        (isConfirmed) => {
          if (isConfirmed) {
            
            this._chartOfAccountService.checkAsReconciliedMonthly(this.accountId,moment(this.monthValue)).subscribe(resp => {
              this.notify.success(this.l('Variance is equal to 0, hence the account is reconciled.'));
              this._router.navigate(['/app/main/reconciliation']);
            })
          }
        }
      );

    }
    else
    {
      this.notify.error(this.l('VarianceNotZero'));
    }

  }

  settings: UppyConfig = {
    uploadAPI: {
      endpoint: AppConsts.remoteServiceBaseUrl + '/api/services/app/Attachments/PostAttachmentFile',
    },
    plugins: {
      Webcam: false
    },
    allowMultipleUploads : false
  }

  fileUploadedResponseItemizedAccount(value): void {
    var response = value.successful
    response.forEach(i => {
      this.attachmentPathsItemized.push(i.response.body.result);

    });
    this.itemizedFileUrl = this.attachmentPathsItemized[0].toString();

   // this.uploadAccountsTrialBalanceExcel(url);
    this.notify.success(this.l('Attachments are Saved Successfully'));

  }

  checkMonthActiveorNot(event) {
    this._managementService.checkMonthStatus(moment(new Date(add(this.selectedDate, 2, "day")))).subscribe(result => {
      this.checkActiveMonth = result;
      this.activeSaveButton = !this.checkActiveMonth
    });
  }

  SaveChanges(): void {
    this.uploadAccountsItemizedExcel(this.itemizedFileUrl);
    this.itemizedFileUrl = "";
  }
  uploadAccountsItemizedExcel(url: string): void {    
    this._httpClient
      .get<any>(this.uploadItemizedUrl + "?url=" + AppConsts.remoteServiceBaseUrl + "/" + url + "&" +"monthSelected="+ this.selectedDate+ url + "&" +"chartsOfAccountId="+ this.accountId)
      .subscribe(response => {
        if (response.success) {
          this.notify.success(this.l('ImportItemizedProcessStart'));
        } else if (response.error != null) {
          this.notify.error(this.l('ImportItemizedUploadFailed'));
        }
      });
  }


  getAuditLogOfAccount() {
    let statusList = [];
    this._auditLogService.getEntityHistory(this.accountId.toString(), "Zinlo.ChartofAccounts.ChartofAccounts","",history.state.data.accountBalanceId).subscribe(resp => {
      resp.forEach((element,index) => {
      switch (element.propertyName) {
      case "Status":          
      element["result"] = this.setStatusHistoryParam(element)
      statusList.push(element)
      break;
      default:
      }
    })
     
    })

    this._auditLogService.getEntityHistory(this.accountId.toString(), "Zinlo.ChartofAccounts.ChartofAccounts","","").subscribe(resp => {
      this.historyOfTask = resp
      this.historyOfTask.forEach((element,index) => {
        switch (element.propertyName) {
          case "AssigneeId":         
            element["result"] =  this.setAssigniHistoryParam(element,index)
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
            default:
            break;
        }
        ;
      });
      statusList.forEach((item,index) => {
        this.historyOfTask.push(item);
      })
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
  changeToggleValue():void{
    if(this.AllOrActive)
    {
      this.AllOrActive = false;
    }
    else{
         this.AllOrActive = true;
    }
    this.getAllItemizedList();

  }

}