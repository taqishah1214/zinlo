import { Component, OnInit, Injector, Output } from '@angular/core';
import { CreateOrEditClosingChecklistDto, ClosingChecklistServiceProxy, GetTaskForEditDto, CategoriesServiceProxy } from '@shared/service-proxies/service-proxies';
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
 assigniName;
 category;
 users;
frequency;
  constructor( private _categoryService: CategoriesServiceProxy,injector: Injector,private _closingChecklistService: ClosingChecklistServiceProxy) {
    super(injector)
    this.commantBox = true;
    
   }

  ngOnInit() {
      this.userSignInName =  this.appSession.user.name.toString().charAt(0).toUpperCase();
      this.taskId=history.state.data.id;
      this._closingChecklistService.getTaskForEdit(this.taskId).subscribe(result => {
      this.getTaskForEdit = result;
      this.getTaskForEdit.frequency = this.frequency;
      console.log("this.frequency",result)
      this.getTaskForEdit.closingMonth=moment().startOf('day');
      this.getTaskForEdit.endsOn=moment().startOf('day');
      this.assigniName=this.getTaskForEdit.assigniName;
      console.log("assigneename",this.assigniName);
      this.parentassigneName = result;
      this.SelectedCategory = result.category;
     });
      this._categoryService.categoryDropDown().subscribe(result => {
       this.category = result;
  });

  }

  onSearchUsers(event): void {
    this._closingChecklistService.userAutoFill(event.query).subscribe(result => {
        this.users = result;
        console.log("task id ",this.users.value)
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
