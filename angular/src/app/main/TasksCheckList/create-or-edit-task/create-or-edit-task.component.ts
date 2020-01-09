import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'app-create-or-edit-task',
  templateUrl: './create-or-edit-task.component.html',
  styleUrls: ['./create-or-edit-task.component.css']
})
export class CreateOrEditTaskComponent implements OnInit {

  constructor(private _router: Router) { }

  ngOnInit() {
  }

  BackToTaskList() :void {
    this._router.navigate(['/app/main/TasksCheckList/tasks']);   
}

}

