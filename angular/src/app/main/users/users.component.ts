import { Component, Injector, ViewChild } from '@angular/core';
import { Router } from '@angular/router';
import { AppComponentBase } from '@shared/common/app-component-base';
import * as _ from 'lodash';
import { UserServiceProxy } from '@shared/service-proxies/service-proxies';

@Component({
  selector: 'app-users',
  templateUrl: './users.component.html',
  styleUrls: ['./users.component.css']
})
export class UsersComponent extends AppComponentBase {
  totalUserCount: number
  users: any = [];
  timeout: any;
  role = '';
  onlyLockedUsers = false;



  constructor(
    injector: Injector,
    private _router: Router,
    private _userServiceProxy: UserServiceProxy,
  ) {
    super(injector);
  }
  ngOnInit() {
  }


}




