import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import {ClosingChecklistServiceProxy} from '@shared/service-proxies/service-proxies';

@Component({
  selector: 'app-tasks',
  templateUrl: './tasks.component.html',
  styleUrls: ['./tasks.component.css']
})
export class TasksComponent implements OnInit {
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
