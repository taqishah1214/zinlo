
import { Component, OnInit, ViewChild, Injector } from '@angular/core';
import { AccountSubTypeServiceProxy, ChartsofAccountServiceProxy, CreateOrEditChartsofAccountDto } from '@shared/service-proxies/service-proxies';
import { Router } from '@angular/router';
import { UserListComponentComponent } from '@app/main/checklist/user-list-component/user-list-component.component';
import { AppComponentBase } from '@shared/common/app-component-base';
import { UppyConfig } from 'uppy-angular';
import { AppConsts } from '@shared/AppConsts';

@Component({
  selector: 'app-createdit-amartization',
  templateUrl: './createdit-amartization.component.html',
  styleUrls: ['./createdit-amartization.component.css']
})
export class CreateditAmartizationComponent extends AppComponentBase implements OnInit  {

  accountSubTypeName: any;
  accountSubTypeList: any;
  accountType: any;
  accountId : number;
  selectedUserID : any;
  editAccountCheck :  boolean = false;
  accountTypeList: Array<{ id: number, name: string }> = [{ id: 1, name: "Fixed" }, { id: 2, name: "Assets" }, { id: 3, name: "Liability" }];
  reconcillationTypeList: Array<{ id: number, name: string }> = [{ id: 1, name: "Itemized" }, { id: 2, name: "Amortization" }]
  reconcillationType: any;
  assigniId : number;
  accountDto: CreateOrEditChartsofAccountDto = new CreateOrEditChartsofAccountDto()

  @ViewChild(UserListComponentComponent, { static: false }) selectedUserId: UserListComponentComponent;
  constructor(private _accountSubTypeService: AccountSubTypeServiceProxy,
    private _chartOfAccountService: ChartsofAccountServiceProxy, private _router: Router, injector: Injector) {
    super(injector)
  }

  ngOnInit() {   
   this.accountId = history.state.data.id;
    this.getAccountSubTypeForDropDown();
    if ( this.accountId != 0)
    {
      
      this.editAccount()
    }
    else
    {    
      this.createNewAccount();
    }   
  }

  createNewAccount() : void {
    this.reconcillationType = "Select Reconcillation Type"
    this.accountType = "Select Account Type"
    this.accountSubTypeName = "Select Account Sub Type"
    this.selectedUserID = 0;
  }


  getAccountSubTypeForDropDown(): void {
    this._accountSubTypeService.accountSubTypeDropDown().subscribe(result => {
      this.accountSubTypeList = result;
    });
  }

  editAccount() : void {
    this.editAccountCheck = true;
    this._chartOfAccountService.getAccountForEdit( this.accountId).subscribe(result => {
       this.accountDto.accountName = result.accountName
       this.accountDto.accountNumber = result.accountNumber
       this.accountType = this.getNameofAccountTypeAndReconcillation(result.accountType,"accountType")
       this.reconcillationType = this.getNameofAccountTypeAndReconcillation(result.reconcillationType,"reconcillation")
       this.accountSubTypeName =result.accountSubType
       this.accountDto.accountSubTypeId = result.accountSubTypeId;
       this.accountDto.id = result.id;
       this.accountDto.accountType = result.accountType
       this.selectedUserID = result.assigniId;
       this.accountDto.reconciliationType = result.reconcillationType
       this.accountDto.assigneeId = result.assigniId
    })
  }

  getNameofAccountTypeAndReconcillation(id , key ) : string {  
    var result = "" ;
    if (key === "accountType")
     {
      this.accountTypeList.forEach(i => {
        if  (i.id == id)
        {
          result = i.name
        }
      })
      return result;
     }
     else if (key === "reconcillation")
     {
      this.reconcillationTypeList.forEach(i => {
        if  (i.id == id)
        {
          result = i.name
        }
      })
      return result;
     }  
  }

  accountSubTypeClick(id, name): void {
    this.accountSubTypeName = name;
    this.accountDto.accountSubTypeId = id;
  }

  reconcillationClick(id, name): void {
    this.reconcillationType = name;
    this.accountDto.reconciliationType = id;
  }
  accountTypeClick(id, name): void {
    this.accountType = name;
    this.accountDto.accountType = id;
  }

  routeToAddNewAccountSubType(): void {
    if (this.accountId != 0)
    {
      this._router.navigate(['/app/main/account/accountsubtype/create-or-edit-accountsubtype'],{ state: { data: { accountId: this.accountId , previousRoute : "account"} } });
    }
    else {
      this._router.navigate(['/app/main/account/accountsubtype/create-or-edit-accountsubtype'],{ state: { data: { accountId: 0, previousRoute : "account" } } });

    }
  }

  onSubmit(): void {
    if (this.editAccountCheck) {
      this.updateAccount()
    }
    else {
      this.createAccount()
    }
  }

  createAccount():void {
    this.accountDto.assigneeId = Number(this.selectedUserId.selectedUserId);
    this._chartOfAccountService.createOrEdit(this.accountDto).subscribe(response => {
      this.notify.success(this.l('Account Successfully Created.'));
      this.redirectToAccountsList();
    }) 
  }

 
  updateAccount() : void {   
    if (this.selectedUserId.selectedUserId != undefined)
    {
      this.accountDto.assigneeId = Number(this.selectedUserId.selectedUserId);
    }
    this._chartOfAccountService.createOrEdit(this.accountDto).subscribe(response => {
      this.notify.success(this.l('Account Successfully Updated.'));
      this.redirectToAccountsList();
    })
  }

  redirectToAccountsList () : void {
    this._router.navigate(['/app/main/account/accounts']);
  }
  settings: UppyConfig = {
    uploadAPI: {
      endpoint:  AppConsts.remoteServiceBaseUrl + '/api/services/app/Attachments/PostAttachmentFile',
    },
    plugins: {
      Webcam: false
    }
  }

  onOpenCalendar(container) {
    container.monthSelectHandler = (event: any): void => {
      container._store.dispatch(container._actions.select(event.date));
    };
    container.setViewMode('month');
  }

}
