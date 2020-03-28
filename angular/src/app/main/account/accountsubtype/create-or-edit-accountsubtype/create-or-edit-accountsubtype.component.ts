import { Component, OnInit, Injector } from '@angular/core';
import { AccountSubTypeServiceProxy, CreateOrEditAccountSubTypeDto } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import { Router } from '@angular/router';
import { finalize } from 'rxjs/operators';


@Component({
  selector: 'app-create-or-edit-accountsubtype',
  templateUrl: './create-or-edit-accountsubtype.component.html',
  styleUrls: ['./create-or-edit-accountsubtype.component.css']
})
export class CreateOrEditAccountsubtypeComponent extends AppComponentBase implements OnInit {
  accountsSubType : CreateOrEditAccountSubTypeDto = new CreateOrEditAccountSubTypeDto()
  accountSubTypeId : number;
  saving = false;
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
    this.saving = true;  
   this.accountSubTypeServiceProxy.createOrEdit(this.accountsSubType).pipe(finalize(() => { this.saving = false; })).subscribe(result => {
    this.notify.success(this.l('SavedSuccessfully'));
    this.redirect(this.accountsSubType.title,result);
    
   })
  }
  redirectToSubAccountsList () : void {
    this._router.navigate(['/app/main/account/accountsubtype']);
  }
  redirect(title,selectedId) : void {
    
    let accountId = history.state.data.accountId;
    let previousRoute = history.state.data.previousRoute;
    if (accountId != 0 && previousRoute == "account")
    {
      this._router.navigate(['/app/main/account/accounts/create-edit-accounts'], { state: { data: { id : accountId, newSubTypeId : selectedId,newSubTypeTitle : title} } });
    }
    else if (previousRoute == "account") {
      this._router.navigate(['/app/main/account/accounts/create-edit-accounts'], { state: { data: { id : 0, newSubTypeId : selectedId,newSubTypeTitle : title} } });
    }
    else
    {
    this._router.navigate(['/app/main/account/accountsubtype']);  
    }
  }

}
