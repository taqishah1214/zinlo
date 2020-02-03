import { Component, OnInit, ViewChild, Injector } from '@angular/core';
import { Router } from '@angular/router';
import { ClosingChecklistServiceProxy, ChangeStatusDto, NameValueDto ,ChangeAssigneeDto} from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import { LazyLoadEvent } from 'primeng/api';
import { Paginator } from 'primeng/paginator';
import { Table } from 'primeng/table';
import { UserListComponentComponent } from './user-list-component/user-list-component.component';
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
  changeAssigniDto : ChangeAssigneeDto = new ChangeAssigneeDto();
  assigniNameForHeader: any = [];
  newArray: any = [];
  assigneeId: any;
  users: any;
  updatedAssigneeId: any;
  getTaskForEdit: void;
  userName: string[];
  plusUserBadgeForHeader : boolean;
  taskId;
  text;
  rowid: number;
  remainingUserForHeader : any = [];
  constructor(private _router: Router,
    private _closingChecklistService: ClosingChecklistServiceProxy, injector: Injector) {
    super(injector)
    this.FilterBoxOpen = false;
  }
  ngOnInit() {
    this.AssigniInputBox = false;
    this.AssigniBoxView = true;

    
  }
  openFieldUpdateAssignee(record) {
    this.rowid = record;
  }
  addInput(i) {
    this.rowid =0;
    i.closingCheckListForViewDto.assigniName = i.text;
    i.closingCheckListForViewDto.assigneeId = this.assigneeId;
    this.openFieldUpdateAssignee;
    if (this.users) {
      this.assigneeId = this.users.filter(a => {
        if (a.name == i.closingCheckListForViewDto.assigniName) {
          return a.value;
        }
      });
      this.taskId = i.closingCheckListForViewDto.id;
      this.updatedAssigneeId = this.assigneeId[0].value;

      this.changeAssigniDto.assigneeId = this.updatedAssigneeId;
      this.changeAssigniDto.taskId = this.taskId;
      this._closingChecklistService.changeAssignee(this.changeAssigniDto).subscribe(result => {
      this.getTaskForEdit = result;
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

        var firstCharacterForAvatar = i.closingCheckListForViewDto.assigniName[0].toUpperCase();
        var lastCharacterForAvatar = i.closingCheckListForViewDto.assigniName.substr(i.closingCheckListForViewDto.assigniName.indexOf(' ') + 1)[0].toUpperCase();
        i.closingCheckListForViewDto["NameAvatar"] = firstCharacterForAvatar + lastCharacterForAvatar;
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
      if (this.assigniNameForHeader.length > 5)
      {
        var limitedUserNameForHeader = [];
        for (var i = 0 ; i < 5 ; i++)
        {
          limitedUserNameForHeader.push(this.assigniNameForHeader[i])

        }
        for (let index = 5; index < this.assigniNameForHeader.length; index++) {
          this.remainingUserForHeader.push(this.assigniNameForHeader[index])   
        }
       this.assigniNameForHeader =  limitedUserNameForHeader

        debugger
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
        console.log("not match")
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
    });
  }

  RedirectToCreateTask(): void {
    this._router.navigate(['/app/main/checklist/create-or-edit-task']);
  }

  RedirectToDetail(recordId): void {
    this._router.navigate(['/app/main/checklist/task-details'], { state: { data: { id: recordId } } });

  }



}