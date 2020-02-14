import { Component, OnInit, Injector, Output, ViewChild } from '@angular/core';
import { CreateOrEditClosingChecklistDto, ClosingChecklistServiceProxy, GetTaskForEditDto, CategoriesServiceProxy, ChangeStatusDto, PostAttachmentsPathDto } from '@shared/service-proxies/service-proxies';
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
  endOnIsEnabled:boolean = true;
  enableValue: boolean = false;
  isChecked :boolean = false;
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
  categoryId: any;
  comment;
  statusValue: any;
  public closingMonthValue: Date;
  createOrEdit: void;
  userName: string[];
  assigneeId: any;
  status: number;
  active = false;
  categoryName : any;
  public userId: number;
  categoriesList : any;

  @ViewChild(UserListComponentComponent, { static: false }) selectedUserId: UserListComponentComponent;
  days: any;
  daysBeforeAfter: any;

  constructor(private _categoryService: CategoriesServiceProxy, injector: Injector, private _closingChecklistService: ClosingChecklistServiceProxy, private _router: Router) {
    super(injector)

  }

  ngOnInit() {
    this.enableValue = false;
    this.isChecked = true;
    
    this.taskId = history.state.data.id;
    this.commantBox = false;
    this.userSignInName = this.appSession.user.name.toString().charAt(0).toUpperCase();
    this.taskId = history.state.data.id;
    this.getTaskForEdit = new GetTaskForEditDto();
    this.active = true;
    this._closingChecklistService.getTaskForEdit(this.taskId).subscribe(result => {
      this.getTaskForEdit = result;
      console.log("result",result);
      this.ChangeStatus(result.statusId);
      this.closingMonthValue = this.getTaskForEdit.closingMonth.toDate();
      this.frequencyId = this.getTaskForEdit.frequency;
      if (this.frequencyId == "5") {
        this.endOnIsEnabled = false;
      }
      else {
        this.endOnIsEnabled = true;
      }
      this.getTaskForEdit.closingMonth = moment().startOf('day');
      this.getTaskForEdit.endsOn = moment().startOf('day');

      if(this.getTaskForEdit.dayBeforeAfter == false){
        this.daysBeforeAfter = "Days After"
      }
      else{
      this.daysBeforeAfter = "Days Before"
      }
      this.assigneeId = this.getTaskForEdit.assigneeId;
      this.parentassigneName = result;
      this.categoryName = this.getTaskForEdit.category;
      this.comment = this.getTaskForEdit.comments;

      this.comment.forEach(i => {
        i.userName;
        var firstCharacterForAvatar = i.userName[0].toUpperCase();
        var lastCharacterForAvatar = i.userName.substr(i.userName.indexOf(' ') + 1)[0].toUpperCase();
        i["NameAvatar"] = firstCharacterForAvatar + lastCharacterForAvatar;
      });
    });
    this._categoryService.categoryDropDown().subscribe(result => {
      this.categoriesList = result;
    });
    this.loadDaysDropdown();

  }

  categoryClick(id,name) : void {
    this.categoryName = name;
    this.getTaskForEdit.categoryId = id; 
  }

  routeToAddNewCategory() : void {
    this._router.navigate(['/app/main/categories/create-or-edit-category'], { state: { data: { id: 0 ,redirectPath : "editChecklist","checklistTask" : this.taskId } } });   
  }

  onChange(val) {
    if (val == 5) {
      this.endOnIsEnabled = false;
    }
    else {
      this.endOnIsEnabled = true;
    }
    if (val == 4) {
      this.enableValue = true;
    }
    else {
      this.enableValue = false;
    }
  }
  onDayChange() {
    this.checklist.endOfMonth = false;
    this.isChecked = true;
  }

  loadDaysDropdown():void{
    this._closingChecklistService.getCurrentMonthDays().subscribe(result=>{
      this.days = result;
    });
  }
  
  ChangeStatus(value): void {
    if (value === 1) {
      this.status = 1;
      this.getTaskForEdit.status = "Not Started";
      this.checklist.status = 1;

    }
    if (value === 2) {
      this.status = 2;
      this.getTaskForEdit.status = "In Process"
      this.checklist.status = 2;
    }
    if (value === 3) {
      this.status = 3;
      this.getTaskForEdit.status = "On Hold"
      this.checklist.status = 3;
    }
    if (value === 4) {
      this.status = 4;
      this.getTaskForEdit.status = "Completed"
      this.checklist.status = 4;
    }
  }

  handleRadioChange() {
    this.checklist.dayBeforeAfter = null;
    this.checklist.dueOn = 0;
    this.isChecked = false;
  }
  onDaysClick(valu) {
    this.isChecked = true;
    if(this.isChecked == true){
      this.daysBeforeAfter == "Days Before";
    }
    else{
      this.daysBeforeAfter == "Days After";
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
      this._router.navigate(['/app/main/checklist/tasks']);
    });
  }
 
  duplicateBtn(){
    this._router.navigate(['/app/main/duplicate-task'], { state: { data: { id: this.taskId } } });
  }
  back(){
    this._router.navigate(['/app/main/checklist/task-details'])
  }
  settings: UppyConfig = {
    uploadAPI: {
      endpoint: "http://localhost:22742/api/services/app/Attachments/PostAttachmentFile",
    },
    plugins: {
      Webcam: false
    }
  }
}
