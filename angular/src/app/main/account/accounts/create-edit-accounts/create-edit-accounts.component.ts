import { Component, OnInit, ViewChild, Injector } from '@angular/core';
import { AccountSubTypeServiceProxy, ChartsofAccountServiceProxy, CreateOrEditChartsofAccountDto } from '@shared/service-proxies/service-proxies';
import { Router } from '@angular/router';
import { UserListComponentComponent } from '@app/main/checklist/user-list-component/user-list-component.component';
import { AppComponentBase } from '@shared/common/app-component-base';

@Component({
  selector: 'app-create-edit-accounts',
  templateUrl: './create-edit-accounts.component.html',
  styleUrls: ['./create-edit-accounts.component.css']
})
export class CreateEditAccountsComponent extends AppComponentBase implements OnInit {

  accountSubTypeName: any;
  accountSubTypeList: any;
  accountType: any;
  accountId : number;
  editAccountCheck :  boolean = false;
  accountTypeList: Array<{ id: number, name: string }> = [{ id: 1, name: "Fixed" }, { id: 2, name: "Assets" }, { id: 3, name: "Liability" }];
  reconcillationTypeList: Array<{ id: number, name: string }> = [{ id: 1, name: "Itemized" }, { id: 2, name: "Amortization" }]
  reconcillationType: any;
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
    this._router.navigate(['/app/main/account/accountsubtype/create-or-edit-accountsubtype']);
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
    this.accountDto.assigneeId = Number(this.selectedUserId.selectedUserId.value);
    this._chartOfAccountService.createOrEdit(this.accountDto).subscribe(response => {
      this.notify.success(this.l('Account Successfully Created.'));
      this.reRouteToAccountsList();
    }) 
  }

  updateAccount() : void {
    this.accountDto.assigneeId = Number(this.selectedUserId.selectedUserId.value);
    this._chartOfAccountService.createOrEdit(this.accountDto).subscribe(response => {
      this.notify.success(this.l('Account Successfully Updated.'));
      this.reRouteToAccountsList();
    })
  }

  reRouteToAccountsList () : void {
    this._router.navigate(['/app/main/account/accounts']);
  }


}
