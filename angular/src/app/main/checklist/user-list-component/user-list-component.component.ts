import { Component, OnInit, EventEmitter, Output, Input, OnChanges, SimpleChanges, ChangeDetectorRef, ViewChild } from '@angular/core';
import { UserServiceProxy, ClosingChecklistServiceProxy, ChangeAssigneeDto} from '@shared/service-proxies/service-proxies';
import { OnChange } from 'ngx-bootstrap';
import { NgSelectComponent } from '@ng-select/ng-select';


@Component({
  selector: 'app-user-list-component',
  templateUrl: './user-list-component.component.html',
  styleUrls: ['./user-list-component.component.css']
})
export class UserListComponentComponent implements OnInit, OnChanges {
  
  users : any;
  input : any;
  public selectedUserId:any;
  changeAssigneeDto : ChangeAssigneeDto = new ChangeAssigneeDto ();
  @Input() comingChangeReguest : any;
  @Input() userId : any;
  @Input() taskId : any;
  @Output() messageEvent = new EventEmitter<string>();
  @ViewChild(NgSelectComponent,{ static: true }) ngSelect : NgSelectComponent;
  @Output("callBack") callBack: EventEmitter<any> = new EventEmitter();
  constructor( private userService :UserServiceProxy,
    private cdf: ChangeDetectorRef,
    private _closingChecklistService: ClosingChecklistServiceProxy ) {}


  ngOnInit() {
      if (this.userId != 0) {
       this.getSelectedUserIdandPicture()
      }
  }

  ngOnChanges(changes: SimpleChanges): void {
    var change = changes['userId'];
    if (change.firstChange === false)
    {
      this.userId = change.currentValue;
      this.getSelectedUserIdandPicture()
    }
  }

  getSelectedUserIdandPicture() : void {
    this._closingChecklistService.getUserWithPicture("",this.userId).subscribe(result => {
      this.users = result; 
      this.input = this.users[0].id;
  });
  }

  onSearchUsers(event): void {
    this._closingChecklistService.getUserWithPicture(event,0).subscribe(result => {
        this.users = result; 
    });
}


  userOnChange(value) : void {
    this.selectedUserId = value;
    if (this.selectedUserId != null)
    {
    this.messageEvent.emit(this.selectedUserId);
    if (this.comingChangeReguest == "ClosingCheckList" || this.comingChangeReguest == "DetailsClosingCheckList" ) {
      this.changeAssigneeDto.assigneeId = this.selectedUserId;
      this.changeAssigneeDto.taskId = this.taskId;
      this._closingChecklistService.changeAssignee(this.changeAssigneeDto).subscribe( result => {
        this.callBack.emit()
      })
    }
  }
}
}



