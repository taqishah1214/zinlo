import { Component, OnInit, Injector, Output, ViewChild } from '@angular/core';
import { CreateOrEditClosingChecklistDto, ClosingChecklistServiceProxy, GetTaskForEditDto, CategoriesServiceProxy, ChangeStatusDto } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import { UppyConfig } from 'uppy-angular';

import * as moment from 'moment';
import { UserListComponentComponent } from '../user-list-component/user-list-component.component';
import { Router } from '@angular/router';


@Component({
  selector: 'app-edit-task',
  templateUrl: './edit-task.component.html',
  styleUrls: ['./edit-task.component.css']
})
export class EditTaskComponent extends AppComponentBase implements OnInit {
  parentassigneName;
  SelectedCategory;
  checklist: CreateOrEditClosingChecklistDto = new CreateOrEditClosingChecklistDto()
  commantBox: boolean
  userSignInName: string
  taskId: any;
  closingMonth: Date
  getTaskForEdit: GetTaskForEditDto = new GetTaskForEditDto();
  _changeStatus: ChangeStatusDto = new ChangeStatusDto();
  assigniName;
  assignee;
  category;
  users;
  frequency;
  frequencyId: string;
  categoryId: number;
  comment;
  statusValue: any;
  public closingMonthValue: Date;
  createOrEdit: void;
  userName: string[];
  assigneeId: any;
  status: number;
  public userId: number;
  @ViewChild(UserListComponentComponent, { static: false }) selectedUserId: UserListComponentComponent;

  constructor(private _categoryService: CategoriesServiceProxy, injector: Injector, private _closingChecklistService: ClosingChecklistServiceProxy,private _router: Router) {
    super(injector)

  }

  ngOnInit() {
    this.taskId = history.state.data.id;
    this.commantBox = false;
    this.userSignInName = this.appSession.user.name.toString().charAt(0).toUpperCase();
    this.taskId = history.state.data.id;
    this._closingChecklistService.getTaskForEdit(this.taskId).subscribe(result => {
      this.getTaskForEdit = result;
      console.log("gettaskforedit", this.getTaskForEdit)
      this.closingMonthValue = this.getTaskForEdit.closingMonth.toDate();


      this.frequencyId = this.getTaskForEdit.frequency;
      this.getTaskForEdit.closingMonth = moment().startOf('day');
      this.getTaskForEdit.endsOn = moment().startOf('day');
      this.assigneeId = this.getTaskForEdit.assigneeId;
      this.parentassigneName = result;
      this.categoryId = this.getTaskForEdit.categoryId;
      this.comment = this.getTaskForEdit.comments;

      this.comment.forEach(i => {
        i.userName;
        var a = i.userName[0].toUpperCase();
        var b = i.userName.substr(i.userName.indexOf(' ') + 1)[0].toUpperCase();
        i["NameAvatar"] = a + b;
      });
    });
    this._categoryService.categoryDropDown().subscribe(result => {
      this.category = result;
    });


  }

  ChangeStatus(value): void {
    if (value === 1) {
      this.status = 1;
      this.getTaskForEdit.status = "Open";
      this.checklist.status = 1;

    }
    if (value === 2) {
      this.status = 2;
      this.getTaskForEdit.status = "Complete"
      this.checklist.status = 2;
    }
    if (value === 3) {
      this.status = 3;
      this.getTaskForEdit.status = "Inprogress"
      this.checklist.status = 3;
    }

  }


  onCreateTask() {
    this.checklist.id = this.getTaskForEdit.id;

  }
  commentClick() {
    this.commantBox = true;
  }
  onCancelComment() {
    this.commantBox = false;

  }
  onComment() {
    this.commantBox = false;
    this.checklist.commentBody;

  }

  onUpdateTask() {
    this.checklist.frequency = this.getTaskForEdit.frequencyId;
    this.checklist.closingMonth = this.getTaskForEdit.closingMonth;
    this.checklist.endsOn = this.getTaskForEdit.endsOn;
    this.checklist.status = 1;
    this.checklist.categoryId = this.getTaskForEdit.categoryId;
    this.checklist.dueOn = this.getTaskForEdit.dueOn;
    this.checklist.endOfMonth = this.getTaskForEdit.endOfMonth;
    this.checklist.noOfMonths = this.getTaskForEdit.noOfMonths;
    this.checklist.taskName = this.getTaskForEdit.taskName;
    this.checklist.instruction = this.getTaskForEdit.instruction;
    this.checklist.assigneeId = Number(this.selectedUserId.selectedUserId.value);
    this.checklist.id = this.taskId;
    this.checklist.comments = [];
    this._closingChecklistService.createOrEdit(this.checklist).subscribe(result => {

      this.notify.success(this.l('SavedSuccessfully Updated'));
      this._router.navigate(['/app/main/TasksCheckList/tasks']);
    });
  }

  settings: UppyConfig = {
    uploadAPI: {
      endpoint: ``,
      headers: {
        Authorization: 'Bearer ' + localStorage.getItem('userToken')
      }
    },
    plugins: {
      Webcam: false
    }
  }

}
