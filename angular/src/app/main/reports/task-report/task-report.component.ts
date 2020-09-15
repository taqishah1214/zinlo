import { Component, OnInit, ViewChild, Injector, ElementRef } from '@angular/core';
import { Router } from '@angular/router';
import { ClosingChecklistServiceProxy, ChangeStatusDto, NameValueDto, ChangeAssigneeDto, CategoriesServiceProxy, NameValueDtoOfInt64 } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import { LazyLoadEvent } from 'primeng/api';
import { Paginator } from 'primeng/paginator';
import { Table } from 'primeng/table';
import * as moment from 'moment';
import { StoreDateService } from "../../../services/storedate.service";
import { FileDownloadService } from '@shared/utils/file-download.service';

@Component({
  selector: 'app-task-report',
  templateUrl: './task-report.component.html',
  styleUrls: ['./task-report.component.css']
})
export class TaskReportComponent extends AppComponentBase implements OnInit {
  @ViewChild('dataTable', { static: true }) dataTable: Table;
  @ViewChild('paginator', { static: true }) paginator: Paginator;
  @ViewChild("dateRangePicker", { static: true }) dateRangePickerElement: ElementRef;
  selectedDateRange: moment.Moment[] = [moment().add(-1, 'month').startOf('day'), moment().endOf('day')];
  filterStatus: number = 0;
  advancedFiltersAreShown = false;
  filterText = '';

  categoryFilter: number = 0;
  statusFilter: number = 1;
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
  constructor(private _router: Router,
    private _categoryService: CategoriesServiceProxy,
    private _closingChecklistService: ClosingChecklistServiceProxy,
    private _fileDownloadService: FileDownloadService, injector: Injector, private userDate: StoreDateService) {
    super(injector)
    this.FilterBoxOpen = false;
  }
  ngOnInit() {
    this.userDate.allUsersInformationofTenant.subscribe(userList => this.users = userList)
    this.initializePageParameters();
    this.loadCategories();
  }
  initializePageParameters(): void {
    this.AssigniInputBox = false;
    this.AssigniBoxView = true;
    this.collapsibleRow = false;
  }
   exportToExcel(): void {
        this._closingChecklistService.getTaskToExcel(
          this.filterText,
          this.categoryFilter,
          this.statusFilter,
          this.getTaskWithAssigneeId,
          this.selectedDateRange[0],
          this.selectedDateRange[1],
          this.primengTableHelper.getSorting(this.dataTable)).subscribe(result => {
                  this._fileDownloadService.downloadTempFile(result);
            });
    }
  onChange() {
    abp.event.trigger('app.dashboardFilters.dateRangePicker.onDateChange', this.selectedDateRange);
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
   this.getClosingCheckListAllTasks();
  }

  getClosingCheckListAllTasks(event?: LazyLoadEvent) {
    if (this.primengTableHelper.shouldResetPaging(event)) {
      this.paginator.changePage(0);
      return;
    }
    this.primengTableHelper.showLoadingIndicator();
    this._closingChecklistService.getReport(
      this.filterText,
      this.categoryFilter,
      this.statusFilter,
      this.getTaskWithAssigneeId,
      this.selectedDateRange[0],
      this.selectedDateRange[1],
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


  loadCategories(): void {
    this._categoryService.categoryDropDown().subscribe(result => {
      this.category = result;
    });
  }
  ResetGrid(): void {
    this.statusFilter = 0;
    this.categoryFilter = 0;  
    this.filterStatus = 0;
    this.getTaskWithAssigneeId = 0;
    this.updateAssigneeOnHeader = true;
    this.getClosingCheckListAllTasks();
  }
}