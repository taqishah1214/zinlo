import { Component, OnInit, Injector } from '@angular/core';
import { Router } from '@angular/router';
import { UppyConfig } from 'uppy-angular';
import { ClosingChecklistServiceProxy } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';

@Component({
  selector: 'app-task-details',
  templateUrl: './task-details.component.html',
  styleUrls: ['./task-details.component.css']
})
export class TaskDetailsComponent extends AppComponentBase implements OnInit {
 taskObject : any;
 taskDetailObject :any;
 recordId : number = 0;
  constructor(
    injector: Injector,
    private _router: Router,
    private _closingChecklistService: ClosingChecklistServiceProxy
    ) {
      super(injector);
     }

  ngOnInit() {
    debugger;
     this.recordId = history.state.data.id;
     this.getTaskDetails(this.recordId);
  }

  RedirectToEditTaskPage() : void {
    this._router.navigate(['/app/main/TasksCheckList/edit-task'], { state: { data: { id: this.recordId } } })
  }

  BackToTaskList(): void {
    this._router.navigate(['/app/main/TasksCheckList/tasks']);
  }
getTaskDetails(id) : void{

   this._closingChecklistService.getDetails(id).subscribe(result=>{
   this.taskDetailObject = result;
   console.log(this.taskDetailObject);

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
  
  deleteTask(): void {
    this.message.confirm(
        '',
        this.l('AreYouSure'),
        (isConfirmed) => {
            if (isConfirmed) {
                this._closingChecklistService.delete(this.recordId)
                    .subscribe(() => {
                        this.notify.success(this.l('SuccessfullyDeleted'));
                        this._router.navigate(['/app/main/TasksCheckList/tasks']);
                    });
            }
        }
    );
}


}
