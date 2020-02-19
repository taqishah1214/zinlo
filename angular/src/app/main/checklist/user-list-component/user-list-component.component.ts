import { Component, OnInit, EventEmitter, Output, Input, OnChanges, SimpleChanges, ChangeDetectorRef, ViewChild } from '@angular/core';
import { UserServiceProxy, ClosingChecklistServiceProxy} from '@shared/service-proxies/service-proxies';
import { OnChange } from 'ngx-bootstrap';
import { NgSelectComponent } from '@ng-select/ng-select';


@Component({
  selector: 'app-user-list-component',
  templateUrl: './user-list-component.component.html',
  styleUrls: ['./user-list-component.component.css']
})
export class UserListComponentComponent implements OnInit {

  users : any;
  input : any;
  public selectedUserId:any;

  @Input() userId : any;
  @Output() messageEvent = new EventEmitter<string>();
  @ViewChild(NgSelectComponent,{ static: true }) ngSelect : NgSelectComponent;
  constructor( private userService :UserServiceProxy,
    private cdf: ChangeDetectorRef,
    private _closingChecklistService: ClosingChecklistServiceProxy ) {

   }

  ngOnInit() {
      if (this.userId) {
        this._closingChecklistService.getUserWithPicture("",this.userId).subscribe(result => {
          this.users = result; 
          this.input = this.users[0].id;
      });
      }
  }
  onSearchUsers(event): void {
    this._closingChecklistService.getUserWithPicture(event,0).subscribe(result => {
        this.users = result; 
    });
}


  userOnChange(value) : void {
    this.selectedUserId = value;
    this.messageEvent.emit(this.selectedUserId);
    debugger;
    console.log(this.input)
  }
}



