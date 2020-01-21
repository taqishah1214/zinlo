import { Component, OnInit, Input } from '@angular/core';
import {ClosingChecklistServiceProxy,ChangeStatusDto}from '@shared/service-proxies/service-proxies';
import { Router } from '@angular/router';

@Component({
  selector: 'app-status',
  templateUrl: './status.component.html',
  styleUrls: ['./status.component.css']
})
export class StatusComponent implements OnInit {

  @Input() StatusList: any;
  @Input() TaskId : any;
  _changeStatus: ChangeStatusDto = new ChangeStatusDto();

  constructor( private _closingChecklistService: ClosingChecklistServiceProxy ,private _router: Router) { }

  ngOnInit() {
  }

  ChangeStatus(value) : void {
    this._changeStatus.statusId = value;
    this._changeStatus.taskId = this.TaskId;
   this._closingChecklistService.changeStatus(this._changeStatus).subscribe(result => 
    {
      this._router.navigate(['/app/main/TasksCheckList/tasks']);
    });
  }

}
