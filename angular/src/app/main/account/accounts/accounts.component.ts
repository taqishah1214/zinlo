import { Component, OnInit, ViewChild, Injector } from '@angular/core';
import { Router } from '@angular/router';
import { AccountSubTypeServiceProxy, ChartsofAccountServiceProxy } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import { LazyLoadEvent } from 'primeng/api';
import { Paginator } from 'primeng/paginator';
import { Table } from 'primeng/table';
import { UserListComponentComponent } from '../../checklist/user-list-component/user-list-component.component';
import { UppyConfig } from 'uppy-angular';
import { FileDownloadService } from '@shared/utils/file-download.service';
import { HttpClient } from '@angular/common/http';
import { AppConsts } from '@shared/AppConsts';
import { finalize } from 'rxjs/operators';
import { FileUpload } from 'primeng/fileupload';

@Component({
  selector: 'app-accounts',
  templateUrl: './accounts.component.html',
  styleUrls: ['./accounts.component.css']
})

export class AccountsComponent extends AppComponentBase implements OnInit {
  @ViewChild(UserListComponentComponent, { static: false }) selectedUserId: UserListComponentComponent;
  @ViewChild('dataTable', { static: true }) dataTable: Table;
  @ViewChild('paginator', { static: true }) paginator: Paginator;
  advancedFiltersAreShown = false;
  filterText = '';
  accountTypeFilter : number = 0;
  categoryFilter: number = 0;
  statusFilter: number = 0;              
  tasksList: any;
  list: any = []
  chartsOfAccountList: any = []
  UserSpecficchartsOfAccountList: any = []
  id: number;
  AssigniInputBox: boolean;
  AssigniBoxView: boolean;
  FilterBoxOpen: boolean;
  public rowId: number = 0;
  assigniNameForHeader: any = [];
  plusUserBadgeForHeader: boolean;
  rowid: number;
  collapsibleRow : boolean
  collapsibleRowId : number;
  remainingUserForHeader: any = [];
  accountSubTypes: any = [];
  accountType : any;
  accountTypeList: Array<{ id: number, name: string }> = [{ id: 1, name: "Fixed" }, { id: 2, name: "Assets" }, { id: 3, name: "Liability" }];
  reconcillationTypeList: Array<{ id: number, name: string }> = [{ id: 1, name: "Itemized" }, { id: 2, name: "Amortization" }]
  updateAssigneeOnHeader: boolean = true;
  getAccountWithAssigneeId : number = 0;
  uploadUrl = AppConsts.remoteServiceBaseUrl + '/Users/ImportAccountsFromExcel';
  uploadBalanceUrl = AppConsts.remoteServiceBaseUrl + '/Users/ImportAccountsTrialBalanceFromExcel';

  @ViewChild('ExcelFileUpload', { static: true }) excelFileUpload: FileUpload;

  constructor(private _router: Router,
    private _accountSubTypeService: AccountSubTypeServiceProxy, injector: Injector, private _httpClient: HttpClient,
    private _chartOfAccountService: ChartsofAccountServiceProxy,private _fileDownloadService: FileDownloadService)
    {
    super(injector)
    this.FilterBoxOpen = false;
  }
  ngOnInit() {
    this.AssigniInputBox = false;
    this.AssigniBoxView = true;
    this.collapsibleRow = false;
    this.accountType = "Account Type"
    this.loadAccountSubType();
  }

  accountTypeClick(id,name) : void {
    this.accountType = name
    this.accountTypeFilter = id
    this.updateAssigneeOnHeader = false;
    this.getAllAccounts()
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

  getAllAccounts(event?: LazyLoadEvent) {
    if (this.primengTableHelper.shouldResetPaging(event)) {
      this.paginator.changePage(0);
      return;
    }

   

    this.primengTableHelper.showLoadingIndicator();
    this._chartOfAccountService.getAll(
      this.filterText,
      this.accountTypeFilter,
      this.getAccountWithAssigneeId,
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
      this.chartsOfAccountList.forEach(i => {
        i["accountType"] =  this.getNameofAccountTypeAndReconcillation(i.accountTypeId,"accountType")
        i["reconciliationType"] =   this.getNameofAccountTypeAndReconcillation(i.reconciliationTypeId,"reconcillation")           
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
editAccount(id) : void {
  this._router.navigate(['/app/main/account/accounts/create-edit-accounts'], { state: { data: { id: id} } });
}

deleteAccount(id) : void {
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
  this._router.navigate(['/app/main/account/accounts/create-edit-accounts'], { state: { data: { id: 0} } });
}

  getNameofAccountTypeAndReconcillation(id , key ) : string {  
    var result = "" ;
    if (key === "accountType")
     {
      this.accountTypeList.forEach(i => {
        if  (i.id == id)
        {
          result = i.name
        }
      })
      return result;
     }
     else if (key === "reconcillation")
     {
      this.reconcillationTypeList.forEach(i => {
        if  (i.id == id)
        {
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
    this.accountTypeFilter = 0
    this.updateAssigneeOnHeader = true;
    this.getAccountWithAssigneeId = 0;
    this.getAllAccounts();
  }

  refreshGrid() : void {
    this.updateAssigneeOnHeader = false;
    this.getAccountWithAssigneeId = 0;
    this.getAllAccounts()
    this.notify.success(this.l('Assigni Successfully Updated.'));
  }

  settings: UppyConfig = {
    uploadAPI: {
      endpoint: "http://localhost:22742/api/services/app/Attachments/PostAttachmentFile",
    },
    plugins: {
      Webcam: false
    }
  }
  settingsUppy: UppyConfig = {
    uploadAPI: {
      endpoint: "http://localhost:22742/api/services/app/Attachments/PostAttachmentFile",
    },
    plugins: {
      Webcam: false
    }
  }

  uploadaccountExcel(data: { files: File }): void {
    const formData: FormData = new FormData();
    const file = data.files[0];
    formData.append('file', file, file.name);

    this._httpClient
        .post<any>(this.uploadUrl, formData)
        .pipe(finalize(() => this.excelFileUpload.clear()))
        .subscribe(response => {
            if (response.success) {
                this.notify.success(this.l('ImportAccountsProcessStart'));
            } else if (response.error != null) {
                this.notify.error(this.l('ImportAccountsUploadFailed'));
            }
        });
}

uploadAccountsTrialBalanceExcel(data: { files: File }): void {
  const formData: FormData = new FormData();
  const file = data.files[0];
  formData.append('file', file, file.name);

  this._httpClient
      .post<any>(this.uploadBalanceUrl, formData)
      .pipe(finalize(() => this.excelFileUpload.clear()))
      .subscribe(response => {
          if (response.success) {
              this.notify.success(this.l('ImportAccountsTrialBalanceProcessStart'));
          } else if (response.error != null) {
              this.notify.error(this.l('ImportAccountsTrialBalanceUploadFailed'));
          }
      });
}

onUploadExcelError(): void {
    this.notify.error(this.l('ImportAccountsUploadFailed'));
}

  downloadExcelFile(): void {
    console.log();
    this._chartOfAccountService.getChartsofAccountToExcel(0)
        .subscribe(result => {
            this._fileDownloadService.downloadTempFile(result);
        });
}



}