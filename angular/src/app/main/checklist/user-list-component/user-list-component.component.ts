import { Component, OnInit, EventEmitter, Output, Input, OnChanges, SimpleChanges, ChangeDetectorRef, ViewChild } from '@angular/core';
import { UserServiceProxy, ClosingChecklistServiceProxy, ChangeAssigneeDto, ChartsofAccountServiceProxy } from '@shared/service-proxies/service-proxies';
import { OnChange } from 'ngx-bootstrap';
import { NgSelectComponent } from '@ng-select/ng-select';
import { StoreDateService } from "../../../services/storedate.service";



@Component({
  selector: 'app-user-list-component',
  templateUrl: './user-list-component.component.html',
  styleUrls: ['./user-list-component.component.css']
})
export class UserListComponentComponent implements OnInit, OnChanges {
  userList : any;
  users: any;
  input: any;
  public selectedUserId: any;
  changeAssigneeDto: ChangeAssigneeDto = new ChangeAssigneeDto();
  @Input() comingChangeReguest: any;
  @Input() userId: any;
  @Input() taskId: any;
  @Input() disable: any;
  defaultUser: Array<{ id: number, name: string, picture: string }> = [{ id: -1, name: " Enter the Assignee Name", picture: "../../../../assets/media/files/emptyUser.svg" }];
  @Output() messageEvent = new EventEmitter<string>();
  @ViewChild(NgSelectComponent, { static: true }) ngSelect: NgSelectComponent;
  @Output("callBack") callBack: EventEmitter<any> = new EventEmitter();
  constructor(private userService: UserServiceProxy,private userDate: StoreDateService,
    private cdf: ChangeDetectorRef,
    private _closingChecklistService: ClosingChecklistServiceProxy,
    private _chartOfAccountService: ChartsofAccountServiceProxy) { }


  ngOnInit() {
    this.userDate.allUsersInformationofTenant.subscribe(userList => this.users = userList)
    if (this.disable === "true") {
      this.disable = true
    }  
    if (this.userId != 0) {
      this.getSelectedUserIdandPicture()
    }
    else {
      this.getUserDefaultPicture()
    }
    
  }

  ngOnChanges(changes: SimpleChanges): void {
    var change = changes['userId'];
    if (change.firstChange === false) {
      this.userId = change.currentValue;
      this.getSelectedUserIdandPicture()
    }
  }

  getUserDefaultPicture(): void {
    this.input = this.defaultUser[0].id;
    let updateLock = false;
    this.users.forEach(element => {
      if (element.id === -1)
      {
        updateLock = true
      }
    });
    if (updateLock == false) {
      this.users.push({ id: -1, name: " Enter the Assignee Name", picture: "../../../../assets/media/files/emptyUser.svg" });
    }
  }



  getSelectedUserIdandPicture(): void {
    this.users.forEach(element => {
      if (element.id == this.userId){
        this.input = element.id;
      }
    });
  }

  userOnChange(value): void {
    this.selectedUserId = value;
    if (this.selectedUserId != undefined ) {
      if (this.selectedUserId != -1) {
        this.messageEvent.emit(this.selectedUserId);
        if (this.comingChangeReguest == "ClosingCheckList" || this.comingChangeReguest == "DetailsClosingCheckList") {
          this.changeAssigneeDto.assigneeId = this.selectedUserId;
          this.changeAssigneeDto.taskId = this.taskId;
          this._closingChecklistService.changeAssignee(this.changeAssigneeDto).subscribe(result => {
            this.callBack.emit()
          })
        }
        else if (this.comingChangeReguest == "accounts") {
          this._chartOfAccountService.changeAccountsAssignee(this.taskId, this.selectedUserId).subscribe(result => {
            this.callBack.emit()
          })
        }
      }
    }
    else{
      this.getUserDefaultPicture();
    }
  }
}



