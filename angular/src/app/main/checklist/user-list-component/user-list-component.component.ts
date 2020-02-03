import { Component, OnInit, EventEmitter, Output, Input, OnChanges, SimpleChanges, ChangeDetectorRef } from '@angular/core';
import { UserServiceProxy, ClosingChecklistServiceProxy} from '@shared/service-proxies/service-proxies';
import { OnChange } from 'ngx-bootstrap';


@Component({
  selector: 'app-user-list-component',
  templateUrl: './user-list-component.component.html',
  styleUrls: ['./user-list-component.component.css']
})
export class UserListComponentComponent implements OnInit {

  users : any;
  public selectedUserId:any;
  @Input() TaskId : any;
  @Input('assigneName') assigneName;
  @Output() messageEvent = new EventEmitter<string>();
   

  constructor( private userService :UserServiceProxy,
    private cdf: ChangeDetectorRef,
    private _closingChecklistService: ClosingChecklistServiceProxy ) {

   }

  ngOnInit() {
    this.userService.userDropDown().subscribe(result => {
      this.users = result;
      this.assigneName = this.users;

  });
  }

  userOnChange() : void {
    this.messageEvent.emit(this.selectedUserId);
  }

  onSearchUsers(event): void {
    this._closingChecklistService.userAutoFill(event.query).subscribe(result => {
        this.users = result;
        this.messageEvent.emit(this.users.value);
        console.log("task id ",this.users.value);
    });
}

// ngOnChanges(changes: SimpleChanges){
//   if(changes.currentValue != undefined )
//     this.assigneName= changes.currentValue;
//     this.cdf.detectChanges()
//     console.log("agchjdbshdb",this.assigneName);
//   console.log("===> ",changes)
// }

}
