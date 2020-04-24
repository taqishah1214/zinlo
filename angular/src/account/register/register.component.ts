import { AbpSessionService } from '@abp/session/abp-session.service';
import { Component, Injector, OnInit, ViewChild } from '@angular/core';
import { ReactiveFormsModule,FormGroup, FormControl, Validators} from '@angular/forms';
import { Router } from '@angular/router';
import { accountModuleAnimation } from '@shared/animations/routerTransition';
import { AppComponentBase } from '@shared/common/app-component-base';
import { finalize, catchError } from 'rxjs/operators';
import { UserRegisterServiceServiceProxy, RegisterUserInput, PersonalInfoDto, BusinessInfo, PaymentDetails, SubscriptionPlansDto, RegisterTenantOutput, CreateOrUpdateContactusInput } from '@shared/service-proxies/service-proxies';
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
import { TenantRegistrationHelperService } from './tenant-registration-helper.service';


declare var $: any;
@Component({
    templateUrl: './register.component.html',
    styleUrls: ['./select-edition.component.less'],
    encapsulation: ViewEncapsulation.None,
    animations: [accountModuleAnimation()]
})
export class RegisterComponent extends AppComponentBase implements OnInit {
    personalInfoForm:FormGroup;
    businessInfoForm:FormGroup;
    paymentDetailsForm:FormGroup;
    contactUsForm:FormGroup;
    personalInfoDto: PersonalInfoDto;
    businessInfoDto: BusinessInfo;
    paymentDetailsDto: PaymentDetails;
    contactUs:CreateOrUpdateContactusInput;
    subscriptionPlansDto: SubscriptionPlansDto;
    editionsSelectOutput: EditionsSelectOutput = new EditionsSelectOutput();
    isUserLoggedIn = false;
    isSetted = false;
    saving = false;
    custom=true;
    editionPaymentType: typeof EditionPaymentType = EditionPaymentType;
    subscriptionStartType: typeof SubscriptionStartType = SubscriptionStartType;
    /*you can change your edition icons order within editionIcons variable */
    editionIcons: string[] = ['./assets/media/ClosingCheckList/startup.svg','./assets/media/ClosingCheckList/premium.svg','./assets/media/ClosingCheckList/Business_info.ico'];
    constructor(

        injector: Injector,
        private _tenantRegistrationService: TenantRegistrationServiceProxy,
        private _subscriptionService: SubscriptionServiceProxy,
        private _userRegistrationServiceProxy: UserRegisterServiceServiceProxy,
        private _tenantRegistrationHelper: TenantRegistrationHelperService,
        private _router: Router
    ) {
        super(injector);
        this.personalInfoDto=new PersonalInfoDto();
        this.businessInfoDto= new BusinessInfo();
        this.paymentDetailsDto= new PaymentDetails();
        this.subscriptionPlansDto= new SubscriptionPlansDto();
        this.contactUs=new CreateOrUpdateContactusInput();
    }
    onOpenCalendar(container) {
        container.monthSelectHandler = (event: any): void => {
          container._store.dispatch(container._actions.select(event.date));
        };
        container.setViewMode('month');
      }
    ngOnInit() {
        this.personalInfoForm = new FormGroup({
            firstName: new FormControl('',[Validators.required]),
            lastName: new FormControl('',[Validators.required]),
            password:new FormControl('',[Validators.required]),
            confirmPassword:new FormControl(''),
            emailAddress: new FormControl('',[Validators.required,Validators.pattern("^[a-z0-9._%+-]+@[a-z0-9.-]+\.[a-z]{2,4}$")]),
            title: new FormControl('')
        })
        this.businessInfoForm = new FormGroup({
            businessName: new FormControl('',[Validators.required]),
            tenantName: new FormControl('',[Validators.required]),
            website:new FormControl(''),
            title: new FormControl('',[Validators.required]),
            phone: new FormControl('',[Validators.pattern("^[0-9]+")]),
            addressLineOne: new FormControl('',[Validators.required]),
            addressLineTwo: new FormControl(''),
            zipCode: new FormControl(''),
            city: new FormControl('NewYark'),
            state: new FormControl('NewYark'),
        })
        this.paymentDetailsForm = new FormGroup({
            cardNumber: new FormControl('',[Validators.required]),
            cvvCode: new FormControl('',[Validators.required]),
            expiryDate:new FormControl('',[Validators.required]),
            email: new FormControl('',[Validators.required,Validators.pattern("^[a-z0-9._%+-]+@[a-z0-9.-]+\.[a-z]{2,4}$")]),
            commitment: new FormControl(0,[Validators.required])
        })
        this.contactUsForm= new FormGroup({
            fullName: new FormControl('',[Validators.required]),
            description: new FormControl('',[Validators.required]),
            companyName:new FormControl('',[Validators.required]),
            contactEmail: new FormControl('',[Validators.required,Validators.pattern("^[a-z0-9._%+-]+@[a-z0-9.-]+\.[a-z]{2,4}$")]),
            numberOfUsers: new FormControl(0,[Validators.required])
        })
        this.paymentDetailsDto.commitment=0
        this.isUserLoggedIn = abp.session.userId > 0;
        
        this._tenantRegistrationService.getEditionsForSelect()
            .subscribe((result) => {
                this.editionsSelectOutput = result;

                if (!this.editionsSelectOutput.editionsWithFeatures || this.editionsSelectOutput.editionsWithFeatures.length <= 0) {
                   // this._router.navigate(['/account/register-tenant']);
                }
            });
    }

    isFree(edition: EditionSelectDto): boolean {
        return !edition.monthlyPrice && !edition.annualPrice;
    }

    isTrueFalseFeature(feature: FlatFeatureSelectDto): boolean {
        return feature.inputType.name === 'CHECKBOX';
    }
    get companyName() { return this.contactUsForm.get('companyName'); }
    get numberOfUsers() { return this.contactUsForm.get('numberOfUsers'); }
    get description() { return this.contactUsForm.get('description'); }
    get contactEmail() { return this.contactUsForm.get('contactEmail'); }
    get fullName() { return this.contactUsForm.get('fullName'); }
    get firstName() { return this.personalInfoForm.get('firstName'); }
    get lastName() { return this.personalInfoForm.get('lastName'); }
    get emailAddress() { return this.personalInfoForm.get('emailAddress'); }
    get title() { return this.personalInfoForm.get('title'); }
    get password() { return this.personalInfoForm.get('password'); }
    get tenantName() { return this.businessInfoForm.get('tenantName'); }
    get businessName() { return this.businessInfoForm.get('businessName'); }
    get zipCode() { return this.businessInfoForm.get('zipCode'); }
    get addressLineOne() { return this.businessInfoForm.get('addressLineOne'); }
    get phone() { return this.businessInfoForm.get('phone'); }
    get commitment() { return this.paymentDetailsForm.get('commitment'); }
    get cvvCode() { return this.paymentDetailsForm.get('cvvCode'); }
    get cardNumber() { return this.paymentDetailsForm.get('cardNumber'); }
    get expiryDate() { return this.paymentDetailsForm.get('expiryDate'); }
    get email() { return this.paymentDetailsForm.get('email'); }
    
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
        this.custom=true;
        this.movetab(1);
    }
    trailSubscription(id: any, type: any, payment: any) {
        this.subscriptionPlansDto.editionId = id
        this.subscriptionPlansDto.subscriptionStartType = type
        this.subscriptionPlansDto.editionPaymentType = payment
        this.custom=false;
    }
    buySubscription(id: any, type: any, payment: any) {
        this.subscriptionPlansDto.editionId = id
        this.subscriptionPlansDto.subscriptionStartType = type
        this.subscriptionPlansDto.editionPaymentType = payment
        this.custom=false;
        this.movetab(1);
    }
    current: number = 0;
    movetab(item: number) {
        if(this.personalInfoForm.value.confirmPassword==this.personalInfoForm.value.password)
        {
            
            this.current = this.current + item;
            
            this.switchTab(this.current);
            if (this.current > 0) {
                $(".previebtn").show();
            } else {
                $(".previebtn").hide();

            }
            

            if (this.current === 3) {
                $(".nxtbtn").hide();
                $(".submit").show();
            } else {
                $(".nxtbtn").show();
                $(".submit").hide();
            }
        }
        else{
            this.notify.error("Password and Confirm Password are not same")
        }
    }
    personalInfoValidations(){
        if(this.personalInfoForm.value.firstName == "")
        {
            this.notify.error("First Name is Required")
            return false;
        }
        else if(this.personalInfoForm.value.lastName == "")
        {
            this.notify.error("Last Name is Required")
            return false;
        }
        else if(this.personalInfoForm.value.emailAddress == "")
        {
            this.notify.error("Email is Required")
            return false;
        }
        else if(this.personalInfoForm.value.password == "")
        {
            this.notify.error("Password is Required")
            return false;
        }
        return true;
    }
    businessInfoValidations(){
        console.log("Moeen")
        if(this.businessInfoForm.value.businessName == "")
        {
            this.notify.error("Business Name is Required")
            return false;
        }
        else if(this.businessInfoForm.value.tenantName == "")
        {
            this.notify.error("Tenant Name is Required")
            return false;
        }
        else if(this.businessInfoForm.value.addressLineOne == "")
        {
            this.notify.error("Address is Required")
            return false;
        }
        else if(this.businessInfoForm.value.zipCode == "")
        {
            this.notify.error("ZipCode is Required")
            return false;
        }
        return true;
    }
    paymentInfoValidations(){
        if(this.paymentDetailsForm.value.cardNumber == "")
        {
            this.notify.error("Card Number is Required")
            return false;
        }
        else if(this.paymentDetailsForm.value.cvvCode == "")
        {
            this.notify.error("CVV Code is Required")
            return false;
        }
        else if(!this.paymentDetailsForm.value.expiryDate)
        {
            this.notify.error("Select Expiry Date")
            return false;
        }
        else if(this.paymentDetailsForm.value.email == "")
        {
            this.notify.error("Email is Required")
            return false;
        }
        return true;
    }
    contactUsValidations(){
        if(this.contactUsForm.value.fullName == "")
        {
            this.notify.error("Name is Required")
            return false;
        }
        else if(this.contactUsForm.value.contactEmail == "")
        {
            
            this.notify.error("Email is Required")
            return false;
        }
        else if(!this.contactUsForm.value.companyName)
        {
            this.notify.error("Company Name is required")
            return false;
        }
        else if(this.contactUsForm.value.description == "")
        {
            this.notify.error("Description is Required")
            return false;
        }
        return true;
    }
    save() {
        if(!this.custom){
            if(this.paymentInfoValidations())
            {
                this.paymentDetailsDto.cardNumber=this.paymentDetailsForm.value.cardNumber
                this.paymentDetailsDto.cvvCode=this.paymentDetailsForm.value.cvvCode
                this.paymentDetailsDto.commitment=this.paymentDetailsForm.value.commitment
                this.paymentDetailsDto.expiryDate=this.paymentDetailsForm.value.expiryDate
                this.paymentDetailsDto.email=this.paymentDetailsForm.value.email
                this.saving = true;
                var userResgister = new RegisterUserInput();
                userResgister.businessInfo = this.businessInfoDto;
            
                userResgister.personalInfo = this.personalInfoDto;
                userResgister.subscriptionPlans = this.subscriptionPlansDto;
                if(this.custom)
                {
                userResgister.contactUs = this.contactUs;
                userResgister.paymentDetails=null;
                }else{
                    userResgister.paymentDetails = this.paymentDetailsDto;
                    userResgister.contactUs=null;
                }
                console.log(userResgister);
                debugger;
            this._userRegistrationServiceProxy.registerUserWithTenant(userResgister)
            .pipe(finalize(() => { this.saving = false; }))
                    .pipe(catchError((err, caught): any => {
                        
                    }))
                    .subscribe((result: RegisterTenantOutput) => {
                        this.notify.success(this.l('SuccessfullyRegistered'));
                        this._tenantRegistrationHelper.registrationResult = result;
    
                        if (parseInt(userResgister.subscriptionPlans.subscriptionStartType.toString()) === SubscriptionStartType.Paid) {
                            this._router.navigate(['account/buy'],
                                {
                                    queryParams: {
                                        tenantId: result.tenantId,
                                        editionId: userResgister.subscriptionPlans.editionId,
                                        subscriptionStartType: userResgister.subscriptionPlans.subscriptionStartType,
                                        editionPaymentType: this.editionPaymentType
                                    }
                                });
                        } else {
                            this._router.navigate(['account/register-tenant-result']);
                        }
                    });
            }

        }
        else{
            if(this.contactUsValidations()){
                this.contactUs.companyName=this.contactUsForm.value.companyName
                this.contactUs.fullName=this.contactUsForm.value.fullName
                this.contactUs.email=this.contactUsForm.value.contactEmail
                this.contactUs.description=this.contactUsForm.value.description
                this.contactUs.numberOfUsers=this.contactUsForm.value.numberOfUsers
                this.saving = true;
                var userResgister = new RegisterUserInput();
                userResgister.businessInfo = this.businessInfoDto;
            
                userResgister.personalInfo = this.personalInfoDto;
                userResgister.subscriptionPlans = this.subscriptionPlansDto;
                if(this.custom)
                {
                userResgister.contactUs = this.contactUs;
                userResgister.paymentDetails=null;
                }else{
                    userResgister.paymentDetails = this.paymentDetailsDto;
                    userResgister.contactUs=null;
                }
                console.log(userResgister);
                debugger;
            this._userRegistrationServiceProxy.registerUserWithTenant(userResgister)
            .pipe(finalize(() => { this.saving = false; }))
                    .pipe(catchError((err, caught): any => {
                        
                    }))
                    .subscribe((result: RegisterTenantOutput) => {
                        this.notify.success(this.l('SuccessfullyRegistered'));
                        this._tenantRegistrationHelper.registrationResult = result;

                        if (parseInt(userResgister.subscriptionPlans.subscriptionStartType.toString()) === SubscriptionStartType.Paid) {
                            this._router.navigate(['account/buy'],
                                {
                                    queryParams: {
                                        tenantId: result.tenantId,
                                        editionId: userResgister.subscriptionPlans.editionId,
                                        subscriptionStartType: userResgister.subscriptionPlans.subscriptionStartType,
                                        editionPaymentType: this.editionPaymentType
                                    }
                                });
                        } else {
                            this._router.navigate(['account/register-tenant-result']);
                        }
                    });
            }
        }
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

                if(this.personalInfoValidations()){
                    this.personalInfoDto.userName=this.personalInfoForm.value.firstName+" "+this.personalInfoForm.value.lastName
                    this.personalInfoDto.emailAddress=this.personalInfoForm.value.emailAddress
                    this.personalInfoDto.password=this.personalInfoForm.value.password
                    this.personalInfoDto.title=this.personalInfoForm.value.title
                    $(".detail").attr("data-ktwizard-state", "current")
                    $(".detailbody").attr("data-ktwizard-state", "current")
                    $(".servicebody").attr("data-ktwizard-state", "")
                    $(".deliverybody").attr("data-ktwizard-state", "")
                    $(".reviewbody").attr("data-ktwizard-state", "")
                    $(".locationbody").attr("data-ktwizard-state", "")
                }
                else{
                    this.current--;
                }
                break;
            case 2:
                
                if(this.businessInfoValidations()){
                    this.businessInfoDto.businessName=this.businessInfoForm.value.businessName
                    this.businessInfoDto.tenantName=this.businessInfoForm.value.tenantName
                    this.businessInfoDto.phoneNumber=this.businessInfoForm.value.phoneNumber
                    this.businessInfoDto.state=this.businessInfoForm.value.state
                    this.businessInfoDto.city=this.businessInfoForm.value.city
                    this.businessInfoDto.addressLineOne=this.businessInfoForm.value.addressLineOne
                    this.businessInfoDto.addressLineTwo=this.businessInfoForm.value.addressLineTwo
                    this.businessInfoDto.website=this.businessInfoForm.value.website
                    this.businessInfoDto.zipCode=this.businessInfoForm.value.zipCode
                    $(".service").attr("data-ktwizard-state", "current")
                    $(".detailbody").attr("data-ktwizard-state", "")
                    $(".servicebody").attr("data-ktwizard-state", "current")
                    $(".deliverybody").attr("data-ktwizard-state", "")
                    $(".reviewbody").attr("data-ktwizard-state", "")
                    $(".locationbody").attr("data-ktwizard-state", "")
                }
                else{
                    this.current--;
                }
                break;
            case 3:
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
