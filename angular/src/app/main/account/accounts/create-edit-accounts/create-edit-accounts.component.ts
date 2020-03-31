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
export class CreateEditAccountsComponent extends AppComponentBase implements OnInit  {
  check:boolean=false;
  name:string="Moeen"
  accountSubTypeName: any =  "Select Account Sub Type";
  accountSubTypeList: any;
  accountType: any = "Select Account Type";
  accountId : number;
  selectedUserID : any;
  editAccountCheck :  boolean = false;
  accountTypeList: Array<{ id: number, name: string }> = [{ id: 1, name: "Fixed" }, { id: 2, name: "Assets" }, { id: 3, name: "Liability" }];
  reconcillationTypeList: Array<{ id: number, name: string }> = [{ id: 1, name: "Itemized" }, { id: 2, name: "Amortized" }]
  reconcillationType: any="Select Reconcillation Type";
  assigniId : number;
  accountDto: CreateOrEditChartsofAccountDto = new CreateOrEditChartsofAccountDto()
  reconciledBox : boolean = false
  recociled : any
  recociledList: Array<{ id: number, name: string }> = [{ id: 1, name: "Net Amount" }, { id: 2, name: "Beginning Amount" }, { id: 3, name: "Accrued Amount" }];
  selectedSubTypeId : any;
  seclectedSubTypeTitle : any;
  isAccountExist : boolean = false;

  @ViewChild(UserListComponentComponent, { static: false }) selectedUserId: UserListComponentComponent;
  constructor(private _accountSubTypeService: AccountSubTypeServiceProxy,
    private _chartOfAccountService: ChartsofAccountServiceProxy, private _router: Router, injector: Injector) {
    super(injector)
  }

  ngOnInit() {   
   this.accountId = history.state.data.id;
   this.accountDto.accountName=history.state.data.name
   this.accountDto.accountNumber=history.state.data.number
   this.accountDto.accountType=history.state.data.type
   this.accountDto.reconciliationType=history.state.data.reconciliation
   this.reconcillationType=history.state.data.reconcillationName
   this.accountType = history.state.data.typeName
   this.recociled = history.state.data.reconciledName
   this.accountDto.reconciled = history.state.data.reconciledID;
   this.reconcillationClick(history.state.data.reconciliation, history.state.data.reconcillationName);
   if(!this.accountType){
    this.accountType="Select Account Type";
   }
   if(!this.reconcillationType){
     this.reconcillationType="Select Reconcillation Type";
   }
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
    this.selectedUserID=history.state.data.userId;
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
       if(!history.state.data.name){
        this.accountDto.accountName = result.accountName
       }
       if(!history.state.data.number){
        this.accountDto.accountNumber = result.accountNumber
       }
       if(!history.state.data.typeName){
       this.accountType = this.getNameofAccountTypeAndReconcillationReconcilled(result.accountType,"accountType")
       }
       if(!history.state.data.reconcillationName){
       this.reconcillationType = this.getNameofAccountTypeAndReconcillationReconcilled(result.reconcillationType,"reconcillation")
       }
       if (result.reconciledId != 0)
       {
        this.reconciledBox = true
        
        if(!history.state.data.reconciledName){
          this.recociled = this.getNameofAccountTypeAndReconcillationReconcilled(result.reconciledId,"recociledList")
          this.accountDto.reconciled = result.reconciledId
        }
      }    
       this.accountDto.id = result.id;
       if(!history.state.data.type){
        this.accountDto.accountType = result.accountType
      }
      if(!history.state.data.userId){
       this.selectedUserID = result.assigniId;
       this.accountDto.assigneeId = result.assigniId
      }
      else{
        this.accountDto.assigneeId = history.state.data.userId;       
        this.selectedUserID=history.state.data.userId;
      }
      if(!history.state.data.reconciliation){
       this.accountDto.reconciliationType = result.reconcillationType
      }
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
      this._router.navigate(['/app/main/account/accountsubtype/create-or-edit-accountsubtype'],{ state: { data: { accountId: this.accountId , previousRoute : "account", name: this.accountDto.accountName, number:this.accountDto.accountNumber, type:this.accountDto.accountType,reconciliation:this.accountDto.reconciliationType,typeName:this.accountType,reconcillationName:this.reconcillationType, userId:Number(this.selectedUserId.selectedUserId),reconciledID:this.accountDto.reconciled,reconciledName:this.recociled} } });
    }
    else {
      this._router.navigate(['/app/main/account/accountsubtype/create-or-edit-accountsubtype'],{ state: { data: { accountId: 0, previousRoute : "account", name: this.accountDto.accountName, number:this.accountDto.accountNumber, type:this.accountDto.accountType ,reconciliation:this.accountDto.reconciliationType, typeName:this.accountType,reconcillationName:this.reconcillationType, userId:Number(this.selectedUserId.selectedUserId),reconciledID:this.accountDto.reconciled,reconciledName:this.recociled} } });

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
    if(history.state.data.userId){
      this.accountDto.assigneeId=history.state.data.userId
    }
    else if (this.selectedUserId.selectedUserId != undefined){
      this.accountDto.assigneeId = Number(this.selectedUserId.selectedUserId);
    }
    this._chartOfAccountService.createOrEdit(this.accountDto).subscribe(response => {
      this.notify.success(this.l('Account Successfully Created.'));
      this.redirectToAccountsList();
    }) 
  }

 
  updateAccount() : void {
    if(history.state.data.userId){
      this.accountDto.assigneeId=history.state.data.userId
    }
    else if (this.selectedUserId.selectedUserId != undefined)
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


}
