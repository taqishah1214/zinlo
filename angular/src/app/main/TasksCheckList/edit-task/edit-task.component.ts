import { Component, OnInit, Injector } from '@angular/core';
import { CreateOrEditClosingChecklistDto } from '@shared/service-proxies/service-proxies';
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

  constructor(injector: Injector) {
    super(injector)
    this.commantBox = true;
   }

  ngOnInit() {
    this.userSignInName =  this.appSession.user.name.toString().charAt(0).toUpperCase();
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
