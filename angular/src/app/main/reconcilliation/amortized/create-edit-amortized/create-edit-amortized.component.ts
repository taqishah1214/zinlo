import { Component, OnInit, ViewChild, Injector } from '@angular/core';
import { AccountSubTypeServiceProxy, AttachmentsServiceProxy, CreateOrEditAmortizationDto, AmortizationServiceProxy } from '@shared/service-proxies/service-proxies';
import { Router } from '@angular/router';
import { UserListComponentComponent } from '@app/main/checklist/user-list-component/user-list-component.component';
import { AppComponentBase } from '@shared/common/app-component-base';
import { UppyConfig } from 'uppy-angular';
import { AppConsts } from '@shared/AppConsts';
import { finalize } from 'rxjs/operators';


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
  attachments: any;
  saving = false;  

  


  @ViewChild(UserListComponentComponent, { static: false }) selectedUserId: UserListComponentComponent;
  constructor(
    private _attachmentService : AttachmentsServiceProxy, private _router: Router, injector: Injector,
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

  getExtensionImagePath(str) {

    var extension = str.split('.')[1];
    extension = extension + ".svg";
    return extension;
  }

  getAmortizedItemDetails() : void {
    this.title = "Edit a Item"
    this.buttonTitle =  "Save"
    this._reconcialtionService.getAmortizedItemDetails(this.amortrizedItemId).subscribe(result => {
      this.amortizationDto = result
      this.attachments = result.attachments;
      this.attachments.forEach(element => {
        var attachmentName = element.attachmentPath.substring(element.attachmentPath.lastIndexOf("/") + 1, element.attachmentPath.lastIndexOf("zinlo"));
        element["attachmentExtension"] = this.getExtensionImagePath(element.attachmentPath)
        element["attachmentName"] = attachmentName
        element["attachmentUrl"] = AppConsts.remoteServiceBaseUrl+"/" + element.attachmentPath
      });
    })
  }
  
  deleteAttachment(id): void {
    let self = this;
        self.message.confirm(
            "",
            this.l('AreYouSure'),
            isConfirmed => {
                if (isConfirmed) {
                  this._attachmentService.deleteAttachmentPath(id)
                  .subscribe(() => {
                    this.notify.success(this.l('Attachment is successfully removed'));
                    this.getAmortizedItemDetails();
                  });
                }
            }
        );
  }

  createNewAccount() : void {
    this.amortizationDto.criteria = 1;
    this.title = "Create a Item"
    this.buttonTitle =  "Create"

  }


  tabFilter(val) : void {
    if (val == "Manual")
    {
      this.accumulateAmount = true
      this.amortizationDto.criteria = 1;

    }
    else if (val == "Monthly")
    {
      this.amortizationDto.criteria = 2;
      this.accumulateAmount = false
      this.amortizationDto.accomulateAmount = 0;

    }
    else if (val == "Daily")
    {
      this.amortizationDto.criteria = 3;
      this.accumulateAmount = false
      this.amortizationDto.accomulateAmount = 0;
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
    this.saving = true;  
    if (this.attachmentPaths != null) {
      this.newAttachementPath = [];
      this.attachmentPaths.forEach(element => {
        this.newAttachementPath.push(element.toString())
      });

      this.amortizationDto.attachmentsPath = this.newAttachementPath;
    }
    this._reconcialtionService.createOrEdit(this.amortizationDto).pipe(finalize(() => { this.saving = false; })).subscribe(response => {
      this.notify.success(this.l('Amortized Item Successfully Updated.'));
      this.redirectToAmortizedList();
    }) 
  }


  createAmortizedItem():void {
    this.saving = true;  
    if (this.attachmentPaths != null) {
      this.newAttachementPath = [];
      this.attachmentPaths.forEach(element => {
        this.newAttachementPath.push(element.toString())
      });

      this.amortizationDto.attachmentsPath = this.newAttachementPath;
    }

    this._reconcialtionService.createOrEdit(this.amortizationDto).pipe(finalize(() => { this.saving = false; })).subscribe(response => {
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
