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


  accumulateAmount : boolean = true
  amortizationDto: CreateOrEditAmortizationDto = new CreateOrEditAmortizationDto()
  amortrizedItemId : number ;
  title : string ;
  buttonTitle : string ;
  accountId:number
  attachmentPaths: any = [];
  newAttachementPath: string[] = [];
  accountName : any;
  accountNo : any;


  @ViewChild(UserListComponentComponent, { static: false }) selectedUserId: UserListComponentComponent;
  constructor(private _accountSubTypeService: AccountSubTypeServiceProxy,
    private _chartOfAccountService: ChartsofAccountServiceProxy, private _router: Router, injector: Injector,
    private _reconcialtionService : AmortizationServiceProxy) {
    super(injector)
  }

  ngOnInit() {   
    this.accountId = history.state.data.accountId
    this.accountName = history.state.data.accountName
    this.accountNo = history.state.data.accountNo
    this.amortrizedItemId = history.state.data.amortrizedItemId;
    if ( this.amortrizedItemId != 0)
    {     
      this.getAmortizedItemDetails()
    }
    else
    {         
      this.createNewAccount();
    }   
  }


  getAmortizedItemDetails() : void {
    this.title = "Edit a Item"
    this.buttonTitle =  "Save"
    this._reconcialtionService.getAmortizedItemDetails(this.amortrizedItemId).subscribe(result => {
      this.amortizationDto = result
    })
  }
  
  createNewAccount() : void {
    this.title = "Create a Item"
    this.buttonTitle =  "Create"

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

  onSubmit(): void {
    this.amortizationDto.chartsofAccountId = this.accountId;
    if (this.amortrizedItemId != 0)
    {
      this.updateAmortizedItem()
    }
    else{
      this.createAmortizedItem()
    }
    
  }


  updateAmortizedItem() : void  { 
    this._reconcialtionService.createOrEdit(this.amortizationDto).subscribe(response => {
      this.notify.success(this.l('Amortized Item Successfully Updated.'));
      this.redirectToAmortizedList();
    }) 
  }


  createAmortizedItem():void {

    if (this.attachmentPaths != null) {
      this.newAttachementPath = [];
      this.attachmentPaths.forEach(element => {
        this.newAttachementPath.push(element.toString())
      });

      this.amortizationDto.attachmentsPath = this.newAttachementPath;
    }

    this._reconcialtionService.createOrEdit(this.amortizationDto).subscribe(response => {
      this.notify.success(this.l('Amortized Item  Successfully Created.'));
      this.redirectToAmortizedList();
    }) 
  }

 
  fileUploadedResponse(value): void {
    var response = value.successful
    response.forEach(i => {
      this.attachmentPaths.push(i.response.body.result);
    });
    this.notify.success(this.l('Attachments are SavedSuccessfully Upload'));
  }

  redirectToAmortizedList () : void {
    this._router.navigate(['/app/main/reconcilliation/amortized'],{ state: { data: { accountId :this.accountId , accountName :this.accountName ,accountNo: this.accountNo}} });

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
