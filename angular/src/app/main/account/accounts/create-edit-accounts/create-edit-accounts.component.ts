import { Component, OnInit, ViewChild, Injector } from '@angular/core';
import { AccountSubTypeServiceProxy, ChartsofAccountServiceProxy, CreateOrEditChartsofAccountDto } from '@shared/service-proxies/service-proxies';
import { Router } from '@angular/router';
import { UserListComponentComponent } from '@app/main/checklist/user-list-component/user-list-component.component';
import { AppComponentBase } from '@shared/common/app-component-base';
import { finalize } from 'rxjs/operators';
import * as moment from 'moment';
import { Table } from 'primeng/components/table/table';
import { Paginator } from 'primeng/components/paginator/paginator';
import { LazyLoadEvent } from 'primeng/components/common/lazyloadevent';
import { StoreDateService } from "../../../../services/storedate.service";
import { Input } from '@angular/core';


@Component({
  selector: 'app-create-edit-accounts',
  templateUrl: './create-edit-accounts.component.html',
  styleUrls: ['./create-edit-accounts.component.css']
})
export class CreateEditAccountsComponent extends AppComponentBase implements OnInit  {
  AccountName:string
  AccountNumber:string
  AccountType:string
  ReconcilationTYpe:string
  AccountAssignee:string
  SecondaryAssignee:number
  AccountSubType:string
  saving = false;
  users: any;
  accountSubTypeName: any =  "Select Account Sub Type";
  accountSubTypeList: any;
  accountType: any = "Select Account Type";
  accountId : number;
  selectedUserID : any;
  @Input() primarySelectedUserId:any;
  @Input() secondarySelectedUserId:any;
  @Input() secondId:any;
  secondaryAssignee:any;
  editAccountCheck :  boolean = false;
  accountTypeList: Array<{ id: number, name: string }> = [{ id: 1, name: "Equity" }, { id: 2, name: "Asset" }, { id: 3, name: "Liability" }];
  reconcillationTypeList: Array<{ id: number, name: string }> = [{ id: 1, name: "Itemized" }, { id: 2, name: "Amortized" },{ id: 3, name: "Not Reconcilied" }]
  reconcillationType: any="Select Reconciliation Type";
  assigniId : number;
  accountDto: CreateOrEditChartsofAccountDto = new CreateOrEditChartsofAccountDto()
  reconciledBox : boolean = false
  recociled : any
  recociledList: Array<{ id: number, name: string }> = [{ id: 1, name: "Net Amount" }, { id: 2, name: "Beginning Amount" }, { id: 3, name: "Accrued Amount" }];
  selectedSubTypeId : any;
  seclectedSubTypeTitle : any;
  isAccountExist : boolean = false;
  date : Date = new Date();
  exceptionsList: any;
  linkAccountCheck = false;
  linkAccountNumber
  searchAccount ;
  reconcilliationMessage = false
  reconciliationTypeFilter: number = 0
  showSecondAssignee: Boolean =false
  

  @ViewChild('dataTable', { static: true }) dataTable: Table;
  @ViewChild('paginator', { static: true }) paginator: Paginator;
  @ViewChild(UserListComponentComponent, { static: false }) selectedUserId: UserListComponentComponent;
  constructor(private _accountSubTypeService: AccountSubTypeServiceProxy,
    private _chartOfAccountService: ChartsofAccountServiceProxy, private _router: Router, injector: Injector,
    private userDate: StoreDateService) {
    super(injector)
  }

  ngOnInit() { 
   this.userDate.allUsersInformationofTenant.subscribe(userList => this.users = userList)
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
  enableSecondaryAssignee():void{
    this.showSecondAssignee=true;
     
  }

  createNewAccount() : void {
    if(history.state.data.userId){
    this.selectedUserID=history.state.data.userId;
    }
    else{
      this.selectedUserID=0;
    }
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
    debugger;
    this.editAccountCheck = true;
    this._chartOfAccountService.getAccountForEdit( this.accountId).subscribe(result => {
      if (result.reconciledId == 3 || result.reconciledId == 2){
        if (result.linkedAccount)
        {
          this.linkAccountNumber = result.linkedAccount;
          this.linkAccountCheck =  true;
        }
        else if (result.reconciledId == 2) {
          this.reconcilliationMessage = true
        }
      }
      this.secondId=result.secondaryId;
  
      this.secondarySelectedUserId=result.secondaryId;
      this.accountDto.creatorUserId = result.creatorUserId;
      this.selectedUserId.selectedUserId = result.assigniId; 
      if(history.state.data.userId){
        this.accountDto.assigneeId=history.state.data.userId;
            }
      else if (this.selectedUserId.selectedUserId != undefined)
      {
        this.accountDto.assigneeId = Number(this.selectedUserId.selectedUserId);
      }
      else {
        this.accountDto.assigneeId=0;
      }
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
       this.accountDto.assigneeId = result.assigniId;
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
    this.AccountSubType=null
  }
  changeValidation(){
    if(this.accountDto.accountName != null)
     {
      this.AccountName=null
     }
    if (this.accountDto.accountNumber != null)
     {
      this.AccountNumber=null
     }
    if (this.accountDto.accountType != null)
     {
      this.AccountType=null
     } 
     if (this.accountDto.accountSubTypeId != null)
     {
      this.AccountSubType=null

     }if (this.accountDto.reconciliationType != null)
     {
      this.ReconcilationTYpe=null
     }
      if (this.selectedUserID!=undefined)
     {
      this.AccountAssignee=null;
     }
     if(this.secondaryAssignee!=undefined)
     {
      this.SecondaryAssignee=null;
     }
     

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
    this.ReconcilationTYpe=null
  }
  accountTypeClick(id, name): void {
    
    this.accountType = name;
    this.accountDto.accountType = id;
    this.AccountType=null
  }
  recociledClick(id , name) : void {
    this.recociled = name;
    this.accountDto.reconciled = id;
    if (id == 2){
      this.reconcilliationMessage = true;
    }
    else{
      this.reconcilliationMessage = false;
    }
  }

  routeToAddNewAccountSubType(): void {
    if(this.selectedUserId.selectedUserId==undefined)
    {
      console.log(this.selectedUserId.selectedUserId=0)
    }
    if (this.accountId != 0)
    {
      this._router.navigate(['/app/main/sub/create-or-edit-accountsubtype'],{ state: { data: { accountId: this.accountId , previousRoute : "account", name: this.accountDto.accountName, number:this.accountDto.accountNumber, type:this.accountDto.accountType,reconciliation:this.accountDto.reconciliationType,typeName:this.accountType,reconcillationName:this.reconcillationType, userId:Number(this.selectedUserId.selectedUserId),reconciledID:this.accountDto.reconciled,reconciledName:this.recociled} } });
    }
    else {
      this._router.navigate(['/app/main/sub/create-or-edit-accountsubtype'],{ state: { data: { accountId: 0, previousRoute : "account", name: this.accountDto.accountName, number:this.accountDto.accountNumber, type:this.accountDto.accountType ,reconciliation:this.accountDto.reconciliationType, typeName:this.accountType,reconcillationName:this.reconcillationType, userId:Number(this.selectedUserId.selectedUserId),reconciledID:this.accountDto.reconciled,reconciledName:this.recociled} } });

    }
  }

  linkedAccountClick(accountNo,accountName)
  {

    this.message.confirm(
      '',
      this.l('Are you sure you want to link this account with Account No: ' +accountNo+' Account Name: ' +accountName),
      (isConfirmed) => {
        if (isConfirmed) {
          this.linkAccountCheck =  true;
          this.linkAccountNumber = accountNo
          this.accountDto.linkedAccountNumber = accountNo;
        }
      }
    );


   
  }

  searchWithName() {
    this.searchAccount
    this.getAllImportLog();
  }

  onSubmit(): void {
    debugger;
    if (this.validationCheck())
    {
      if(this.primarySelectedUserId === this.secondarySelectedUserId)
      {
        this.message.error("Assignee and Secondary Assignee cannot be same.");
      }
      else{
      if (this.editAccountCheck) 
      {
        this.updateAccount()
      }
      else
        this.createAccount();
      }
    }
  }

  validationCheck() {
    let found=0
    if(this.accountDto.accountName == null)
     {
      this.AccountName="Account Name is required"
      found=1
     }
    if (this.accountDto.accountNumber == null)
     {
      this.AccountNumber="Account Number is required"
      found=1
     }
    if (this.accountDto.accountType == null)
     {
      this.AccountType="Select the Account Type"
      found=1
     } if (this.accountDto.accountSubTypeId == null)
     {
      this.AccountSubType="Select the Account SubType"
      found=1

     }if (this.accountDto.reconciliationType == null)
     {
      this.ReconcilationTYpe="Select the ReconciliationType"
      found=1
     }
      if (this.selectedUserId.selectedUserId ==undefined)
     {
      this.AccountAssignee="Select an Assignee"
      found=1
     }
     if(found==0){
      return true;
    }
    return false
  }

  CheckAccountNumber() : void {
    this._chartOfAccountService.checkAccountNumber(this.accountDto.accountNumber).subscribe(response => {
      this.isAccountExist = response
      
    })
    this.AccountNumber=null
  }

  createAccount():void {
    debugger;
    this.saving = true;
    if(history.state.data.userId){
      this.accountDto.assigneeId=history.state.data.userId  
      this.accountDto.secondaryId=this.secondarySelectedUserId
    }
    else if (this.selectedUserId.selectedUserId != undefined){
      this.accountDto.assigneeId = Number(this.selectedUserId.selectedUserId);
      if(this.secondarySelectedUserId!= null)
        this.accountDto.secondaryId=Number(this.secondarySelectedUserId);
      else
       this.accountDto.secondaryId=0;
    }
    this._chartOfAccountService.createOrEdit(this.accountDto).subscribe(response => {
      this.notify.success(this.l('Account Successfully Created.'));
      this.redirectToAccountsList();
    }) 
    this.saving = false;
  }

 
  updateAccount() : void {
    this.saving = true;
     this.accountDto.secondaryId = this.secondarySelectedUserId; 
    this._chartOfAccountService.createOrEdit(this.accountDto).pipe(finalize(() => {  }))
    .subscribe(response => {
      this.notify.success(this.l('Account Successfully Updated.'));
      this.saving = false;
      this.redirectToAccountsList();
    })
    
  }

  redirectToAccountsList () : void {
    this._router.navigate(['/app/main/account']);
  }

  getUserIndex(id) {
    return this.users.findIndex(x => x.id === id);
  }

  getNameofAccountTypeAndReconcillation(id, key): string {
    var result = "";
    if (key === "accountType") {
      this.accountTypeList.forEach(i => {
        if (i.id == id) {
          result = i.name
        }
      })
      return result;
    }
    else if (key === "reconcillation") {
      this.reconcillationTypeList.forEach(i => {
        if (i.id == id) {
          result = i.name
        }
      })
      return result;
    }
  }

  getAllImportLog(event?: LazyLoadEvent) {
      if (this.primengTableHelper.shouldResetPaging(event)) {
        this.paginator.changePage(0);
        return;
      }
      this.primengTableHelper.showLoadingIndicator();
      this._chartOfAccountService.getAll(
        this.searchAccount,
        0,
        moment(new Date()),
        0,
        false,
        true,
        this.reconciliationTypeFilter,
        false,
        this.primengTableHelper.getSorting(this.dataTable),
        this.primengTableHelper.getSkipCount(this.paginator, event),
        this.primengTableHelper.getMaxResultCount(this.paginator, event)
      ).subscribe(result => {
        this.primengTableHelper.totalRecordsCount = result.totalCount;
        let data = result.items;
        data.forEach(i => {
          i["assigneeName"] =  this.users[this.getUserIndex(i.assigneeId)].name;
          i["profilePicture"] =  this.users[this.getUserIndex(i.assigneeId)].profilePicture;
          i["accountType"] = this.getNameofAccountTypeAndReconcillation(i.accountTypeId, "accountType")

          // var attachmentName = i.filePath.substring(i.filePath.lastIndexOf("/") + 1, i.filePath.lastIndexOf("zinlo"));
          // i["attachmentName"] = attachmentName
          // i["attachmentUrl"] =  i.filePath
        })
  
        this.primengTableHelper.records = data;
        this.primengTableHelper.hideLoadingIndicator();
        this.exceptionsList = result.items;
        
        this.primengTableHelper.records = this.exceptionsList;
      });

    }
    
    
  }


