import { Component, Injector, OnInit } from '@angular/core';
import { accountModuleAnimation } from '@shared/animations/routerTransition';
import { AppComponentBase } from '@shared/common/app-component-base';

@Component({
    templateUrl: './tenant-register-user-result.component.html',
    animations: [accountModuleAnimation()]
})
export class TenentRegisterUserComponent extends AppComponentBase implements OnInit {

    tenantUrl: string;

    saving = false;

    constructor(
        injector: Injector,
    ) {
        super(injector);
    }
    ngOnInit(): void {
       
    }


}