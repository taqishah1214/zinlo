import { Component, OnInit, EventEmitter, Output } from '@angular/core';
import { UserServiceProxy} from '@shared/service-proxies/service-proxies';


@Component({
  selector: 'app-user-list-component',
  templateUrl: './user-list-component.component.html',
  styleUrls: ['./user-list-component.component.css']
})
export class UserListComponentComponent implements OnInit {

  users : any;
  public selectedUserId:any;
  @Output() messageEvent = new EventEmitter<string>();
  constructor( private userService :UserServiceProxy ) {

   }

  ngOnInit() {
    this.userService.userDropDown().subscribe(result => {
      this.users = result;
  });
  }

  userOnChange() : void {
    this.messageEvent.emit(this.selectedUserId);
  }

}
