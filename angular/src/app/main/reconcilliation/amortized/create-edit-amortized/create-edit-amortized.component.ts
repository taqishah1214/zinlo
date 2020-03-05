import { Component, OnInit, ViewChild, Injector } from '@angular/core';
import { AccountSubTypeServiceProxy, ChartsofAccountServiceProxy, CreateOrEditChartsofAccountDto, CreateOrEditAmortizationDto, AmortizationServiceProxy } from '@shared/service-proxies/service-proxies';
import { Router } from '@angular/router';
import { UserListComponentComponent } from '@app/main/checklist/user-list-component/user-list-component.component';
import { AppComponentBase } from '@shared/common/app-component-base';
import { UppyConfig } from 'uppy-angular';
import { AppConsts } from '@shared/AppConsts';

@Component({
  selector: 'app-create-edit-amortized',
  templateUrl: './create-edit-amortized.component.html',
  styleUrls: ['./create-edit-amortized.component.css']
})
export class CreateEditAmortizedComponent extends AppComponentBase implements OnInit  {

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

  accumulateAmount : boolean = true
  amortizationDto: CreateOrEditAmortizationDto = new CreateOrEditAmortizationDto()

  @ViewChild(UserListComponentComponent, { static: false }) selectedUserId: UserListComponentComponent;
  constructor(private _accountSubTypeService: AccountSubTypeServiceProxy,
    private _chartOfAccountService: ChartsofAccountServiceProxy, private _router: Router, injector: Injector,
    private _reconcialtionService : AmortizationServiceProxy) {
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

  tabFilter(val) : void {
    if (val == "Manual")
    {
      this.accumulateAmount = true
    }
    else if (val == "Monthly")
    {
      this.accumulateAmount = false
    }
    else if (val == "Daily")
    {

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
    // if (this.editAccountCheck) {
    //   this.updateAccount()
    // }
    // else {
    //   this.createAccount()
    // }
    this.createAmortizedItem()
  }

  createAmortizedItem():void {
    debugger
    this._reconcialtionService.createOrEdit(this.amortizationDto).subscribe(response => {
      this.notify.success(this.l('Account Successfully Created.'));
      this.redirectToAmortizedList();
    }) 
  }


  redirectToAmortizedList () : void {
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
