import { Component, OnInit, Injector } from '@angular/core';
import { CreateOrEditClosingChecklistDto, ClosingChecklistServiceProxy, GetTaskForEditDto } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import { UppyConfig } from 'uppy-angular';


@Component({
  selector: 'app-edit-task',
  templateUrl: './edit-task.component.html',
  styleUrls: ['./edit-task.component.css']
})
export class EditTaskComponent extends AppComponentBase implements OnInit {

checklist: CreateOrEditClosingChecklistDto = new CreateOrEditClosingChecklistDto()
commantBox : boolean
userSignInName : string
taskId : any;
closingMonth : Date
getTaskForEdit : GetTaskForEditDto = new GetTaskForEditDto();

  constructor(injector: Injector,private _closingChecklistService: ClosingChecklistServiceProxy) {
    super(injector)
    this.commantBox = true;
    
   }

  ngOnInit() {
    this.userSignInName =  this.appSession.user.name.toString().charAt(0).toUpperCase();
     this.taskId=history.state.data.id;
     this._closingChecklistService.getTaskForEdit(this.taskId).subscribe(result => {
       console.log("hammad",result);
       this.getTaskForEdit = result;
      


     })

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
