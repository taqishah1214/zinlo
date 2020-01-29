import { Component, OnInit, ViewChild, Injector } from '@angular/core';
import { Router } from '@angular/router';
import { ClosingChecklistServiceProxy, ChangeStatusDto, NameValueDto } from '@shared/service-proxies/service-proxies';
import { UserListComponentComponent } from '../user-list-component/user-list-component.component';
import { AppComponentBase } from '@shared/common/app-component-base';
import { LazyLoadEvent } from 'primeng/api';
import { Paginator } from 'primeng/paginator';
import { Table } from 'primeng/table';
@Component({
  selector: 'app-tasks',
  templateUrl: './tasks.component.html',
  styleUrls: ['./tasks.component.css']
})
export class TasksComponent extends AppComponentBase implements OnInit {
  @ViewChild(UserListComponentComponent, { static: false }) selectedUserId: UserListComponentComponent;
  @ViewChild('dataTable', { static: true }) dataTable: Table;
  @ViewChild('paginator', { static: true }) paginator: Paginator;
  advancedFiltersAreShown = false;
  filterText = '';
  titleFilter = '';
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
  assigniNameForHeader: any = [];
  newArray: any = [];
  constructor(private _router: Router,
    private _closingChecklistService: ClosingChecklistServiceProxy, injector: Injector) {
    super(injector)
    this.FilterBoxOpen = false;
  }
  ngOnInit() {
    this.AssigniInputBox = false;
    this.AssigniBoxView = true;
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
      this.primengTableHelper.getSorting(this.dataTable),
      this.primengTableHelper.getSkipCount(this.paginator, event),
      this.primengTableHelper.getMaxResultCount(this.paginator, event)
    ).subscribe(result => {
      this.primengTableHelper.totalRecordsCount = result.totalCount;
      this.primengTableHelper.records = result.items;
      this.primengTableHelper.hideLoadingIndicator();
      this.list = result.items;
      this.ClosingCheckList = result.items;
      this.ClosingCheckList.forEach(i => {

        var a = i.closingCheckListForViewDto.assigniName[0].toUpperCase();
        var b = i.closingCheckListForViewDto.assigniName.substr(i.closingCheckListForViewDto.assigniName.indexOf(' ') + 1)[0].toUpperCase();
        i.closingCheckListForViewDto["NameAvatar"] = a + b;
        if (i.closingCheckListForViewDto.status === "Inprogress") {
          i.closingCheckListForViewDto["StatusColor"] = this.StatusColorBox[0]
        }
        else if (i.closingCheckListForViewDto.status === "Open") {
          i.closingCheckListForViewDto["StatusColor"] = this.StatusColorBox[1]
        }
        else if (i.closingCheckListForViewDto.status === "Complete") {
          i.closingCheckListForViewDto["StatusColor"] = this.StatusColorBox[2]
        }
        this.assigniNameForHeader.push({ nameAvatar: i.closingCheckListForViewDto.NameAvatar, assigneeId: i.closingCheckListForViewDto.assigneeId });

      });

      this.assigniNameForHeader = this.getUnique(this.assigniNameForHeader, "assigneeId")
    });
  }

  //End
  GetUserTasks(userId) {
    debugger;
    this.ClosingCheckList.forEach(i => {

      if (i.closingCheckListForViewDto.assigneeId === userId) {
        this.UserSpecficClosingCheckList.push(this.ClosingCheckList[i])
      }
      else {
        console.log("not match")
      }

      debugger;
    });
    this.ClosingCheckList = this.UserSpecficClosingCheckList
    debugger;
  }

  getUnique(arr, comp) {

    const unique = arr
      .map(e => e[comp])

      // store the keys of the unique objects
      .map((e, i, final) => final.indexOf(e) === i && i)

      // eliminate the dead keys & store unique objects
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
    });
  }

  RedirectToCreateTask(): void {
    this._router.navigate(['/app/main/TasksCheckList/create-or-edit-task']);
  }

  RedirectToDetail(recordId): void {
    this._router.navigate(['/app/main/TasksCheckList/task-details'], { state: { data: { id: recordId } } });

  }



}
