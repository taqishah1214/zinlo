import { Component, OnInit, Injector } from '@angular/core';
import { ReactiveFormsModule,FormGroup, FormControl} from '@angular/forms';
import { Validators } from '@angular/forms';
import { RegisterTenantUserInput, AccountServiceProxy, SessionServiceProxy, CustomTenantRequestLinkResolveInput, InviteUserServiceServiceProxy } from '@shared/service-proxies/service-proxies';
import { AbpSessionService } from 'abp-ng2-module/dist/src/session/abp-session.service';
import { Router, ActivatedRoute } from '@angular/router';
import { AppComponentBase } from '@shared/common/app-component-base';

@Component({
  selector: 'tenantUserRegister',
  templateUrl: './tenant-user-register.component.html',
})

export class TenantUserRegisterComponent extends AppComponentBase implements OnInit{
  
  
  personalInfoForm:FormGroup;
  tenantUserInfoDto:RegisterTenantUserInput;
  saving = false;
  
  constructor(
    injector: Injector,
    private _accountServiceProxy:AccountServiceProxy,
    private _sessionService: AbpSessionService,
    private _router: Router,
    private _inviteUserService:InviteUserServiceServiceProxy,
    private _activatedRoute: ActivatedRoute,
  ){
    super(injector);
      
    this.tenantUserInfoDto= new RegisterTenantUserInput();
  }
  ngOnInit()
  {

    if (this._activatedRoute.snapshot.queryParams['c']) {
      var c = this._activatedRoute.snapshot.queryParams['c'];
      this._accountServiceProxy.regsiterLinkResolve(new CustomTenantRequestLinkResolveInput({ c: c })).subscribe((response) => {
       this._inviteUserService.getByEmail(response.email).subscribe((res)=>
        {
          this.personalInfoForm = new FormGroup({
            firstName: new FormControl(''+res.firstName,[Validators.required]),
            lastName: new FormControl(res.lastName,[Validators.required]),
            userName:new FormControl(res.userName,[Validators.required]),
            password:new FormControl('',[Validators.required]),
            emailAddress: new FormControl(res.email,[Validators.required,Validators.pattern("^[a-z0-9._%+-]+@[a-z0-9.-]+\.[a-z]{2,4}$")]),
            title: new FormControl(res.title,[Validators.required]),
            phone: new FormControl(res.phoneNumber,[Validators.pattern("^(\([0-9]{3}\) |[0-9]{3}-)[0-9]{3}-[0-9]{4}$")]),
            address: new FormControl('',[Validators.required]),
            city: new FormControl(''),
            state: new FormControl(''),
          });

        })
          abp.multiTenancy.setTenantIdCookie(response.tenantId);
      });

  }

  

   this.personalInfoForm = new FormGroup({
    firstName: new FormControl('',[Validators.required]),
    lastName: new FormControl('',[Validators.required]),
    userName:new FormControl('',[Validators.required]),
    password:new FormControl('',[Validators.required]),
    emailAddress: new FormControl('',[Validators.required,Validators.pattern("^[a-z0-9._%+-]+@[a-z0-9.-]+\.[a-z]{2,4}$")]),
    title: new FormControl('',[Validators.required]),
    phone: new FormControl('',[Validators.pattern("^(\([0-9]{3}\) |[0-9]{3}-)[0-9]{3}-[0-9]{4}$")]),
    address: new FormControl('',[Validators.required]),
    city: new FormControl(''),
    state: new FormControl(''),
  });

}
  FormGroup(arg0: { firstName: FormControl; lastName: FormControl; }) {
    throw new Error("Method not implemented.");
  }
  register(){
      this.saving=true;
      this.tenantUserInfoDto.name=this.personalInfoForm.value.firstName;
      this.tenantUserInfoDto.surname=this.personalInfoForm.value.lastName;
      this.tenantUserInfoDto.emailAddress=this.personalInfoForm.value.emailAddress;
      this.tenantUserInfoDto.title=this.personalInfoForm.value.title;
      this.tenantUserInfoDto.phoneNumber=this.personalInfoForm.value.phone;
      this.tenantUserInfoDto.address=this.personalInfoForm.value.address;
      this.tenantUserInfoDto.city=this.personalInfoForm.value.city;
      this.tenantUserInfoDto.state=this.personalInfoForm.value.state;
      this.tenantUserInfoDto.tenantId=this._sessionService.tenantId;
      this.tenantUserInfoDto.userName = this.personalInfoForm.value.userName;
      this.tenantUserInfoDto.password=this.personalInfoForm.value.password;
      this._accountServiceProxy.registerTenantUser(this.tenantUserInfoDto).subscribe(result=>
        {
          this.notify.success(this.l('SuccessfullyRegistered'));
              this._router.navigate(['account/tenentRegisterUserResult']);
        });
      
        this.saving=false;
  }
  get firstName() { return this.personalInfoForm.get('firstName'); }
  get lastName() { return this.personalInfoForm.get('lastName'); }
  get emailAddress() { return this.personalInfoForm.get('emailAddress'); }
  get address() { return this.personalInfoForm.get('address'); }
  get title() { return this.personalInfoForm.get('title'); }
  get userName() { return this.personalInfoForm.get('userName'); }
  get password() { return this.personalInfoForm.get('password'); }
  get phone() { return this.personalInfoForm.get('phone'); }
  
}
