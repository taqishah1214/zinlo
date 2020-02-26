import { Component, OnInit, ViewChild, Injector } from '@angular/core';
import { Router } from '@angular/router';
import { ClosingChecklistServiceProxy, ChangeStatusDto, NameValueDto, ChangeAssigneeDto, CategoriesServiceProxy, NameValueDtoOfInt64 } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import { LazyLoadEvent } from 'primeng/api';
import { Paginator } from 'primeng/paginator';
import { Table } from 'primeng/table';
import { UserListComponentComponent } from './user-list-component/user-list-component.component';
import * as moment from 'moment';
@Component({
  selector: 'app-tasks',
  templateUrl: './checklist.component.html',
  styleUrls: ['./checklist.component.css']
})
export class Checklist extends AppComponentBase implements OnInit {
  @ViewChild(UserListComponentComponent, { static: false }) selectedUserId: UserListComponentComponent;
  @ViewChild('dataTable', { static: true }) dataTable: Table;
  @ViewChild('paginator', { static: true }) paginator: Paginator;
  filterStatus: number = 0;
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
  StatusColorBox: any = ["bg-purple", "bg-golden", "bg-sea-green","bg-gray"]
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
  getTaskWithAssigneeId : number = 0;
  yearCount : number;
  collapsibleRow : boolean
  collapsibleRowId : number;
  monthCount : number =1;
  updateAssigneeOnHeader : boolean = true
  monthsArray = new Array("January", "February", "March",
    "April", "May", "June", "July", "August", "September",
    "October", "Novemeber", "December");
  remainingUserForHeader: any = [];
  category: NameValueDtoOfInt64[] = [];
  constructor(private _router: Router,
    private _categoryService: CategoriesServiceProxy,
    private _closingChecklistService: ClosingChecklistServiceProxy, injector: Injector) {
    super(injector)
    this.FilterBoxOpen = false;   
  }
  ngOnInit() {
    this.initializePageParameters();
    this.loadCategories();
  }
  initializePageParameters():void{
    this.minDate = new Date(new Date().getFullYear(), 0O0, 0O5, 17, 23, 42, 11);
    this.maxDate = new Date(new Date().getFullYear(), 0O11, 0O5, 17, 23, 42, 11);
    this.AssigniInputBox = false;
    this.AssigniBoxView = true;
    this.currentDate = new Date();
    this.currentYear = this.currentDate.getFullYear()
    var curr_month = this.currentDate.getMonth();
    this.monthCount = curr_month;
    this.currentMonth = this.monthsArray[curr_month]
    this.yearCount = 1;
    this.collapsibleRow = false;
    this.currentDate = new Date();
  }

  openFieldUpdateAssignee(record) {
    this.rowid = record;
  }
  collapsibleRowClick(id) {
    this.collapsibleRowId = id;
    this.collapsibleRow = !this.collapsibleRow;
  }

  onOpenCalendar(container) {
    container.monthSelectHandler = (event: any): void => {
      container._store.dispatch(container._actions.select(event.date));
    };
    container.setViewMode('month');
  }

  calculateDate(preNext):void{
    this.updateAssigneeOnHeader = true
    if(preNext == 1)
    {
       var month = this.currentDate.getMonth();
       this.currentMonth = this.monthsArray[month];
       var year = this.currentDate.getFullYear();
       this.currentYear = year;
       if(month != 11)
       {
         this.currentDate.setMonth(month + 1);
         var month1 = this.currentDate.getMonth();
         this.currentMonth =this.monthsArray[month1];
         var index = this.monthsArray.indexOf(this.currentMonth)+ 1;
         this.monthFilter = index + "/"+ this.currentYear;
         this.monthValue = null;
         this.monthValue = new Date(this.currentDate.getFullYear(),this.currentDate.getMonth())
         this.getClosingCheckListAllTasks();
       }
       else if(month == 11)
       {
        var year = this.currentDate.getFullYear();
          this.currentDate.setFullYear(year + 1);
         this.currentDate.setMonth(0);
        var year1 = this.currentDate.getFullYear();
        this.currentYear = year1;
         this.currentMonth =this.monthsArray[0];
         var index = this.monthsArray.indexOf(this.currentMonth)+ 1;
         this.monthFilter = index + "/"+ this.currentYear;
         this.monthValue = null;
         this.monthValue = new Date(this.currentDate.getFullYear(),this.currentDate.getMonth())
         this.getClosingCheckListAllTasks();
       }
    }
    else if(preNext == -1)
    {
      var monthIndex = this.currentDate.getMonth();
       this.currentMonth = this.monthsArray[monthIndex];
       var year = this.currentDate.getFullYear();
       this.currentYear = year;
       if(monthIndex != 0)
       {
         this.currentDate.setMonth(monthIndex - 1);
         var month1 = this.currentDate.getMonth();
         this.currentMonth =this.monthsArray[month1];
         var index = this.monthsArray.indexOf(this.currentMonth)+ 1;
         this.monthFilter = index + "/"+ this.currentYear;
         this.monthValue = null;
         this.monthValue = new Date(this.currentDate.getFullYear(),this.currentDate.getMonth())
         this.getClosingCheckListAllTasks();
       }
       else if(monthIndex == 0 )
       {
        var year = this.currentDate.getFullYear();
          this.currentDate.setFullYear(year - 1);
         this.currentDate.setMonth(11);
        var year1 = this.currentDate.getFullYear();
        this.currentYear = year1;
         this.currentMonth =this.monthsArray[11];
         var index = this.monthsArray.indexOf(this.currentMonth) + 1;
         this.monthFilter = index + "/"+ this.currentYear;
         this.monthValue = null;
         this.monthValue = new Date(this.currentDate.getFullYear(),this.currentDate.getMonth())
         this.getClosingCheckListAllTasks();
       }

    }
  }
  getClosingCheckListAllTasks(event?: LazyLoadEvent) {
    if (this.primengTableHelper.shouldResetPaging(event)) {
      this.paginator.changePage(0);
      return;
    }

    this.primengTableHelper.showLoadingIndicator();
    this._closingChecklistService.getAll(
      this.filterText,
      this.titleFilter,
      this.categoryFilter,
      this.statusFilter,
      moment(this.dateFilter),
      this.monthFilter,
      this.getTaskWithAssigneeId,
      this.primengTableHelper.getSorting(this.dataTable),
      this.primengTableHelper.getSkipCount(this.paginator, event),
      this.primengTableHelper.getMaxResultCount(this.paginator, event)
    ).subscribe(result => {
      this.primengTableHelper.totalRecordsCount = result.totalCount;
      this.primengTableHelper.records = result.items;
      this.primengTableHelper.hideLoadingIndicator();
      this.list = result.items;
      this.ClosingCheckList = result.items
      if (this.ClosingCheckList.length==0) {
        this.assigniNameForHeader = [];
        this.remainingUserForHeader = [];
        this.updateAssigneeOnHeader = true
      }
      this.ClosingCheckList.forEach(j => {
        j.group.forEach(i => {
          if (i.status === "NotStarted") {
            i["StatusColor"] = this.StatusColorBox[3]
          }
          else if (i.status === "InProcess") {
            i["StatusColor"] = this.StatusColorBox[0]
          }
          else if (i.status === "OnHold") {
            i["StatusColor"] = this.StatusColorBox[1]
          }
          else if (i.status === "Completed") {
            i["StatusColor"] = this.StatusColorBox[2]
          }
         
            if (i.statusId == 1) {
              i.status = "Not Started";
            }
            else if (i.statusId == 2) {
              i.status = "In Process";
            }
            if (i.statusId == 3) {
              i.status = "On Hold";
            }
            else if (i.statusId == 4) {
              i.status = "Completed";
            }
        });
        if (this.updateAssigneeOnHeader === true)
        {
          this.assigniNameForHeader = [];
          this.plusUserBadgeForHeader = false
          this.remainingUserForHeader = [];
          this.assigniNameForHeader = j.overallMonthlyAssignee; 
        }
      });
      if (this.updateAssigneeOnHeader === true)
      {
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
      this.updateAssigneeOnHeader = false;
    }
      
    });
  }
  GetUserTasks(id) {
    this.getTaskWithAssigneeId = id;
    this.getClosingCheckListAllTasks();
  }

  getUniqueNamesByAssignee(arr, comp) {
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
  RedirectToCreateTask(): void {
    this._router.navigate(['/app/main/checklist/createtask']);
  }

  RedirectToDetail(recordId): void {
    this._router.navigate(['/app/main/checklist/task-details'], { state: { data: { id: recordId } } });

  }
  filterByStatus(event): void {
    this.statusFilter = event.target.value.toString();
    this.updateAssigneeOnHeader = false;
    this.getClosingCheckListAllTasks();
  }
  filterByCategory(event): void {
    this.categoryFilter = event.target.value.toString();
    this.updateAssigneeOnHeader = false;
    this.getClosingCheckListAllTasks();
  }
  filterByDate(event): void {
    this.dateFilter = event;
    this.updateAssigneeOnHeader = false;
    this.getClosingCheckListAllTasks();
  }
  filterByMonth(event):void{
   this.updateAssigneeOnHeader = true
   var month =  event.getMonth() + 1;
   this.monthFilter = month +"/"+ event.getFullYear()
   this.getClosingCheckListAllTasks();
  }
  loadCategories(): void {
    this._categoryService.categoryDropDown().subscribe(result => {
      this.category = result;
    });
  }
  ResetGrid(): void {
    this.statusFilter = 0;
    this.categoryFilter = 0;
    this.dateFilter = new Date(2000, 0O5, 0O5, 17, 23, 42, 11);
    this.titleFilter = '';
    this.filterStatus = 0;
    this.getTaskWithAssigneeId = 0;
    this.updateAssigneeOnHeader = true;
    this.getClosingCheckListAllTasks();
  }

}