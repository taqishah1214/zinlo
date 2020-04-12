import { Component, OnInit, ViewChild, Injector } from '@angular/core';
import { Router } from '@angular/router';
import { AccountSubTypeServiceProxy, ChartsofAccountServiceProxy } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import { LazyLoadEvent } from 'primeng/api';
import { Paginator } from 'primeng/paginator';
import { Table } from 'primeng/table';
import { UserListComponentComponent } from '../checklist/user-list-component/user-list-component.component';
import { UppyConfig } from 'uppy-angular';

@Component({
  selector: 'app-reconcilliation',
  templateUrl: './reconcilliation.component.html',
  styleUrls: ['./reconcilliation.component.css']
})

export class ReconcilliationComponent extends AppComponentBase implements OnInit {
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
  accountTypeList: Array<{ id: number, name: string }> = [{ id: 1, name: "Equity" }, { id: 2, name: "Assets" }, { id: 3, name: "Liability" }];
  reconcillationTypeList: Array<{ id: number, name: string }> = [{ id: 1, name: "Itemized" }, { id: 2, name: "Amortized" }]
  StatusColorBox: any = ["bg-blue", "bg-sea-green", "bg-gray"]
  updateAssigneeOnHeader: boolean = true;
  currentDate :any 
  getAccountWithAssigneeId : number = 0;
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
    this.currentDate = new Date();
    this.loadAccountSubType();
  }

  // accountTypeClick(id,name) : void {
  //   this.accountType = name
  //   this.accountTypeFilter = id
  //   this.updateAssigneeOnHeader = false
  //   this.getAllAccounts()
  // }
  accountTypeClick(event): void {
    
      this.accountTypeFilter = parseInt(event.target.value)
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
      false,
      this.primengTableHelper.getSorting(this.dataTable),
      this.primengTableHelper.getSkipCount(this.paginator, event),
      this.primengTableHelper.getMaxResultCount(this.paginator, event)
    ).subscribe(result => {
      this.plusUserBadgeForHeader = false;
      this.primengTableHelper.totalRecordsCount = result.totalCount;
      this.primengTableHelper.records = result.items;
      this.primengTableHelper.hideLoadingIndicator();
      this.list = result.items;
      this.chartsOfAccountList = result.items
      this.chartsOfAccountList.forEach(i => {
        i["accountType"] =  this.getNameofAccountTypeAndReconcillation(i.accountTypeId,"accountType")
        i["reconciliationType"] =   this.getNameofAccountTypeAndReconcillation(i.reconciliationTypeId,"reconcillation")           
        i["statusName"] = this.getStatusNameWithId(i.statusId)  
        i["statusBoxColor"] = this.getStatusBoxColor(i.statusId) 
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

  ChangeStatus( selectedStatusId, accountId) {
    this._chartOfAccountService.changeStatus(accountId,selectedStatusId).subscribe(result => {
      this.ResetGrid();
      this.notify.success(this.l('Status Successfully Updated.'));

    })
  }


  getStatusBoxColor(id) : string {
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

  getStatusNameWithId(id): string {
    if (id == 1) {
      return "In Process"
    }
    else if (id == 2) {
      return "Open"
    }
    else if (id == 3) {
      return "Complete"
    }
  }

  reDirectToItemizedAmotized (reconciliationTypeId,accountId,accountNo,accountName){
  if (reconciliationTypeId == 1)
  {
    this._router.navigate(['/app/main/reconcilliation/itemized'],{ state: { data: { accountId : accountId, accountName :accountName ,accountNo: accountNo  }} });

  }
  else if (reconciliationTypeId == 2) {
    this._router.navigate(['/app/main/reconcilliation/amortized'],{ state: { data: { accountId : accountId , accountName :accountName ,accountNo: accountNo}} });
  }
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
  

  GetAssigniSpecficAccounts(userId) {
    this.getAccountWithAssigneeId = userId;
    this.getAllAccounts();
  }

  
  loadAccountSubType(): void {
    this._accountSubTypeService.accountSubTypeDropDown().subscribe(result => {
    this.accountSubTypes = result;
    });
  }
  ResetGrid(): void {
    this.accountType = "Account Type"
    this.accountTypeFilter = 0
    this.getAccountWithAssigneeId = 0;
    this.updateAssigneeOnHeader = true
    this.getAllAccounts();
  }

  refreshGrid() : void {
    this.updateAssigneeOnHeader = true
    this.getAccountWithAssigneeId = 0;
    this.getAllAccounts()
    this.notify.success(this.l('Assigni Successfully Updated.'));
  }

}