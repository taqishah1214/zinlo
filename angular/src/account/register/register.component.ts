import { AbpSessionService } from '@abp/session/abp-session.service';
import { Component, Injector, OnInit, ViewChild } from '@angular/core';
import { Router } from '@angular/router';
import { accountModuleAnimation } from '@shared/animations/routerTransition';
import { AppComponentBase } from '@shared/common/app-component-base';
import { UserRegisterServiceServiceProxy, RegisterUserInput, PersonalInfoDto, BusinessInfo, PaymentDetails, SubscriptionPlansDto } from '@shared/service-proxies/service-proxies';
import { ViewEncapsulation } from '@angular/core';
import {
    EditionSelectDto,
    EditionWithFeaturesDto,
    EditionsSelectOutput,
    FlatFeatureSelectDto,
    SubscriptionServiceProxy,
    TenantRegistrationServiceProxy,
    EditionPaymentType,
    SubscriptionStartType
} from '@shared/service-proxies/service-proxies';
import * as _ from 'lodash';


declare var $: any;
@Component({
    templateUrl: './register.component.html',
    styleUrls: ['./select-edition.component.less'],
    encapsulation: ViewEncapsulation.None,
    animations: [accountModuleAnimation()]
})
export class RegisterComponent extends AppComponentBase implements OnInit {
    personalInfoDto: PersonalInfoDto;
    businessInfoDto: BusinessInfo;
    paymentDetailsDto: PaymentDetails;
    subscriptionPlansDto: SubscriptionPlansDto;
    editionsSelectOutput: EditionsSelectOutput = new EditionsSelectOutput();
    isUserLoggedIn = false;
    isSetted = false;
    editionPaymentType: typeof EditionPaymentType = EditionPaymentType;
    subscriptionStartType: typeof SubscriptionStartType = SubscriptionStartType;
    /*you can change your edition icons order within editionIcons variable */
    editionIcons: string[] = ['flaticon-open-box', 'flaticon-rocket', 'flaticon-gift', 'flaticon-confetti', 'flaticon-cogwheel-2', 'flaticon-app', 'flaticon-coins', 'flaticon-piggy-bank', 'flaticon-bag', 'flaticon-lifebuoy', 'flaticon-technology-1', 'flaticon-cogwheel-1', 'flaticon-infinity', 'flaticon-interface-5', 'flaticon-squares-3', 'flaticon-interface-6', 'flaticon-mark', 'flaticon-business', 'flaticon-interface-7', 'flaticon-list-2', 'flaticon-bell', 'flaticon-technology', 'flaticon-squares-2', 'flaticon-notes', 'flaticon-profile', 'flaticon-layers', 'flaticon-interface-4', 'flaticon-signs', 'flaticon-menu-1', 'flaticon-symbol'];
    constructor(

        injector: Injector,
        private _tenantRegistrationService: TenantRegistrationServiceProxy,
        private _subscriptionService: SubscriptionServiceProxy,
        private _userRegistrationServiceProxy: UserRegisterServiceServiceProxy,
        private _router: Router
    ) {
        super(injector);
    }

    ngOnInit() {
        this.isUserLoggedIn = abp.session.userId > 0;

        this._tenantRegistrationService.getEditionsForSelect()
            .subscribe((result) => {
                this.editionsSelectOutput = result;

                if (!this.editionsSelectOutput.editionsWithFeatures || this.editionsSelectOutput.editionsWithFeatures.length <= 0) {
                    this._router.navigate(['/account/register-tenant']);
                }
            });
    }

    isFree(edition: EditionSelectDto): boolean {
        return !edition.monthlyPrice && !edition.annualPrice;
    }

    isTrueFalseFeature(feature: FlatFeatureSelectDto): boolean {
        return feature.inputType.name === 'CHECKBOX';
    }

    featureEnabledForEdition(feature: FlatFeatureSelectDto, edition: EditionWithFeaturesDto): boolean {
        const featureValues = _.filter(edition.featureValues, { name: feature.name });
        if (!featureValues || featureValues.length <= 0) {
            return false;
        }

        const featureValue = featureValues[0];
        return featureValue.value.toLowerCase() === 'true';
    }

    getFeatureValueForEdition(feature: FlatFeatureSelectDto, edition: EditionWithFeaturesDto): string {
        const featureValues = _.filter(edition.featureValues, { name: feature.name });
        if (!featureValues || featureValues.length <= 0) {
            return '';
        }

        const featureValue = featureValues[0];
        return featureValue.value;
    }

    upgrade(upgradeEdition: EditionSelectDto, editionPaymentType: EditionPaymentType): void {
        this._router.navigate(['/account/upgrade'], { queryParams: { upgradeEditionId: upgradeEdition.id, editionPaymentType: editionPaymentType } });
    }
    startSubscription(id: any, type: any, payment: any) {
        this.subscriptionPlansDto.editionId = id
        this.subscriptionPlansDto.subscriptionStartType = type
        this.subscriptionPlansDto.editionPaymentType = payment
    }
    trailSubscription(id: any, type: any, payment: any) {
        this.subscriptionPlansDto.editionId = id
        this.subscriptionPlansDto.subscriptionStartType = type
        this.subscriptionPlansDto.editionPaymentType = payment
    }
    buySubscription(id: any, type: any, payment: any) {
        this.subscriptionPlansDto.editionId = id
        this.subscriptionPlansDto.subscriptionStartType = type
        this.subscriptionPlansDto.editionPaymentType = payment
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
            $(".submit").show();
        } else {
            $(".nxtbtn").show();
            $(".submit").hide();
        }

        this.switchTab(this.current);
    }

    save() {
        var userResgister = new RegisterUserInput();
        userResgister.businessInfo = this.businessInfoDto;
        userResgister.paymentDetails = this.paymentDetailsDto;
        userResgister.personalInfo = this.personalInfoDto;
        userResgister.subscriptionPlans = this.subscriptionPlansDto;
        console.log(userResgister);
        debugger;
        this._userRegistrationServiceProxy.registerUserWithTenant(userResgister);

        //call the service method to save the result
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
