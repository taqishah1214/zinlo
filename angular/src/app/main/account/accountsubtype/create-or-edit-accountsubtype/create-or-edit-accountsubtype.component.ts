import { Component, OnInit, Injector } from '@angular/core';
import { AccountSubTypeServiceProxy, CreateOrEditAccountSubTypeDto } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import { Router } from '@angular/router';

@Component({
  selector: 'app-create-or-edit-accountsubtype',
  templateUrl: './create-or-edit-accountsubtype.component.html',
  styleUrls: ['./create-or-edit-accountsubtype.component.css']
})
export class CreateOrEditAccountsubtypeComponent extends AppComponentBase implements OnInit {
  accountsSubType : CreateOrEditAccountSubTypeDto = new CreateOrEditAccountSubTypeDto()
  accountSubTypeId : number;
  constructor(private accountSubTypeServiceProxy: AccountSubTypeServiceProxy , injector: Injector,private _router: Router) {
    super(injector) 
  }

  ngOnInit() {
    this.accountSubTypeId = history.state.data.id;
    if (this.accountSubTypeId > 0) {
      this.accountSubTypeServiceProxy.getAccountSubTypeForEdit(this.accountSubTypeId).subscribe(result => {
        this.accountsSubType = result;
      });
    }
  }

  onSubmit() : void {
   this.accountSubTypeServiceProxy.createOrEdit(this.accountsSubType).subscribe(result => {
    this.notify.success(this.l('SavedSuccessfully'));
    this._router.navigate(['/app/main/account/accountsubtype']);
   })
  }

}
