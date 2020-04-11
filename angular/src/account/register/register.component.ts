import { Component, Injector, OnInit, ViewChild } from '@angular/core';
import { Router } from '@angular/router';
import { AppConsts } from '@shared/AppConsts';
import { accountModuleAnimation } from '@shared/animations/routerTransition';
import { AppComponentBase } from '@shared/common/app-component-base';
import { AccountServiceProxy, PasswordComplexitySetting, ProfileServiceProxy, RegisterOutput } from '@shared/service-proxies/service-proxies';
import { LoginService } from '../login/login.service';
import { RegisterModel } from './register.model';
import { finalize, catchError } from 'rxjs/operators';
import { RecaptchaComponent } from 'ng-recaptcha';

declare var $: any;
@Component({
    templateUrl: './register.component.html',
    animations: [accountModuleAnimation()]
})
export class RegisterComponent extends AppComponentBase implements OnInit {

    constructor(
        injector: Injector,
    ) {
        super(injector);
    }

    ngOnInit() {
    }

    current: number = 0;
    movetab(item: number) {

        this.current = this.current + item;
        if (this.current > 0) {
            $(".previebtn").show();
        } else {
            $(".previebtn").hide();

        }
        
        if (this.current === 4) {
            $(".nxtbtn").hide();
        } else {
            $(".nxtbtn").show();
        }

        this.switchTab(this.current);
    }

    switchTab(item: number) {
        switch (item) {
            case 0:
                $(".location").attr("data-ktwizard-state", "current")
                $(".detailbody").attr("data-ktwizard-state", "")
                $(".servicebody").attr("data-ktwizard-state", "")
                $(".deliverybody").attr("data-ktwizard-state", "")
                $(".reviewbody").attr("data-ktwizard-state", "")
                $(".locationbody").attr("data-ktwizard-state", "current")
                break;
            case 1:
                $(".detail").attr("data-ktwizard-state", "current")
                $(".detailbody").attr("data-ktwizard-state", "current")
                $(".servicebody").attr("data-ktwizard-state", "")
                $(".deliverybody").attr("data-ktwizard-state", "")
                $(".reviewbody").attr("data-ktwizard-state", "")
                $(".locationbody").attr("data-ktwizard-state", "")

                break;
            case 2:
                $(".service").attr("data-ktwizard-state", "current")
                $(".detailbody").attr("data-ktwizard-state", "")
                $(".servicebody").attr("data-ktwizard-state", "current")
                $(".deliverybody").attr("data-ktwizard-state", "")
                $(".reviewbody").attr("data-ktwizard-state", "")
                $(".locationbody").attr("data-ktwizard-state", "")
                break;
            case 3:
                $(".delivery").attr("data-ktwizard-state", "current")
                $(".detailbody").attr("data-ktwizard-state", "")
                $(".servicebody").attr("data-ktwizard-state", "")
                $(".deliverybody").attr("data-ktwizard-state", "current")
                $(".reviewbody").attr("data-ktwizard-state", "")
                $(".locationbody").attr("data-ktwizard-state", "")

                break;
            case 4:
                $(".review").attr("data-ktwizard-state", "current")
                $(".detailbody").attr("data-ktwizard-state", "")
                $(".servicebody").attr("data-ktwizard-state", "")
                $(".deliverybody").attr("data-ktwizard-state", "")
                $(".reviewbody").attr("data-ktwizard-state", "current")
                $(".locationbody").attr("data-ktwizard-state", "")

                break;
            default: break;
        }
    }
    tabpClick(item: number) {
        this.switchTab(item);
    }



}
