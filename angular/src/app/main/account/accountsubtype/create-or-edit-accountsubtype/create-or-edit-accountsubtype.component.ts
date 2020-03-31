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
      this._router.navigate(['/app/main/account/accounts/create-edit-accounts'], { state: { data: { id : accountId, newSubTypeId : selectedId,newSubTypeTitle : title ,name:history.state.data.name,number:history.state.data.number, type:history.state.data.type, reconciliation:history.state.data.reconciliation, typeName:history.state.data.typeName,reconcillationName:history.state.data.reconcillationName,userId:history.state.data.userId,reconciledID:history.state.data.reconciledID,reconciledName:history.state.data.reconciledName} } });
    }
    else if (previousRoute == "account") {
      this._router.navigate(['/app/main/account/accounts/create-edit-accounts'], { state: { data: { id : 0, newSubTypeId : selectedId,newSubTypeTitle : title,name:history.state.data.name,number:history.state.data.number, type:history.state.data.type, reconciliation:history.state.data.reconciliation, typeName:history.state.data.typeName,reconcillationName:history.state.data.reconcillationName,userId:history.state.data.userId,reconciledID:history.state.data.reconciledID,reconciledName:history.state.data.reconciledName} } });
    }
    else
    {
    this._router.navigate(['/app/main/account/accountsubtype']);  
    }
  }

}
