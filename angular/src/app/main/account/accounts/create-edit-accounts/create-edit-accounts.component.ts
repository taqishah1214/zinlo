import { Component, OnInit, ViewChild, Injector } from '@angular/core';
import { AccountSubTypeServiceProxy, ChartsofAccountServiceProxy, CreateOrEditChartsofAccountDto } from '@shared/service-proxies/service-proxies';
import { Router } from '@angular/router';
import { UserListComponentComponent } from '@app/main/checklist/user-list-component/user-list-component.component';
import { AppComponentBase } from '@shared/common/app-component-base';
import { finalize } from 'rxjs/operators';
import * as moment from 'moment';


@Component({
  selector: 'app-create-edit-accounts',
  templateUrl: './create-edit-accounts.component.html',
  styleUrls: ['./create-edit-accounts.component.css']
})
export class CreateEditAccountsComponent extends AppComponentBase implements OnInit  {
  saving = false;
  accountSubTypeName: any =  "Select Account Sub Type";
  accountSubTypeList: any;
  accountType: any;
  accountId : number;
  selectedUserID : any;
  editAccountCheck :  boolean = false;
  accountTypeList: Array<{ id: number, name: string }> = [{ id: 1, name: "Equity" }, { id: 2, name: "Assets" }, { id: 3, name: "Liability" }];
  reconcillationTypeList: Array<{ id: number, name: string }> = [{ id: 1, name: "Itemized" }, { id: 2, name: "Amortized" }]
  reconcillationType: any;
  assigniId : number;
  accountDto: CreateOrEditChartsofAccountDto = new CreateOrEditChartsofAccountDto()
  reconciledBox : boolean = false
  recociled : any
  recociledList: Array<{ id: number, name: string }> = [{ id: 1, name: "Net Amount" }, { id: 2, name: "Beginning Amount" }, { id: 3, name: "Accrued Amount" }];
  selectedSubTypeId : any;
  seclectedSubTypeTitle : any;
  isAccountExist : boolean = false;
  date : any;

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
    this.selectedUserID = 0;
    this.selectedSubTypeId = history.state.data.newSubTypeId;
    if (this.selectedSubTypeId != 0)
    {
      this.accountSubTypeName = history.state.data.newSubTypeTitle
      this.accountDto.accountSubTypeId = this.selectedSubTypeId
    }
    
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
       this.accountType = this.getNameofAccountTypeAndReconcillationReconcilled(result.accountType,"accountType")
       this.reconcillationType = this.getNameofAccountTypeAndReconcillationReconcilled(result.reconcillationType,"reconcillation")
       
       if (result.reconciledId != 0)
       {
        this.reconciledBox = true
        this.recociled = this.getNameofAccountTypeAndReconcillationReconcilled(result.reconciledId,"recociledList")
        this.accountDto.reconciled = result.reconciledId
      }    
       this.accountDto.id = result.id;
       this.accountDto.accountType = result.accountType
       this.selectedUserID = result.assigniId;
       this.accountDto.reconciliationType = result.reconcillationType
       this.accountDto.assigneeId = result.assigniId
       this.selectedSubTypeId = history.state.data.newSubTypeId;     
       if (this.selectedSubTypeId != 0)
       {
         this.accountSubTypeName = history.state.data.newSubTypeTitle
         this.accountDto.accountSubTypeId = this.selectedSubTypeId
       }      
       else{
        this.accountSubTypeName =result.accountSubType
        this.accountDto.accountSubTypeId = result.accountSubTypeId;
       }
    })
  }

  getNameofAccountTypeAndReconcillationReconcilled(id , key ) : string {  
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
     else if (key === "recociledList")  
     {
      this.recociledList.forEach(i => {
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
    if (id == 2)
    {
      this.reconciledBox = true
    }
    else{
      this.reconciledBox = false
      this.accountDto.reconciled = 0;
    }
  }
  accountTypeClick(id, name): void {
    this.accountType = name;
    this.accountDto.accountType = id;
  }
  recociledClick(id , name) : void {
    this.recociled = name;
    this.accountDto.reconciled = id;
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
    
    if (this.validationCheck())
    {
      if (this.editAccountCheck) {
        this.updateAccount()
      }
      else {
        this.createAccount()
      }
    }   
  }

  validationCheck() {
    if(this.accountDto.accountName == null)
     {
      this.notify.error("Select the Account Name")
      return false;
     }
     else if (this.accountDto.accountNumber == null)
     {
      this.notify.error("Select the Account Number")
      return false;
     }
     else if (this.accountDto.accountType == null)
     {
      this.notify.error("Select the Account Type")
      return false;
     }
     else if (this.accountDto.accountSubTypeId == null)
     {
      this.notify.error("Select the Account SubType")
      return false;

     }
     else if (this.accountDto.reconciliationType == null)
     {
      this.notify.error("Select the ReconciliationType")
      return false;
     }
     else if (this.isAccountExist)
     {
      this.notify.error("Account Number is already Exist.")
     }
     return true;
  }

  CheckAccountNumber() : void {
    this._chartOfAccountService.checkAccountNoExist(this.accountDto.accountNumber).subscribe(response => {
      this.isAccountExist = response
    })
  }

  createAccount():void {
    this.accountDto.assigneeId = Number(this.selectedUserId.selectedUserId);
    this.accountDto.closingMonth = moment(new Date())
    this.saving = true;
    this._chartOfAccountService.createOrEdit(this.accountDto).pipe(finalize(() => { this.saving = false; }))
    .subscribe(response => {
      this.notify.success(this.l('Account Successfully Created.'));
      this.redirectToAccountsList();
    }) 
  }

 
  updateAccount() : void { 
    this.saving = true;  
    if (this.selectedUserId.selectedUserId != undefined)
    {
      this.accountDto.assigneeId = Number(this.selectedUserId.selectedUserId);
    }
    this._chartOfAccountService.createOrEdit(this.accountDto).pipe(finalize(() => { this.saving = false; }))
    .subscribe(response => {
      this.notify.success(this.l('Account Successfully Updated.'));
      this.redirectToAccountsList();
    })
  }

  redirectToAccountsList () : void {
    this._router.navigate(['/app/main/account/accounts']);
  }


}
