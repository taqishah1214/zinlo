import { Component, OnInit, ViewChild, Injector } from '@angular/core';
import { Router } from '@angular/router';
import { ClosingChecklistServiceProxy, ChangeStatusDto, NameValueDto, ChangeAssigneeDto, CategoriesServiceProxy, NameValueDtoOfInt64, AccountSubTypeServiceProxy, ChartsofAccountServiceProxy } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import { LazyLoadEvent } from 'primeng/api';
import { Paginator } from 'primeng/paginator';
import { Table } from 'primeng/table';
import { UserListComponentComponent } from '../../checklist/user-list-component/user-list-component.component';
import * as moment from 'moment';

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
  dateFilter: Date = new Date(2000, 0O5, 0O5, 17, 23, 42, 11);
  monthFilter = '100/2000';
  monthValue:Date = new Date();
  minDate:Date;
  maxDate:Date;               
  tasksList: any;
  list: any = []
  ClosingCheckList: any = []
  UserSpecficClosingCheckList: any = []
  id: number;
  AssigniInputBox: boolean;
  AssigniBoxView: boolean;
  StatusColorBox: any = ["bg-purple", "bg-golden", "bg-sea-green","bg-magenta"]
  FilterBoxOpen: boolean;
  public rowId: number = 0;
  changeStatus: ChangeStatusDto = new ChangeStatusDto();
  changeAssigniDto: ChangeAssigneeDto = new ChangeAssigneeDto();
  assigniNameForHeader: any = [];
  newArray: any = [];
  assigneeId: any;
  users: any;
  updatedAssigneeId: any;
  getTaskForEdit: void;
  userName: string[];
  plusUserBadgeForHeader: boolean;
  taskId;
  text;
  currentDate: Date;
  currentMonth: string
  currentYear: Number;
  rowid: number;
  yearCount : number;
  collapsibleRow : boolean
  collapsibleRowId : number;
  monthCount : number =1;
  chartsOfAccountList : any = [] 
  monthsArray = new Array("January", "February", "March",
    "April", "May", "June", "July", "August", "September",
    "October", "Novemeber", "December");


  remainingUserForHeader: any = [];
  accountSubTypes: any = [];
  accountTypeList: Array<{ id: number, name: string }> = [{ id: 1, name: "Fixed" }, { id: 2, name: "Assets" }, { id: 3, name: "Liability" }];
  reconcillationTypeList: Array<{ id: number, name: string }> = [{ id: 1, name: "Itemized" }, { id: 2, name: "Amortization" }]
  
  constructor(private _router: Router,
    private _accountSubTypeService: AccountSubTypeServiceProxy,
    private _closingChecklistService: ClosingChecklistServiceProxy, injector: Injector,
    private _chartOfAccountService: ChartsofAccountServiceProxy) {
    super(injector)
    this.FilterBoxOpen = false;
  }
  ngOnInit() {
    this.AssigniInputBox = false;
    this.AssigniBoxView = true;
    this.collapsibleRow = false
    this.loadAccountSubType();
  }
  openFieldUpdateAssignee(record) {
    this.rowid = record;
  }
  collapsibleRowClick(id) {
    console.log("idddd",id);
    this.collapsibleRowId = id;
    this.collapsibleRow = !this.collapsibleRow;
  }

  addInput(i) {
    this.rowid = 0;
    i.assigniName = i.text;
    i.assigneeId = this.assigneeId;
    this.openFieldUpdateAssignee;
    if (this.users) {
      this.assigneeId = this.users.filter(a => {
        if (a.name == i.assigniName) {
          return a.value;
        }
      });
      this.taskId = i.id;
      this.updatedAssigneeId = this.assigneeId[0].value;
      this.changeAssigniDto.assigneeId = this.updatedAssigneeId;
      this.changeAssigniDto.taskId = this.taskId;
      this._closingChecklistService.changeAssignee(this.changeAssigniDto).subscribe(result => {
        this.getTaskForEdit = result;
        this.getClosingCheckListAllTasks();
      });
    }
  }

  onSearchUsers(event): void {
    this._closingChecklistService.userAutoFill(event.query).subscribe(result => {
      this.users = result;
      this.userName = result.map(a => a.name);
    });

  }
  //Start
  getClosingCheckListAllTasks(event?: LazyLoadEvent) {
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
      this.assigniNameForHeader = this.getUnique(this.assigniNameForHeader, "assigneeId")      
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

  //End
  GetUserTasks(userId) {
    this.ClosingCheckList.forEach(i => {

      if (i.closingCheckListForViewDto.assigneeId === userId) {
        this.UserSpecficClosingCheckList.push(this.ClosingCheckList[i])
      }
    });
    this.ClosingCheckList = this.UserSpecficClosingCheckList
  }

  getUnique(arr, comp) {

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
  ChangeStatus(statusId, TaskId): void {
    this.changeStatus.statusId = statusId;
    this.changeStatus.taskId = TaskId;
    this._closingChecklistService.changeStatus(this.changeStatus).subscribe(result => {
      this.notify.success(this.l("Status Successfully Changed"));
      this.getClosingCheckListAllTasks();
    });
  }
  

  
  loadAccountSubType(): void {
    this._accountSubTypeService.accountSubTypeDropDown().subscribe(result => {
    this.accountSubTypes = result;
    });
  }
  ResetGrid(): void {
    this.statusFilter = 0;
    this.categoryFilter = 0;
    this.dateFilter = new Date(2000, 0O5, 0O5, 17, 23, 42, 11);
    this.titleFilter = '';
    this.getClosingCheckListAllTasks();
  }

}