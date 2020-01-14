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
  checklist: CreateOrEditClosingChecklistDto = new CreateOrEditClosingChecklistDto();


  constructor
    (private _router: Router,
     private _categoryService: CategoriesServiceProxy,
     private _closingChecklistService: ClosingChecklistServiceProxy) {

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
     console.log("value",value);
     this.closingMonth = value;
  }

  onCreateTask() : void {
    console.log("Email",this.Email)
    console.log("assigneeName",this.assigneeName)
    console.log("taskName",this.taskName)
    console.log("comment",this.comment)

    var date = new Date();

    this.checklist.frequency = 2;
    this.checklist.assigneeNameId = 2;
    this.checklist.categoryId = 1;
    this.checklist.closingMonth = moment(date);
    this.checklist.commentBody = this.comment;
    this.checklist.instruction = "task 1";
    this.checklist.noOfMonths = 2;
    this.checklist.dueOn = 2;
    this.checklist.endOfMonth = false;
    this.checklist.status = 1;
    this.checklist.endsOn =moment(date);
    this.checklist.attachment = "hammad"

debugger
    this._closingChecklistService.createOrEdit(this.checklist).subscribe(result => {
      debugger
      console.log("done")
      });

  }
  frqenceySelect(value) : void {
    this.frequenct = value;
    console.log("value",value);


  }

}

