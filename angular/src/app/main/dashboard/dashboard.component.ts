import { Component, Injector, ViewEncapsulation, OnInit } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { DashboardCustomizationConst } from '@app/shared/common/customizable-dashboard/DashboardCustomizationConsts';
import { StoreDateService } from "../../services/storedate.service";
import { UserServiceProxy,AccountSubTypeServiceProxy ,CategoriesServiceProxy} from '@shared/service-proxies/service-proxies';


@Component({
    templateUrl: './dashboard.component.html',
    styleUrls: ['./dashboard.component.less'],
    encapsulation: ViewEncapsulation.None
})


export class DashboardComponent  extends AppComponentBase implements OnInit {
    dashboardName = DashboardCustomizationConst.dashboardNames.defaultTenantDashboard;
    userList :any = []
    categoriesList :any = []
    accountSubTypeList :any = []
    constructor(
        injector: Injector , private storeData: StoreDateService , private _userService: UserServiceProxy,private _categoryService: CategoriesServiceProxy, private _accountSubtypeService :AccountSubTypeServiceProxy)
       {
        super(injector);
    }
    ngOnInit(): void {
           
    }
}
