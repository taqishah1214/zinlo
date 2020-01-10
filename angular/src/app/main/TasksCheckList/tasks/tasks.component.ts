import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'app-tasks',
  templateUrl: './tasks.component.html',
  styleUrls: ['./tasks.component.css']
})
export class TasksComponent implements OnInit {

  constructor(private _router: Router) {
  }

  ngOnInit() {
  }

  RedirectToCreateTask() :void {
    this._router.navigate(['/app/main/TasksCheckList/create-or-edit-task']);   
}

}
