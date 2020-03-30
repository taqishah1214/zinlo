import { Component, Injector, ViewEncapsulation, OnInit } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { DashboardCustomizationConst } from '@app/shared/common/customizable-dashboard/DashboardCustomizationConsts';
import { UserDateService } from "../../services/user-date.service";
import { UserServiceProxy } from '@shared/service-proxies/service-proxies';


@Component({
    templateUrl: './dashboard.component.html',
    styleUrls: ['./dashboard.component.less'],
    encapsulation: ViewEncapsulation.None
})

export class DashboardComponent  extends AppComponentBase implements OnInit {
    dashboardName = DashboardCustomizationConst.dashboardNames.defaultTenantDashboard;
    userList :any = []
    constructor(
        injector: Injector , private userDate: UserDateService , private _userService: UserServiceProxy,) {
        super(injector);
    }
    ngOnInit(): void {
        this._userService.getAllUsers().subscribe(result => { 
            this.userList = result
            this.userDate.setUserList(this.userList)
        })       
    }
}
