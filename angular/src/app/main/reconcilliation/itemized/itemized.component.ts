import { Component, Injector, ViewChild } from '@angular/core';
import { Router } from '@angular/router';
import { AppComponentBase } from '@shared/common/app-component-base';
import * as _ from 'lodash';
import {  ItemizationServiceProxy,TimeManagementsServiceProxy, CreateOrEditTimeManagementDto ,AuditLogServiceProxy} from '@shared/service-proxies/service-proxies';
import { UserInformation } from '../../CommonFunctions/UserInformation';
import { LazyLoadEvent } from 'primeng/api';
import { Paginator } from 'primeng/paginator';
import { Table } from 'primeng/table';
import { AppConsts } from '@shared/AppConsts';
import * as moment from 'moment';
import { add, subtract } from 'add-subtract-date';
import { UserDateService } from "../../../services/user-date.service";


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
  historyOfTask: any = [];
  assigniHistory : any = [];
  statusHistory: any = [];
  users : any = [];
  commentShow = true;
  historyList : any =[];
  AssigniColorBox: any = ["bg-purple", "bg-golden", "bg-sea-green", "bg-gray"," .bg-brown",".bg-blue","bg-magenta"]
  userSignInName: string;


  constructor(
    injector: Injector,
    private _router: Router,
    private userInfo: UserInformation,
    private _itemizedService:ItemizationServiceProxy,
    private _timeManagementsServiceProxy :TimeManagementsServiceProxy,
    private _auditLogService : AuditLogServiceProxy,
    private userDate: UserDateService,
    

  ) {
    super(injector);
  }
  ngOnInit() {
    this.userSignInName = this.appSession.user.name.toString().toUpperCase();
    this.userDate.allUsersInformationofTenant.subscribe(userList => this.users = userList)
    this.accountId = history.state.data.accountId
    this.accountName = history.state.data.accountName
    this.accountNo = history.state.data.accountNo
    this.commantBox = true;
    this.getProfilePicture();
    this.userName = this.appSession.user.name.toString();
    this.getAuditLogOfAccount();


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
    this.comment = "";
  }

  reconciledClick() {
    if ( this.variance == 0) {

      this.message.confirm(
        'The variance is equal to 0. Do you want to reconciled this account',
         "",
        (isConfirmed) => {
          if (isConfirmed) {
            this._timeManagementsServiceProxy.createOrEdit(this.CreateTimeManagementDto).subscribe(() => {
              this.notify.success(this.l('Variance is equal to 0, hence the account is reconciled.'));
              this._router.navigate(['/app/main/reconcilliation']);
             })      
          }
        }
      );

    }
    else
    {
      this.notify.error(this.l('Variance is not equal to 0, hence the account is not reconciled'));
    }

  }

















  onChangeCommentOrHistory(value){
    console.log(this.historyOfTask)
    debugger;
    if (value == 1)
   {
    this.commentShow = true
   }else {
    this.commentShow = false
   }
  }

  getAuditLogOfAccount() {
    this._auditLogService.getEntityHistory("1".toString(), "Zinlo.ClosingChecklist.ClosingChecklist").subscribe(resp => {
      this.historyOfTask = resp
      this.historyOfTask.forEach((element,index) => {
        switch (element.propertyName) {
          case "AssigneeId":         
            element["result"] =  this.setAssigniHistoryParam(element,index)
            break;
            case "Status":          
            element["result"] = this.setStatusHistoryParam(element)
            break;
            case "TaskName":          
            element["result"] = this.setTaskNameHistoryParam(element)
            break;
            case "DueDate":          
            element["result"] = this.setDueDateHistoryParam(element)
            break;
            case "DayBeforeAfter":          
            element["result"] = this.setDaysBeforeAfterHistoryParam(element)
            break;
            case "CategoryId":          
            element["result"] = this.setDaysCategoryIdHistoryParam(element)
            debugger;
            break;
            
          default:
            console.log("not found");
            break;
        }
        ;
      });
    })
  }


  




  findTheUserFromList(id) : number{
  return this.users.findIndex(x => x.id === id);
  }

  setDaysCategoryIdHistoryParam(item){
  let array : any = []
   array["ChangeOccurUser"] = this.users[this.findTheUserFromList(item.userId)]; 
   array["NewValue"] = this.getCategoryTitleWithName(parseInt(item.newValue)); 
   array["PreviousValue"] = this.getCategoryTitleWithName(parseInt(item.originalValue)); 
   debugger;
   return array
  }

  getRandomNo() {
    let a =  Math.random() * (6 - 0) + 0;
    return ;
  }
  
 async getCategoryTitleWithName(id) {
   return ""
  }

  getDaysBeforeAfterNameWith(id) {
    switch (id) {
      case 1:
        return "None"
      case 2:
        return "DaysBefore"
      case 3:
        return "DaysAfter"
      default:
        return ""
    }
  }

  setDaysBeforeAfterHistoryParam(item){
    let array : any = []
   array["ChangeOccurUser"] = this.users[this.findTheUserFromList(item.userId)]; 
   array["NewValue"] = this.getDaysBeforeAfterNameWith(parseInt(item.newValue)); 
   array["PreviousValue"] = this.getDaysBeforeAfterNameWith(parseInt(item.originalValue)); 
   return array
  }
  
  setDueDateHistoryParam(item){
    let array : any = []
   array["ChangeOccurUser"] = this.users[this.findTheUserFromList(item.userId)]; 
   array["NewValue"] = item.newValue; 
   array["PreviousValue"] = item.originalValue; 
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
  return array
}

setTaskNameHistoryParam(item){
  let array : any = []
  array["ChangeOccurUser"] = this.users[this.findTheUserFromList(item.userId)]; 
  array["NewValue"] = item.newValue; 
  array["PreviousValue"] = item.originalValue; 
  return array
}

  findStatusName(value): string {

    switch (value) {
      case 1:
        return "Not Started"
      case 2:
        return "In Process"
      case 3:
        return "On Hold"
      case 4:
        return "Completed"
      default:
        return ""
    }

  }



























}