import { Component, OnInit, Input, ViewChild, Output, EventEmitter } from '@angular/core';
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
  @Output() messageEvent = new EventEmitter<number>();
  _changeStatus: ChangeStatusDto = new ChangeStatusDto();
  
  @Output() reload: EventEmitter<any> = new EventEmitter();
  constructor( private _closingChecklistService: ClosingChecklistServiceProxy ,private _router: Router) { }

  ngOnInit() {
  }

  RedirectToDetail(recordId): void {
    this._router.navigate(['/app/main/checklist/task-details'], { state: { data: { id: recordId } } });

  }
  ChangeStatus(value) : void {
    
   
    this.ChangeValue(value);
    this._changeStatus.statusId = value;
    this._changeStatus.taskId = this.TaskId;
    if(this.TaskId != 0){
      this._closingChecklistService.changeStatus(this._changeStatus).subscribe();
      this.RedirectToDetail(this.TaskId)
    }
    else{
      this.messageEvent.emit(this._changeStatus.statusId);
    }
    this.reload.emit();
  }

  ChangeValue(value): void {
    if (value === 1) {
      this._changeStatus.statusId = 1;
       this.StatusList = "Not Started";
    }
    if (value === 2) {
      this._changeStatus.statusId = 2;
      this.StatusList = "In Process"
    }
    if (value === 3) {
      this._changeStatus.statusId = 3;
      this.StatusList = "On Hold"
    }
    if (value === 4) {
      this._changeStatus.statusId = 4;
      this.StatusList = "Completed"
    }
  }

}
