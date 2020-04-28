import { Component, OnInit, ViewChild, Injector } from '@angular/core';
import { Router } from '@angular/router';
import { ClosingChecklistServiceProxy, ChangeStatusDto, NameValueDto, ChangeAssigneeDto, CategoriesServiceProxy, NameValueDtoOfInt64 } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import { LazyLoadEvent } from 'primeng/api';
import { Paginator } from 'primeng/paginator';
import { Table } from 'primeng/table';
import { UserListComponentComponent } from './user-list-component/user-list-component.component';
import * as moment from 'moment';
import { add, subtract } from 'add-subtract-date';
import { StoreDateService } from "../../services/storedate.service";
import * as $ from 'jquery';

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
  AllOrActive = false;
  advancedFiltersAreShown = false;
  filterText = '';
  titleFilter = '';
  categoryFilter: number = 0;
  statusFilter: number = 0;
  dateFilter: Date = new Date();
  tasksList: any;
  list: any = []
  ClosingCheckList: any = []
  UserSpecficClosingCheckList: any = []
  id: number;
  AssigniInputBox: boolean;
  AssigniBoxView: boolean;
  StatusColorBox: any = ["bg-purple", "bg-golden", "bg-sea-green", "bg-gray"]
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
  rowid: number;
  getTaskWithAssigneeId: number = 0;
  collapsibleRow: boolean
  collapsibleRowId: number;
  updateAssigneeOnHeader: boolean = true
  monthStatus: boolean;
  remainingUserForHeader: any = [];
  category: NameValueDtoOfInt64[] = [];
  selectedDate = new Date();
  constructor(private _router: Router,
    private _categoryService: CategoriesServiceProxy,
    private _closingChecklistService: ClosingChecklistServiceProxy, injector: Injector, private userDate: StoreDateService) {
    super(injector)
    this.FilterBoxOpen = false;
  }
  ngOnInit() {
    // VERSION WITH BOOTSTRAP 4 : https://codepen.io/seltix/pen/XRPrwM

    $(document).ready(function(){
      // Show hide popover
          $(".dropdown-menu").on('click', function (e) {
    e.stopPropagation();
    });
  });


    this.userDate.allUsersInformationofTenant.subscribe(userList => this.users = userList)
    console.log("selected date",this.selectedDate);
    this.initializePageParameters();
    this.loadCategories();
  }
  initializePageParameters(): void {
    this.AssigniInputBox = false;
    this.AssigniBoxView = true;
    this.collapsibleRow = false;
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
  filterByMonth(event) {
    if(event===1){
      this.selectedDate =new Date(add(this.selectedDate, 1, "month"));
   }
   else if(event === -1) {
     this.selectedDate = new Date( subtract(this.selectedDate, 1, "month"));
   }
   else {
     this.selectedDate = new Date(add(event, 2, "day"));
   }
   
   this.getClosingCheckListAllTasks();
  }

  getClosingCheckListAllTasks(event?: LazyLoadEvent) {
    if (this.primengTableHelper.shouldResetPaging(event)) {
      this.paginator.changePage(0);
      return;
    }
    this.dateFilter = this.selectedDate;
    this.primengTableHelper.showLoadingIndicator();
    this._closingChecklistService.getAll(
      this.filterText,
      this.titleFilter,
      this.categoryFilter,
      this.statusFilter,
      moment(this.dateFilter),
      this.getTaskWithAssigneeId,
      this.AllOrActive,
      undefined,
      undefined,
      this.primengTableHelper.getSorting(this.dataTable),
      this.primengTableHelper.getSkipCount(this.paginator, event),
      this.primengTableHelper.getMaxResultCount(this.paginator, event)
    ).subscribe(result => {
      this.primengTableHelper.totalRecordsCount = result.totalCount;
      this.primengTableHelper.records = result.items;
      this.primengTableHelper.hideLoadingIndicator();
      this.list = result.items;
      if(result.items.length>0){
      this.monthStatus = result.items[0].monthStatus;
      }
      this.ClosingCheckList = result.items
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
          i["assigniName"] =  this.users[this.getUserIndex(i.assigneeId)].name;
          i["profilePicture"] =  this.users[this.getUserIndex(i.assigneeId)].profilePicture;
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
        if (this.updateAssigneeOnHeader === true) {
          this.assigniNameForHeader = [];
          this.plusUserBadgeForHeader = false
          this.remainingUserForHeader = [];
          this.assigniNameForHeader = j.overallMonthlyAssignee;
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

  getUserIndex(id) {
    return this.users.findIndex(x => x.id === id);
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
    this._router.navigate(['/app/main/checklist/createtask'], { state: { data: { categoryid: 0, categoryTitle: "" } } });
  }

  redirectToDetail(recordId): void {
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

  changeToggleValue():void{
    if(this.AllOrActive)
    {
      this.AllOrActive = false;
    }
    else{
         this.AllOrActive = true;
    }
    this.getClosingCheckListAllTasks();
    console.log(this.AllOrActive);

  }




}