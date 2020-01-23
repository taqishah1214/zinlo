import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { UppyConfig } from 'uppy-angular';

@Component({
  selector: 'app-task-details',
  templateUrl: './task-details.component.html',
  styleUrls: ['./task-details.component.css']
})
export class TaskDetailsComponent implements OnInit {
 taskObject : any;
 
  constructor(private _router: Router) { }

  ngOnInit() {
    history.state.data;
  }

  BackToTaskList(): void {
    this._router.navigate(['/app/main/TasksCheckList/tasks']);
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
