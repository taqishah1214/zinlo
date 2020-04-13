import { AbpSessionService } from '@abp/session/abp-session.service';
import { Component, Injector, OnInit, ViewChild } from '@angular/core';
import { Router } from '@angular/router';
import { accountModuleAnimation } from '@shared/animations/routerTransition';
import { AppComponentBase } from '@shared/common/app-component-base';
import { SessionServiceProxy, UpdateUserSignInTokenOutput } from '@shared/service-proxies/service-proxies';
import { UrlHelper } from 'shared/helpers/UrlHelper';
import { RecaptchaComponent } from 'ng-recaptcha';
import { AppConsts } from '@shared/AppConsts';


declare var $: any;
@Component({
    templateUrl: './register.component.html',
    animations: [accountModuleAnimation()]
})
export class RegisterComponent extends AppComponentBase implements OnInit {
    firstName:string;
    lastName:string;
    email:string;
    title:string;
    password:string;
    repeatPassword:string;
    businessName:string;
    website:string;
    phoneNumber:string;
    addressLineOne:string;
    addressLineTwo:string;
    city:string;
    state:string;
    cardNumber:string;
    zipCode:string;
    CVVCode:string;
    expiryDate:string;
    commitment:string;
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
