import { Component, OnInit, ViewChild, Injector } from '@angular/core';
import { Router } from '@angular/router';
import { AccountSubTypeServiceProxy, ChartsofAccountServiceProxy } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import { LazyLoadEvent } from 'primeng/api';
import { Paginator } from 'primeng/paginator';
import { Table } from 'primeng/table';
import { UserListComponentComponent } from '../../checklist/user-list-component/user-list-component.component';
import { UppyConfig} from 'uppy-angular';
import { FileDownloadService } from '@shared/utils/file-download.service';
import { HttpClient } from '@angular/common/http';
import { AppConsts } from '@shared/AppConsts';
import { finalize } from 'rxjs/operators';
import { FileUpload } from 'primeng/fileupload';
import { Observable } from 'rxjs';
import { SignalRService } from '@app/services/signalRService';
import { StoreDateService } from "../../../services/storedate.service";
import * as moment from 'moment';
import * as $ from 'jquery';
import { add, subtract } from 'add-subtract-date';


@Component({
  selector: 'app-accounts',
  templateUrl: './accounts.component.html',
  styleUrls: ['./accounts.component.css']
})

export class AccountsComponent extends AppComponentBase implements OnInit {
  @ViewChild(UserListComponentComponent, { static: false }) selectedUserId: UserListComponentComponent;
  @ViewChild('dataTable', { static: true }) dataTable: Table;
  @ViewChild('paginator', { static: true }) paginator: Paginator;
  AllOrActive = false;
  advancedFiltersAreShown = false;
  account:boolean=false;
  balance:boolean=true;
  filterText = '';
  accountTypeFilter: number = 0;
  categoryFilter: number = 0;
  statusFilter: number = 0;
  tasksList: any;
  list: any = [];
  chartsOfAccountList: any = []
  UserSpecficchartsOfAccountList: any = []
  id: number;
  AssigniInputBox: boolean;
  AssigniBoxView: boolean;
  FilterBoxOpen: boolean;
  rowId: number = 0;
  rowIdForBulkDelete: number = 0;

  assigniNameForHeader: any = [];
  plusUserBadgeForHeader: boolean;
  rowid: number;
  collapsibleRow: boolean
  collapsibleRowId: number;
  remainingUserForHeader: any = [];
  accountSubTypes: any = [];
  accountType: any;
  accountTypeList: Array<{ id: number, name: string }> = [{ id: 1, name: "Equity" }, { id: 2, name: "Asset" }, { id: 3, name: "Liability" }];
  reconcillationTypeList: Array<{ id: number, name: string }> = [{ id: 1, name: "Itemized" }, { id: 2, name: "Amortized" }]
  updateAssigneeOnHeader: boolean = true;
  getAccountWithAssigneeId: number = 0;
  attachmentPathsTrialBalance: any = [];
  attachmentPathsChartsofAccounts: any = [];
  chartsOfAccountsfileUrl : string = "";
  chartsOfAccountsfileUrlTrialBalance : string = "";
  monthStatus : boolean = false
  users: any;
  delectedBulkAccounts : any [];
  selectedDate : any = new Date();


  uploadUrl = AppConsts.remoteServiceBaseUrl + '/AccountsExcel/ImportAccountsFromExcel';
  uploadBalanceUrl = AppConsts.remoteServiceBaseUrl + '/AccountsExcel/ImportAccountsTrialBalanceFromExcel';

  @ViewChild('ExcelFileUpload', { static: true }) excelFileUpload: FileUpload;

  constructor(private _router: Router,
    private _accountSubTypeService: AccountSubTypeServiceProxy, injector: Injector, private _httpClient: HttpClient,
    private _chartOfAccountService: ChartsofAccountServiceProxy, private _fileDownloadService: FileDownloadService,
    private _signalRService: SignalRService,private userDate: StoreDateService) {
    super(injector)
    this.FilterBoxOpen = false;
  }
  onClose(){
    console.log(this.chartsOfAccountsfileUrl)
  }
  onBreadChange(value){
    if(value=="Upload Trial Balance")
    {
      this.account=false;
      this.balance=true;
    }
    else if(value=="Upload Chart of Accounts")
    {
      this.balance=false;
      this.account=true;
    }
 }
  ngOnInit() {
    $(document).ready(function(){
        // Show hide popover
            $(".dropdown-menu").on('click', function (e) {
      e.stopPropagation();
      });
    });
    this.userDate.allUsersInformationofTenant.subscribe(userList => this.users = userList)
    this.AssigniInputBox = false;
    this.AssigniBoxView = true;
    this.collapsibleRow = false;
    this.accountType = "Account Type"
    this.loadAccountSubType();
    
  }
  onOpenCalendar(container) {
    container.monthSelectHandler = (event: any): void => {
      container._store.dispatch(container._actions.select(event.date));
    };
    container.setViewMode('month');
  }
  accountTypeClick(event): void {
    if (parseInt(event.target.value) == 0) {
      this.accountTypeFilter = parseInt(event.target.value)
      this.updateAssigneeOnHeader = false;
      this.getAllAccounts()
    }
    else{
      this.accountTypeFilter = parseInt(event.target.value)
      this.updateAssigneeOnHeader = false;
      this.getAllAccounts()
    } 
  }

  openFieldUpdateAssignee(record) {
    this.rowid = record;
  }
  collapsibleRowClick(id) {
    this.collapsibleRowId = id;
    this.collapsibleRow = !this.collapsibleRow;
  }

  searchWithName() {
    this.updateAssigneeOnHeader = false
    this.getAllAccounts();
  }
  checkValue(id){
   this.chartsOfAccountList[this.getAccountIndex(id)].isSelected = !this.chartsOfAccountList[this.getAccountIndex(id)].isSelected;
 }

 selectAllAccounts() {
this.chartsOfAccountList.forEach(element => {
  element.isSelected = true
  });
 }



  getAllAccounts(event?: LazyLoadEvent) {
    if (this.primengTableHelper.shouldResetPaging(event)) {
      this.paginator.changePage(0);
      return;
    }
    this.primengTableHelper.showLoadingIndicator();
    debugger;
    this._chartOfAccountService.getAll(
      this.filterText,
      this.accountTypeFilter,
      moment(this.selectedDate),
      this.getAccountWithAssigneeId,
      this.AllOrActive,
      this.primengTableHelper.getSorting(this.dataTable),
      this.primengTableHelper.getSkipCount(this.paginator, event),
      this.primengTableHelper.getMaxResultCount(this.paginator, event),
    ).subscribe(result => {
      this.plusUserBadgeForHeader = false;
      this.remainingUserForHeader = [];
      this.primengTableHelper.totalRecordsCount = result.totalCount;
      this.primengTableHelper.records = result.items;
      this.primengTableHelper.hideLoadingIndicator();
      this.list = result.items;
      this.chartsOfAccountList = result.items
      this.monthStatus = this.chartsOfAccountList[0].monthStatus;
      this.chartsOfAccountList.forEach(i => {
        i["assigneeName"] =  this.users[this.getUserIndex(i.assigneeId)].name;
        i["profilePicture"] =  this.users[this.getUserIndex(i.assigneeId)].profilePicture;
        i["accountType"] = this.getNameofAccountTypeAndReconcillation(i.accountTypeId, "accountType")
        i["reconciliationType"] = this.getNameofAccountTypeAndReconcillation(i.reconciliationTypeId, "reconcillation")
        i["isSelected"] =  false;
        if (this.updateAssigneeOnHeader === true) {
          this.assigniNameForHeader = [];
          this.plusUserBadgeForHeader = false
          this.remainingUserForHeader = [];
          this.assigniNameForHeader = i.overallMonthlyAssignee;
        }

      });

      if (this.updateAssigneeOnHeader === true) {
        if (this.assigniNameForHeader.length > 5) {
          this.remainingUserForHeader = [];
          var limitedUserNameForHeader = [];
          for (var i = 0; i < 5; i++) {
            limitedUserNameForHeader.push(this.assigniNameForHeader[i])
          }
          for (let index = 5; index < this.assigniNameForHeader.length; index++) {
            this.remainingUserForHeader.push(this.assigniNameForHeader[index])
          }
          this.assigniNameForHeader = limitedUserNameForHeader
          this.plusUserBadgeForHeader = true
        }
        this.updateAssigneeOnHeader = false;
      }

    });
  }

  getAccountIndex(id) {
    return this.chartsOfAccountList.findIndex(x => x.id === id);
  }
  

editAccount(id) : void { 
    this._router.navigate(['/app/main/account/create-edit-accounts'], { state: { data: { id: id,newSubTypeId : 0} } });
}
getUserIndex(id) {
  return this.users.findIndex(x => x.id === id);
}

  deleteAccount(id): void {
    this.message.confirm(
      '',
      this.l('AreYouSure'),
      (isConfirmed) => {
        if (isConfirmed) {
          this._chartOfAccountService.delete(id)
            .subscribe(() => {
              this.ResetGrid();
              this.notify.success(this.l('SuccessfullyDeleted'));
            });
        }
      }
    );
  }

RedirectToCreateAccount(): void {
  this._router.navigate(['/app/main/account/create-edit-accounts'], { state: { data: { id: 0 , newSubTypeId : 0} } });
}


  getNameofAccountTypeAndReconcillation(id, key): string {
    var result = "";
    if (key === "accountType") {
      this.accountTypeList.forEach(i => {
        if (i.id == id) {
          result = i.name
        }
      })
      return result;
    }
    else if (key === "reconcillation") {
      this.reconcillationTypeList.forEach(i => {
        if (i.id == id) {
          result = i.name
        }
      })
      return result;
    }
  }
  GetAssigniSpecficAccounts(userId) {
    this.getAccountWithAssigneeId = userId;
    this.getAllAccounts();
  }

  getUniqueAccounts(arr, comp) {

    const unique = arr
      .map(e => e[comp])
      .map((e, i, final) => final.indexOf(e) === i && i)
      .filter(e => arr[e]).map(e => arr[e]);
    return unique;
  }

  ChangeAssigniBox(id): void {
    this.rowId = id;
  }
  openFilterClick(): void {
    this.FilterBoxOpen = !this.FilterBoxOpen;
  }
  loadAccountSubType(): void {
    this._accountSubTypeService.accountSubTypeDropDown().subscribe(result => {
      this.accountSubTypes = result;
    });
  }
  ResetGrid(): void {
    this.accountType = "Account Type"
    this.accountTypeFilter = 0;
    this.categoryFilter=0;
    this.updateAssigneeOnHeader = true;
    this.getAccountWithAssigneeId = 0;
    this.getAllAccounts();
  }

  refreshGrid(): void {
    this.updateAssigneeOnHeader = false;
    this.getAccountWithAssigneeId = 0;
    this.getAllAccounts()
    this.notify.success(this.l('Assigni Successfully Updated.'));
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

  fileUploadOption(value){
    if(this.balance){
      this.fileUploadedResponseTrialBalance(value)
    }
    else if(this.account){
      this.fileUploadedResponseChartsOfAccounts(value)
    }
  }
  fileUploadedResponseChartsOfAccounts(value): void {
    var response = value.successful
    response.forEach(i => {
      this.attachmentPathsChartsofAccounts.push(i.response.body.result);
    });
    this.notify.success(this.l('Attachments are SavedSuccessfully Upload'));
    this.chartsOfAccountsfileUrl = this.attachmentPathsChartsofAccounts[0].toString();
   // this.uploadaccountExcel(url);
   
  }
  

  fileUploadedResponseTrialBalance(value): void {
    var response = value.successful
    console.log(response)
    response.forEach(i => {
      this.attachmentPathsTrialBalance.push(i.response.body.result);

    });
    console.log(this.attachmentPathsTrialBalance)
    console.log(this.chartsOfAccountsfileUrlTrialBalance = this.attachmentPathsTrialBalance[0].toString());

   // this.uploadAccountsTrialBalanceExcel(url);
    this.notify.success(this.l('Attachments are Saved Successfully'));

  }
  uploadaccountExcel(url: string): void {
    this._httpClient
      .get<any>(this.uploadUrl + "?url=" + AppConsts.remoteServiceBaseUrl + "/" + url)
      .subscribe(response => {
        if (response.success) {
          this.notify.success(this.l('ImportAccountsProcessStart'));
        } else if (response.error != null) {
          this.notify.error(this.l('ImportAccountsUploadFailed'));
        }
      });
  }

  filterByMonth(event) {
      this.selectedDate =new Date(add(this.selectedDate, 1, "month")); 
      debugger;
  }
  uploadAccountsTrialBalanceExcel(url: string): void {
    console.log(this.uploadBalanceUrl)
    console.log(url)
    console.log(AppConsts.remoteServiceBaseUrl)
    debugger;
    this._httpClient
      .get<any>(this.uploadBalanceUrl + "?url=" + AppConsts.remoteServiceBaseUrl + "/" + url + "&" +"monthSelected="+ this.selectedDate)
      .subscribe(response => {
        console.log(response)
        if (response.success) {
          this.notify.success(this.l('ImportAccountsTrialBalanceProcessStart'));
        } else if (response.error != null) {
          this.notify.error(this.l('ImportAccountsTrialBalanceUploadFailed'));
        }
      });
  }

  
  SaveChanges() :void{
    if(this.chartsOfAccountsfileUrl != "")
    {
      this.uploadaccountExcel(this.chartsOfAccountsfileUrl);
      this.chartsOfAccountsfileUrl = "";
    }
    if(this.chartsOfAccountsfileUrlTrialBalance != "")
    {
      this.uploadAccountsTrialBalanceExcel(this.chartsOfAccountsfileUrlTrialBalance);
      this.chartsOfAccountsfileUrlTrialBalance = "";
    }

   

    // this._signalRService.startHubConnection();
    // var file = this._signalRService.addBasicListener();
  }

  onUploadExcelError(): void {
    this.notify.error(this.l('ImportAccountsUploadFailed'));
  }

  downloadExcelFile(): void {
    this._chartOfAccountService.getChartsofAccountToExcel(0)
      .subscribe(result => {
        debugger
        this._fileDownloadService.downloadTempFile(result);
      });
  }
  downloadTrialBalanceExcelFile(): void {
    this._chartOfAccountService.loadChartsofAccountTrialBalanceToExcel()
      .subscribe(result => {
        this._fileDownloadService.downloadTempFile(result);
      });
  }
  changeToggleValue():void{
    if(this.AllOrActive)
    {
      this.AllOrActive = false;
    }
    else{
         this.AllOrActive = true;
    }
    this.getAllAccounts();
    console.log(this.AllOrActive);

  }
  restoreAccount(id):void{
   this._chartOfAccountService.restoreAccount(id).subscribe(result=>{
    this.notify.success(this.l('AccountRestoredSuccessfully'));
    this.getAllAccounts();
   })
  
  }
}