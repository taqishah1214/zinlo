import { Component, OnInit, ViewChild } from '@angular/core';
import { Router } from '@angular/router';
import { CategoriesServiceProxy, NameValueDto, TaskDto, CreateOrEditClosingChecklistDto, ClosingChecklistServiceProxy } from '@shared/service-proxies/service-proxies';
import * as moment from "moment";

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
  checklist: CreateOrEditClosingChecklistDto = new CreateOrEditClosingChecklistDto();


  constructor
    (private _router: Router,
     private _categoryService: CategoriesServiceProxy,
     private _closingChecklistService: ClosingChecklistServiceProxy) {

      this.commantBox = true;

  }

  ngOnInit() {

    //   this._categoryService.categoryDropDown().subscribe(result => {
    //     debugger
    //     this.categories = result;
    // });
    //   console.log(this.categories);
  }

  BackToTaskList(): void {
    this._router.navigate(['/app/main/TasksCheckList/tasks']);
  }

  closingMonthSelect(value) : void {
    //////////////////
    this.checklist.closingMonth = value;
  }

  onChangeAssigniName(value) : void {
    this.checklist.assigneeNameId = value
  }
  onChangeFrequency(value) : void {
    this.checklist.frequency = value;
  }

  onChangeDueOn(value) : void {
    this.checklist.dueOn = value;
  }
  onChangeDaysBefore(value) : void {
    ///////////////////////
    this.checklist.dayBeforeAfter = false;
  }
  onChangeEndsOn(value): void {
    //////////////////////////////
    this.checklist.endsOn = value;
  }
  EndofMonthSelected() : void {
    this.checklist.endOfMonth = true;
  }
  EndofMonthUnselected():void {
  this.checklist.endOfMonth =false;
  }


  onCreateTask() : void {
    var date = new Date();
     
    this.checklist.taskName = "hammad";
    this.checklist.categoryId = 1;
    this.checklist.assigneeNameId = 1;
    this.checklist.closingMonth =moment(date) ;
    this.checklist.status = 1;
    this.checklist.attachment = "hammadfile";
    this.checklist.instruction = "hammadtask1";
    this.checklist.noOfMonths = 1;
    this.checklist.dueOn = 1;
    this.checklist.endsOn = moment(date);
    this.checklist.dayBeforeAfter = false;
    this.checklist.endOfMonth = false;
    this.checklist.frequency = 1;
    this.checklist.commentBody = "hammadcomment"


    debugger
    this._closingChecklistService.createOrEdit(this.checklist).subscribe(result => {
     
      console.log("done")
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

