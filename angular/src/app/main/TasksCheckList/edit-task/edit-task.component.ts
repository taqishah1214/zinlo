import { Component, OnInit, Injector, Output } from '@angular/core';
import { CreateOrEditClosingChecklistDto, ClosingChecklistServiceProxy, GetTaskForEditDto, CategoriesServiceProxy, ChangeStatusDto } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import { UppyConfig } from 'uppy-angular';

import * as moment  from 'moment';


@Component({
  selector: 'app-edit-task',
  templateUrl: './edit-task.component.html',
  styleUrls: ['./edit-task.component.css']
})
export class EditTaskComponent extends AppComponentBase implements OnInit {
  parentassigneName;
  SelectedCategory;
  checklist: CreateOrEditClosingChecklistDto = new CreateOrEditClosingChecklistDto()
  commantBox : boolean
  userSignInName : string
  taskId : any;
  closingMonth : Date
  getTaskForEdit : GetTaskForEditDto = new GetTaskForEditDto();
  _changeStatus: ChangeStatusDto = new ChangeStatusDto();
  assigniName;
  assignee;
  category;
  users;
  frequency;
  frequencyId: string;
  categoryId: number;
  comment;
  statusValue :any;
 public closingMonthValue:Date;
  createOrEdit: void;
  userName: string[];
  assigneeId: any;

  constructor( private _categoryService: CategoriesServiceProxy,injector: Injector,private _closingChecklistService: ClosingChecklistServiceProxy) {
    super(injector)
    this.commantBox = true;
    
   }

  ngOnInit() {
   
      this.userSignInName =  this.appSession.user.name.toString().charAt(0).toUpperCase();
      this.taskId=history.state.data.id;
      this._closingChecklistService.getTaskForEdit(this.taskId).subscribe(result => {
      this.getTaskForEdit = result;
      this.closingMonthValue = this.getTaskForEdit.closingMonth.toDate();


      this.frequencyId = this.getTaskForEdit.frequency;
      this.getTaskForEdit.closingMonth=moment().startOf('day');
      this.getTaskForEdit.endsOn=moment().startOf('day');
      this.assigniName = this.getTaskForEdit.assigniName;
      this.parentassigneName = result;
      this.categoryId = this.getTaskForEdit.categoryId;
      this.comment = this.getTaskForEdit.comments;
      console.log("shdshsdhshkhs",this.assigniName)
      
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

  ChangeStatus (value) : void {
    if (value === 1)
  {
    this.statusValue = 1;
    this.getTaskForEdit.status = "Open"
  }
  if (value === 2)
  {
    this.statusValue = 2;
    this.getTaskForEdit.status = "Complete"
  } 
  if (value === 3)
  {
    this.statusValue = 3;
    this.getTaskForEdit.status = "Inprogress"
  }
  // this.checklist = this.getTaskForEdit;
  }

  
  onSearchUsers(event): void {
    this._closingChecklistService.userAutoFill(event.query).subscribe(result => {
      this.users = result;
      this.userName = result.map(a => a.name);
      });
      if (this.users) {
        this.assigneeId = this.users.filter(a => {
          if (a.name == this.getTaskForEdit.assigniName) {
            return a.value;
          }
        });
        
  }
}
onCreateTask(){
 this.checklist.id = this.getTaskForEdit.id;
  this.checklist.frequency = this.getTaskForEdit.frequencyId;
  this.checklist.closingMonth = this.getTaskForEdit.closingMonth;
  this.checklist.endsOn = this.getTaskForEdit.endsOn;
  this.checklist.status = this.statusValue;
  this.checklist.categoryId = this.getTaskForEdit.categoryId;
  this.checklist.dueOn = this.getTaskForEdit.dueOn;
  this.checklist.endOfMonth  = this.getTaskForEdit.endOfMonth;
  this.checklist.noOfMonths = this.getTaskForEdit.noOfMonths;
  this.checklist.taskName = this.getTaskForEdit.taskName;
  this.checklist.instruction = this.getTaskForEdit.instruction;
  // this.checklist. = this.getTaskForEdit.comments
  // this.checklist.assigneeId = this.getTaskForEdit.assigniName
  console.log("Edited for status ",this.checklist.status)
    this._closingChecklistService.createOrEdit(this.checklist).subscribe(result => {
    this.createOrEdit = result;
    console.log("this.createOrEdit ",result)

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
