import { Component, OnInit, ViewChild, Injector } from '@angular/core';
import { AccountSubTypeServiceProxy, AttachmentsServiceProxy, CreateOrEditAmortizationDto, AmortizationServiceProxy } from '@shared/service-proxies/service-proxies';
import { Router } from '@angular/router';
import { UserListComponentComponent } from '@app/main/checklist/user-list-component/user-list-component.component';
import { AppComponentBase } from '@shared/common/app-component-base';
import { UppyConfig } from 'uppy-angular';
import { AppConsts } from '@shared/AppConsts';
import { finalize } from 'rxjs/operators';
import * as moment from 'moment';
import {UserInformation} from "../../../CommonFunctions/UserInformation"




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
  commantModal : Boolean;
  commantBox : Boolean;
  userName :any;
  UserProfilePicture: any;
  updateLock : Boolean = true; 
  CriteriaButton :any = 0;
  commentFiles:File[]=[];
  monthStatus:boolean;
  netAmount : any;
  accuredAmount : any;
  selectedDate = new Date;
  @ViewChild(UserListComponentComponent, { static: false }) selectedUserId: UserListComponentComponent;
  constructor(
    private _attachmentService : AttachmentsServiceProxy, private _router: Router, injector: Injector,
    private _reconcialtionService : AmortizationServiceProxy,private userInfo: UserInformation) {
    super(injector)
  }
  ngOnInit() {   
    if (history.state.navigationId == 1){
      this._router.navigate(['/app/main/reconciliation']);
    }
    this.selectedDate = history.state.data.selectedDate
    this.accountId = history.state.data.accountId
    this.accountName = history.state.data.accountName
    this.userName = this.appSession.user.name.toString();
    this.accountNo = history.state.data.accountNo
    this.netAmount =history.state.data.netAmount
    this.accuredAmount =history.state.data.accuredAmount
    this.monthStatus = history.state.data.monthStatus
    this.commantBox = true;
    this.getProfilePicture();
    this.amortrizedItemId = history.state.data.amortrizedItemId;
    if ( this.amortrizedItemId != 0)
    {     
      this.getAmortizedItemDetails()
      this.updateLock = false;

    }
    else
    {         
      this.createNewAccount();
    }   
  }
  RedirectToDetails(amortizedItemId,accured,net) : void {
    this._router.navigate(['/app/main/reconciliation/amortized/amortized-details'],{ state: { data: { monthStatus : this.monthStatus , accountId : this.accountId ,accountName :this.accountName ,accountNo: this.accountNo,amortrizedItemId : amortizedItemId,accuredAmount: accured,netAmount:net,selectedDate : this.selectedDate ,linkedAccountNo: history.state.data.linkedAccountNo , linkedAccountName : history.state.data.linkedAccountName }} });
  }
  uploadCommentFile($event) {
    this.commentFiles.push($event.target.files[0]);
  }
  removeCommentFile(index)
  {
    this.commentFiles.splice(index, 1);
  }
  getExtensionImagePath(str) {

    var extension = str.split('.')[1];
    extension = extension + ".svg";
    return extension;
  }

  commentClick(): void {
    this.commantModal = true;
    this.commantBox = false;
  }
  onComment(): void {
    this.commantModal = false;
    this.commantBox = true;
  }
  onCancelComment(): void {
    this.amortizationDto.commentBody = "";
    this.commantModal = false;
    this.commantBox = true;
  }
  
  getAmortizedItemDetails() : void {
    this.title = "Edit an Item"
    this.buttonTitle =  "Save"
    this._reconcialtionService.getAmortizedItemDetails(this.amortrizedItemId).subscribe(result => {
      this.amortizationDto = result
      debugger;
      this.CriteriaButton = this.amortizationDto.criteria
      this.attachments = result.attachments;
      this.attachments.forEach(element => {
        var attachmentName = element.attachmentPath.substring(element.attachmentPath.lastIndexOf("/") + 1, element.attachmentPath.lastIndexOf("zinlo"));
        element["attachmentExtension"] = this.getExtensionImagePath(element.attachmentPath)
        element["attachmentName"] = attachmentName
        element["attachmentUrl"] = AppConsts.remoteServiceBaseUrl+"/" + element.attachmentPath
      });
    })
  }
  getProfilePicture() {
    this.userInfo.getProfilePicture();
    this.userInfo.profilePicture.subscribe(
      data => {
        this.UserProfilePicture = data.valueOf();
     });
    if (this.UserProfilePicture == undefined)
    {
      this.UserProfilePicture = "";
    }
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
    this.amortizationDto.closingMonth = moment(new Date())
    this.amortizationDto.criteria = 1;
    this.title = "Create an Item"
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
    if(this.validationCheck()){
      if (this.amortrizedItemId != 0)
      {
        this.updateAmortizedItem()
      }
      else{
        this.createAmortizedItem()
      }
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
    this.amortizationDto.creationTime = moment(this.selectedDate)
    this._reconcialtionService.createOrEdit(this.amortizationDto).pipe(finalize(() => { this.saving = false; })).subscribe(response => {
      this.notify.success(this.l('Amortized Item  Successfully Created.'));
      this.redirectToAmortizedList();
    }) 
  }
  changeValidation(){
    if(this.amortizationDto.startDate != null)
     {
      this.StartDate=null
     }
     if(this.amortizationDto.endDate != null)
     {
      this.EndDate=null
    }
     if(this.amortizationDto.amount != null)
     {
      this.Amount=null
     }
     if(this.accumulateAmount){
      if(this.amortizationDto.accomulateAmount != null)
      {
        this.AccAmount=null
      }
     }
     if(this.amortizationDto.inoviceNo != null)
     {
      this.InvoiceNumber=null
     }
     if(this.amortizationDto.journalEntryNo != null)
     {
      this.EntryNo=null
     }
  }
  StartDate:string
  EndDate:string
  Amount:string
  AccAmount:string
  InvoiceNumber:string
  EntryNo:string
  validationCheck() {
    let found=0
     if(this.amortizationDto.startDate == null)
     {
      this.StartDate="Select the Start Date"
      found=1
     }
     if(this.amortizationDto.endDate == null)
     {
      this.EndDate="Select the End Date"
      found=1
     }
     if(this.amortizationDto.amount == null)
     {
      this.Amount="Select the Amount"
      found=1
     }
     if(this.accumulateAmount){
      if(this.amortizationDto.accomulateAmount == null)
      {
        this.AccAmount="Select the Accomulate Amount"
        found=1
      }
    }
     if(this.amortizationDto.inoviceNo == null)
     {
      this.InvoiceNumber="Invoice No is required"
      found=1
     }
     if(this.amortizationDto.journalEntryNo == null)
     {
      this.EntryNo="Entry No is required"
      found=1
     }
     if(found==0){
      return true;
      }
      return false;
    }
  fileUploadedResponse(value): void {
    var response = value.successful
    response.forEach(i => {
      this.attachmentPaths.push(i.response.body.result);
    });
    this.notify.success(this.l('Attachments are SavedSuccessfully Upload'));
  }

  redirectToAmortizedList () : void {
        this._router.navigate(['/app/main/reconciliation/amortized'],{ state: { data: { accountId :this.accountId , accountName :this.accountName ,accountNo: this.accountNo, selectedDate : this.selectedDate,linkedAccountNo: history.state.data.linkedAccountNo , linkedAccountName : history.state.data.linkedAccountName}} });
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
