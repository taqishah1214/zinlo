import { Component, OnInit, ViewChild } from '@angular/core';
import { Router } from '@angular/router';
import { CategoriesServiceProxy, NameValueDto, CreateOrEditClosingChecklistDto, ClosingChecklistServiceProxy , UserServiceProxy} from '@shared/service-proxies/service-proxies';
import * as moment from "moment";
import { CategorieDropDownComponent } from '@app/main/categories/categorie-drop-down/categorie-drop-down.component';
import { UserListComponentComponent } from '../user-list-component/user-list-component.component';

@Component({
  selector: 'app-create-or-edit-task',
  templateUrl: './create-or-edit-task.component.html',
  styleUrls: ['./create-or-edit-task.component.css']
})


export class CreateOrEditTaskComponent implements OnInit {
  categories: any;
  Email: string;
  taskName: string;
  assigneeName: string;
  comment: string;
  closingMonth : string;
  frequenct : string;
  commantModal : boolean;
  commantBox : boolean;
  closingMonthInputBox : boolean;
  closingMonthModalBox : boolean;
  checklist: CreateOrEditClosingChecklistDto = new CreateOrEditClosingChecklistDto();
  @ViewChild(CategorieDropDownComponent,{ static: false }) selectedCategoryId: CategorieDropDownComponent;
  @ViewChild(UserListComponentComponent,{ static: false }) selectedUserId: UserListComponentComponent;

  

  constructor
    (private _router: Router,
     private _categoryService: CategoriesServiceProxy,
     private _closingChecklistService: ClosingChecklistServiceProxy) {

      this.commantBox = true;
      this.closingMonthInputBox = true;
      this.closingMonthModalBox = false;
  }

  ngOnInit() {

  }

  closingMonthClick() : void {
    this.closingMonthInputBox = false;
      this.closingMonthModalBox = true;
  }

  BackToTaskList(): void {
    this._router.navigate(['/app/main/TasksCheckList/tasks']);
  }

  

  closingMonthSelect(value) : void {
    var date = new date();
    ////////////////////
    this.checklist.closingMonth = moment(date);
  }

  onChangeAssigniName(value) : void {
    this.checklist.assigneeNameId = value
  }
 
  onChangeDaysBefore(value) : void {
    console.log("onChangeDaysBefore");
    ///////////////////////
    this.checklist.dayBeforeAfter = false;
  }
  onChangeEndsOn(value): void {
    var date = new date();
    //////////////////////////////
    this.checklist.endsOn = moment(date);
  }
  EndofMonthSelected() : void {
    this.checklist.endOfMonth = true;
  }
  EndofMonthUnselected():void {
  this.checklist.endOfMonth =false;
  }
 


  onCreateTask() : void {
    if(this.checklist.dayBeforeAfter)
    {
      this.checklist.dayBeforeAfter = true;
     }
     else
     {
      this.checklist.dayBeforeAfter = false;
    }

    if(this.checklist.endOfMonth)
     {
      this.checklist.endOfMonth = true;
     }
     else
     {
      this.checklist.endOfMonth = false;
     }

    this.checklist.dueOn = Number(this.checklist.dueOn);
    this.checklist.frequency = Number(this.checklist.frequency);
    this.checklist.status = 1
    this.checklist.assigneeNameId = Number(this.selectedUserId.selectedUserId);
    this.checklist.categoryId = Number(this.selectedCategoryId.categoryId);
    this.checklist.noOfMonths = Number(this.checklist.noOfMonths);


    ////////////////////////////////////////////////////Ask from Taqi
    //this.checklist.closingMonth = moment(date);
    //this.checklist.endsOn = moment(date);
    ///////////////////////////////////////////////////
  


    debugger
    this._closingChecklistService.createOrEdit(this.checklist).subscribe(result => {
      alert("Successfully Created!!!")
      });
  }

  commentClick() : void {
    this.commantModal = true;
    this.commantBox = false;
  }

  onComment() : void {
    this.commantModal = false;
    this.commantBox = true;
  }
  onCancelComment() : void {
    this.commantModal = false;
    this.commantBox = true;
  }


}

