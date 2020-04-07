import { Component, Injector, OnInit, ViewChild } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import * as _ from 'lodash';
import { UserServiceProxy } from '@shared/service-proxies/service-proxies';
import { CreateOrEditUserModalComponent } from '../../admin/users/create-or-edit-user-modal.component';
import { EditUserPermissionsModalComponent } from '../../admin/users/edit-user-permissions-modal.component';
import { appModuleAnimation } from '@shared/animations/routerTransition';



@Component({
  selector: 'app-users',
  templateUrl: './users.component.html',
  styleUrls: ['./users.component.css'],
  animations: [appModuleAnimation()]

})
export class UsersComponent  {


  }





