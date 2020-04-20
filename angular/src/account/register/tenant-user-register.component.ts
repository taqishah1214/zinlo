import { Component, OnInit } from '@angular/core';
import { ReactiveFormsModule,FormGroup, FormControl} from '@angular/forms';
import { Validators } from '@angular/forms';
@Component({
  selector: 'tenantUserRegister',
  templateUrl: './tenant-user-register.component.html',
})

export class TenantUserRegisterComponent implements OnInit {
  
  
  personalInfoForm:FormGroup;
  ngOnInit()
  {
   this.personalInfoForm = new FormGroup({
    firstName: new FormControl('',[Validators.required]),
    lastName: new FormControl('',[Validators.required]),
    emailAddress: new FormControl('',[Validators.required,Validators.pattern("^[a-z0-9._%+-]+@[a-z0-9.-]+\.[a-z]{2,4}$")]),
    title: new FormControl('',[Validators.required]),
    phone: new FormControl(''),
    address: new FormControl('',[Validators.required]),
    city: new FormControl(''),
    state: new FormControl(''),
  });
}
  FormGroup(arg0: { firstName: FormControl; lastName: FormControl; }) {
    throw new Error("Method not implemented.");
  }
  register(){
      console.log(this.personalInfoForm);
  }
  get firstName() { return this.personalInfoForm.get('firstName'); }
//   get lastName() { return this.personalInfoForm.get('lastName'); }
//   get email() { return this.personalInfoForm.get('email'); }
//   get phone() { return this.personalInfoForm.get('phone'); }
//   get password() { return this.personalInfoForm.get('password'); }
  
}
