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
  advancedFiltersAreShown = false;
  filterText = '';
  titleFilter = '';
  categoryFilter: number = 0;
  statusFilter: number = 0;
  dateFilter: Date = new Date(2000, 0O5, 0O5, 17, 23, 42, 11);
  monthFilter = '00/0000';
  tasksList: any;
  list: any = []
  ClosingCheckList: any = []
  UserSpecficClosingCheckList: any = []
  id: number;
  AssigniInputBox: boolean;
  AssigniBoxView: boolean;
  StatusColorBox: any = ["bg-purple", "bg-golden", "bg-sea-green"]
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
  yearCount: number;
  monthCount: number = 1;
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
    this.AssigniInputBox = false;
    this.AssigniBoxView = true;
    this.currentDate = new Date();
    this.currentYear = this.currentDate.getFullYear()
    var curr_month = this.currentDate.getMonth();
    this.monthCount = curr_month;
    this.currentMonth = this.monthsArray[curr_month]
    this.yearCount = 1;
    this.loadCategories();
  }
  openFieldUpdateAssignee(record) {
    this.rowid = record;
  }

  onOpenCalendar(container) {
    container.monthSelectHandler = (event: any): void => {
      container._store.dispatch(container._actions.select(event.date));
    };
    container.setViewMode('month');
  }
  monthChangeHeader(operation): void {

    if (this.monthCount == -1) {
      this.monthCount = 12;
      this.currentYear = this.currentDate.getFullYear() - (Math.abs(this.yearCount));
      --this.yearCount;

    }
    if (operation == 1) {
      debugger
      this.currentMonth = this.monthsArray[this.monthCount];
      console.log("logged date change by ihsan",this.currentMonth);
      console.log("Current year",this.currentYear);
      var index = this.monthsArray.indexOf(this.currentMonth);
      this.monthFilter = index + "/"+ this.currentYear;
      this.getClosingCheckListAllTasks();
      ++this.monthCount
    }
    else {
      debugger
      if (this.monthCount == 0) {
        this.monthCount = 12;
        this.currentYear = this.currentDate.getFullYear() - (Math.abs(this.yearCount));
        --this.yearCount;
      }
      this.currentMonth = this.monthsArray[this.monthCount]
      --this.monthCount

    }
    if (this.monthCount == 12) {
      this.monthCount = 0;
      this.currentYear = this.currentDate.getFullYear() + this.yearCount;
      ++this.yearCount;

    }

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
      this.primengTableHelper.getSorting(this.dataTable),
      this.primengTableHelper.getSkipCount(this.paginator, event),
      this.primengTableHelper.getMaxResultCount(this.paginator, event)
    ).subscribe(result => {
      this.primengTableHelper.totalRecordsCount = result.totalCount;
      this.primengTableHelper.records = result.items;
      this.primengTableHelper.hideLoadingIndicator();
      this.list = result.items;
      this.ClosingCheckList = result.items
      this.ClosingCheckList.forEach(j => {
        j.group.forEach(i => {
          var firstCharacterForAvatar = i.assigniName[0].toUpperCase();
          var lastCharacterForAvatar = i.assigniName.substr(i.assigniName.indexOf(' ') + 1)[0].toUpperCase();
          i["NameAvatar"] = firstCharacterForAvatar + lastCharacterForAvatar;
          if (i.status === "NotStarted") {
            i["StatusColor"] = this.StatusColorBox[0]
          }
          else if (i.status === "InProcess") {
            i["StatusColor"] = this.StatusColorBox[1]
          }
          else if (i.status === "OnHold") {
            i["StatusColor"] = this.StatusColorBox[2]
          }
          else if (i.status === "Completed") {
            i["StatusColor"] = this.StatusColorBox[2]
          }
          this.assigniNameForHeader.push({ nameAvatar: i.NameAvatar, assigneeId: i.assigneeId });
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
      });

      this.assigniNameForHeader = this.getUnique(this.assigniNameForHeader, "assigneeId")
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
    });
  }
  //End
  GetUserTasks(userId) {
    this.ClosingCheckList.forEach(i => {

      if (i.closingCheckListForViewDto.assigneeId === userId) {
        this.UserSpecficClosingCheckList.push(this.ClosingCheckList[i])
      }
      else {

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
  RedirectToCreateTask(): void {
    this._router.navigate(['/app/main/checklist/createtask']);
  }

  RedirectToDetail(recordId): void {
    this._router.navigate(['/app/main/checklist/task-details'], { state: { data: { id: recordId } } });

  }
  //Filters
  filterByStatus(event): void {
    this.statusFilter = event.target.value.toString();
    this.getClosingCheckListAllTasks();
  }
  filterByCategory(event): void {
    this.categoryFilter = event.target.value.toString();
    this.getClosingCheckListAllTasks();
  }
  filterByDate(event): void {
    this.dateFilter = event;
    console.log("datedd", event);
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
    this.getClosingCheckListAllTasks();
  }

}