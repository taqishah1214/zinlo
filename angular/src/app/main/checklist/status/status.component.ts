import { Component, OnInit, Input, ViewChild } from '@angular/core';
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

  RedirectToDetail(recordId): void {
    this._router.navigate(['/app/main/checklist/task-details'], { state: { data: { id: recordId } } });

  }
  ChangeStatus(value) : void {
    console.log("-------------------------------",this.StatusList)
    this.ChangeValue(value);
    this._changeStatus.statusId = value;
    this._changeStatus.taskId = this.TaskId;
    this._closingChecklistService.changeStatus(this._changeStatus).subscribe(result => 
    {
      
    });
    this.RedirectToDetail(this.TaskId)
  }

  ChangeValue(value): void {
    if (value === 1) {
      this._changeStatus.statusId = 1;
       this.StatusList = "Not Started";
      // this.checklist.status = 1;

    }
    if (value === 2) {
      this._changeStatus.statusId = 2;
      this.StatusList = "In Process"
      // this.checklist.status = 2;
    }
    if (value === 3) {
      this._changeStatus.statusId = 3;
      this.StatusList = "On Hold"
      // this.checklist.status = 3;
    }
    if (value === 4) {
      this._changeStatus.statusId = 4;
      this.StatusList = "Completed"
      // this.checklist.status = 4;
    }
  }

}
