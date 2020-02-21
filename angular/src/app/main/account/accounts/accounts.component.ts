import { Component, OnInit, ViewChild, Injector } from '@angular/core';
import { Router } from '@angular/router';
import { AccountSubTypeServiceProxy, ChartsofAccountServiceProxy } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import { LazyLoadEvent } from 'primeng/api';
import { Paginator } from 'primeng/paginator';
import { Table } from 'primeng/table';
import { UserListComponentComponent } from '../../checklist/user-list-component/user-list-component.component';

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
  titleFilter = '';
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
  
  constructor(private _router: Router,
    private _accountSubTypeService: AccountSubTypeServiceProxy, injector: Injector,
    private _chartOfAccountService: ChartsofAccountServiceProxy) {
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
  }

  openFieldUpdateAssignee(record) {
    this.rowid = record;
  }
  collapsibleRowClick(id) {
    this.collapsibleRowId = id;
    this.collapsibleRow = !this.collapsibleRow;
  } 
  getAllAccounts(event?: LazyLoadEvent) {
    this.assigniNameForHeader = []
    if (this.primengTableHelper.shouldResetPaging(event)) {
      this.paginator.changePage(0);
      return;
    }

    this.primengTableHelper.showLoadingIndicator();
    this._chartOfAccountService.getAll(
      this.filterText,
      this.titleFilter,
      this.primengTableHelper.getSorting(this.dataTable),
      this.primengTableHelper.getSkipCount(this.paginator, event),
      this.primengTableHelper.getMaxResultCount(this.paginator, event)
    ).subscribe(result => {
      this.primengTableHelper.totalRecordsCount = result.totalCount;
      this.primengTableHelper.records = result.items;
      this.primengTableHelper.hideLoadingIndicator();
      this.list = result.items;
      this.chartsOfAccountList = result.items
      this.chartsOfAccountList.forEach(i => {
        i["accountType"] =  this.getNameofAccountTypeAndReconcillation(i.accountTypeId,"accountType")
        i["reconciliationType"] =   this.getNameofAccountTypeAndReconcillation(i.reconciliationTypeId,"reconcillation")           
        this.assigniNameForHeader.push({ assigniName: i.assigneeName, assigneeId: i.assigneeId,profilePicture : i.profilePicture});
      });
      this.assigniNameForHeader = this.getUniqueAccounts(this.assigniNameForHeader, "assigneeId")      
      if (this.assigniNameForHeader.length > 5)
      {
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
    this.chartsOfAccountList.forEach(i => {

      if (i.assigneeId === userId) {
        this.UserSpecficchartsOfAccountList.push(this.chartsOfAccountList[i])
      }
    });
    this.chartsOfAccountList = this.UserSpecficchartsOfAccountList
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
    this.getAllAccounts();
  }

  refreshGrid() : void {
    this.getAllAccounts()
    this.notify.success(this.l('Assigni Successfully Updated.'));
  }

}