import { Component, OnInit, ViewChild } from '@angular/core';
import { Router } from '@angular/router';
import {ClosingChecklistServiceProxy} from '@shared/service-proxies/service-proxies';
import { UserListComponentComponent } from '../user-list-component/user-list-component.component';


@Component({
  selector: 'app-tasks',
  templateUrl: './tasks.component.html',
  styleUrls: ['./tasks.component.css']
})
export class TasksComponent implements OnInit {
  @ViewChild(UserListComponentComponent,{ static: false }) selectedUserId: UserListComponentComponent;

  
  ClosingCheckList : any


  constructor(private _router: Router,     private _closingChecklistService: ClosingChecklistServiceProxy) {
  }

  ngOnInit() {
    this._closingChecklistService.getAll().subscribe(result => {
      console.log("Done",result)
       this.ClosingCheckList = result.items;
      });
  }

  RedirectToCreateTask() :void {
    this._router.navigate(['/app/main/TasksCheckList/create-or-edit-task']);   
}


}
