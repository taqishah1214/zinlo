
import { Component, OnInit, Injector, Output, ViewChild } from '@angular/core';
import { CreateOrEditClosingChecklistDto, ClosingChecklistServiceProxy, GetTaskForEditDto, CategoriesServiceProxy, ChangeStatusDto, PostAttachmentsPathDto } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import { UppyConfig } from 'uppy-angular';

import * as moment from 'moment';
import { UserListComponentComponent } from '../checklist/user-list-component/user-list-component.component';
import { Router } from '@angular/router';


@Component({
  selector: 'app-duplicate-task',
  templateUrl: './duplicate-task.component.html',
  styleUrls: ['./duplicate-task.component.css']
})
export class DuplicateTaskComponent extends AppComponentBase implements OnInit {
  parentassigneName;
  endOnIsEnabled:boolean = true;

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
  active =false;
  duplicateData;
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
    this.getTaskForEdit = new GetTaskForEditDto();
    this.active = true;
    this._closingChecklistService.getTaskForEdit(this.taskId).subscribe(result => {
      this.getTaskForEdit = result;
      this.closingMonthValue = this.getTaskForEdit.closingMonth.toDate();


      this.frequencyId = this.getTaskForEdit.frequency;
      if(this.frequencyId == "5"){
        this.endOnIsEnabled = false;
      }
      this.getTaskForEdit.closingMonth = moment().startOf('day');
      this.getTaskForEdit.endsOn = moment().startOf('day');
      this.assigneeId = this.getTaskForEdit.assigneeId;
      this.parentassigneName = result;
      this.categoryId = this.getTaskForEdit.categoryId;
    });
    this._categoryService.categoryDropDown().subscribe(result => {
      this.category = result;
    });
    
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

  duplicateTask() {
    this.checklist.frequency = this.getTaskForEdit.frequencyId;
    this.checklist.closingMonth = this.getTaskForEdit.closingMonth;
    this.checklist.endsOn = this.getTaskForEdit.endsOn;
    this.checklist.categoryId = this.getTaskForEdit.categoryId;
    this.checklist.dueOn = this.getTaskForEdit.dueOn;
    this.checklist.endOfMonth = this.getTaskForEdit.endOfMonth;
    this.checklist.noOfMonths = this.getTaskForEdit.noOfMonths;
    this.checklist.taskName = this.getTaskForEdit.taskName;
    this.checklist.instruction = this.getTaskForEdit.instruction;
    this.checklist.assigneeId = Number(this.selectedUserId.selectedUserId.value);
    this.checklist.status = 1;
    this.checklist.comments = [];
    this._closingChecklistService.createOrEdit(this.checklist).subscribe(result => {

      this.notify.success(this.l('Successfully Duplicated'));
      this._router.navigate(['/app/main/checklist/tasks']);

    });
    
  }
  routerEvent(value){
    if(value = -1)
  this._router.navigate(['/app/main/categories/create-or-edit-category'], { state: { data: { id: 0 } } });
  }
  back(){
    this._router.navigate(['/app/main/checklist'])
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
