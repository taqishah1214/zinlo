import { Component, OnInit, Injector } from '@angular/core';
import { AccountSubTypeServiceProxy, CreateOrEditAccountSubTypeDto } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import { Router } from '@angular/router';
import { finalize } from 'rxjs/operators';
import { StoreDateService } from '../../../../services/storedate.service';


@Component({
  selector: 'app-create-or-edit-accountsubtype',
  templateUrl: './create-or-edit-accountsubtype.component.html',
  styleUrls: ['./create-or-edit-accountsubtype.component.css']
})
export class CreateOrEditAccountsubtypeComponent extends AppComponentBase implements OnInit {
  accountsSubType : CreateOrEditAccountSubTypeDto = new CreateOrEditAccountSubTypeDto()
  accountSubTypeId : number;
  accountSubTypeList :any = []
  accountSubtypeExist : boolean = false
  saving = false;
  constructor(private storeData: StoreDateService,private accountSubTypeServiceProxy: AccountSubTypeServiceProxy , injector: Injector,private _router: Router) {
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

  CheckAccountSubType() { 
    this.accountsSubType.title
    this.accountSubTypeServiceProxy.isAccountSubTypeExist(this.accountsSubType.title).subscribe(resp => {
      this.accountSubtypeExist = resp
    })
    
  }

  onSubmit() : void {
    this.saving = true;  
   this.accountSubTypeServiceProxy.createOrEdit(this.accountsSubType).pipe(finalize(() => { this.saving = false; })).subscribe(result => {
    this.notify.success(this.l('SavedSuccessfully'));
    this.accountSubTypeServiceProxy.accountSubTypeDropDown().subscribe(result => { 
      this.accountSubTypeList = result
      this.storeData.setAccountSubTypeList(this.accountSubTypeList)
  })  
    this.redirect(this.accountsSubType.title,result);
    
   })
  }
  redirectToSubAccountsList () : void {
    this.redirect('','')
  }
  redirect(title,selectedId) : void {
    
    let accountId = history.state.data.accountId;
    let previousRoute = history.state.data.previousRoute;
    if (accountId != 0 && previousRoute == "account")
    {
      this._router.navigate(['/app/main/account/create-edit-accounts'], { state: { data: { id : accountId, newSubTypeId : selectedId,newSubTypeTitle : title ,name:history.state.data.name,number:history.state.data.number, type:history.state.data.type, reconciliation:history.state.data.reconciliation, typeName:history.state.data.typeName,reconcillationName:history.state.data.reconcillationName,userId:history.state.data.userId,reconciledID:history.state.data.reconciledID,reconciledName:history.state.data.reconciledName} } });
    }
    else if (previousRoute == "account") {
      this._router.navigate(['/app/main/account/create-edit-accounts'], { state: { data: { id : 0, newSubTypeId : selectedId,newSubTypeTitle : title,name:history.state.data.name,number:history.state.data.number, type:history.state.data.type, reconciliation:history.state.data.reconciliation, typeName:history.state.data.typeName,reconcillationName:history.state.data.reconcillationName,userId:history.state.data.userId,reconciledID:history.state.data.reconciledID,reconciledName:history.state.data.reconciledName} } });
    }
    else
    {
    this._router.navigate(['/app/main/sub']);  
    }
  }

}
