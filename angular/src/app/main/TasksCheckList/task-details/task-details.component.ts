import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { UppyConfig } from 'uppy-angular';
import { ClosingChecklistServiceProxy } from '@shared/service-proxies/service-proxies';

@Component({
  selector: 'app-task-details',
  templateUrl: './task-details.component.html',
  styleUrls: ['./task-details.component.css']
})
export class TaskDetailsComponent implements OnInit {
 taskObject : any;
 taskDetailObject :any;
 recordId : number = 0;
  constructor(private _router: Router,
    private _closingChecklistService: ClosingChecklistServiceProxy
    ) { }

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



}
