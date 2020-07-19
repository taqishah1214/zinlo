import { Component, OnInit, ViewChild, Injector } from '@angular/core';
import { ChartsofAccountServiceProxy, CreateOrEditItemizationDto, ItemizationServiceProxy, GetEditionEditOutput, AttachmentsServiceProxy } from '@shared/service-proxies/service-proxies';
import { Router } from '@angular/router';
import { UserListComponentComponent } from '@app/main/checklist/user-list-component/user-list-component.component';
import { AppComponentBase } from '@shared/common/app-component-base';
import { UppyConfig } from 'uppy-angular';
import { AppConsts } from '@shared/AppConsts';
import { finalize } from 'rxjs/operators';
import * as moment from 'moment';
import {UserInformation} from "../../../CommonFunctions/UserInformation"



@Component({
  selector: 'app-create-edit-itemized',
  templateUrl: './create-edit-itemized.component.html',
  styleUrls: ['./create-edit-itemized.component.css']
  
})
export class CreateEditItemizedComponent extends AppComponentBase implements OnInit  {
  itemizedDto : CreateOrEditItemizationDto = new CreateOrEditItemizationDto();
  title = "Add an Item";
  Save  = "Save";
  accountName : any;
  accountNo : any;
  accountId : any;
  ItemizedItemId : any;
  UserProfilePicture: any;
  attachmentPaths: any = [];
  recordId: number;
  newAttachementPath: string[] = [];
  attachments: any;
  saving = false;
  userName : string;
  commantModal : Boolean;
  commantBox : Boolean;
  updateLock : Boolean = true; 
  commentFiles:File[]=[];
  selectedDate : Date  = new Date;
  monthStatus:boolean
  constructor(private _itemizationServiceProxy : ItemizationServiceProxy,
    private _attachmentService : AttachmentsServiceProxy, private _router: Router, injector: Injector, private userInfo: UserInformation) {
    super(injector)
  }
  uploadCommentFile($event) {
    this.commentFiles.push($event.target.files[0]);
  }
  removeCommentFile(index)
  {
    this.commentFiles.splice(index, 1);
  }
  ngOnInit() {  
    if (history.state.navigationId == 1){
      this._router.navigate(['/app/main/reconciliation']);
    }
    this.accountId = history.state.data.accountId
    this.userName = this.appSession.user.name.toString();
    this.commantBox = true;
    this.ItemizedItemId = history.state.data.ItemizedItemId;
    this.accountName = history.state.data.accountName
    this.accountNo = history.state.data.accountNo
    this.monthStatus = history.state.data.monthStatus
    this.getProfilePicture();
    if(this.ItemizedItemId != 0)
    {
      this.updateLock = false;
      this.title = "Edit an Item";
      this.Save = "Update";
      this.getDetailsofItem(this.ItemizedItemId);
    }
    else {
      this.itemizedDto.closingMonth = moment(new Date())
    }
  
  }
  RedirectToDetail(ItemizedItemId) : void {   
    this._router.navigate(['/app/main/reconciliation/itemized/itemized-details'],{ state: { data: { monthStatus : this.monthStatus ,accountId : this.accountId ,accountName :this.accountName ,accountNo: this.accountNo,ItemizedItemId : ItemizedItemId , selectedDate :history.state.data.selectedDate,amount : history.state.data.amount}} });
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
    this.itemizedDto.commentBody = "";
    this.commantModal = false;
    this.commantBox = true;
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
  validationCheck() {
    if(this.itemizedDto.date == null)
     {
      this.notify.error("Select Date")
      return false;
     }
     else if(this.itemizedDto.description == null)
     {
      this.notify.error("Select the Description")
      return false;
     }
     else if(this.itemizedDto.amount == null)
     {
      this.notify.error("Select the Amount")
      return false;
     }
     return true;
    }
  redirectToItemsList () : void {  
      this._router.navigate(['/app/main/reconciliation/itemized'],{ state: { data: { accountId :this.accountId , accountName :this.accountName ,accountNo: this.accountNo ,selectedDate :history.state.data.selectedDate}} }); 
  }
  settings: UppyConfig = {
    uploadAPI: {
      endpoint:  AppConsts.remoteServiceBaseUrl + '/api/services/app/Attachments/PostAttachmentFile',
    },
    plugins: {
      Webcam: false
    }
  }
  fileUploadedResponse(value): void {
    var response = value.successful
    response.forEach(i => {
      this.attachmentPaths.push(i.response.body.result);
    });
    this.notify.success(this.l('Attachments are SavedSuccessfully Upload'));
  }

  onOpenCalendar(container) {
    container.monthSelectHandler = (event: any): void => {
      container._store.dispatch(container._actions.select(event.date));
    };
    container.setViewMode('month');
  }
  SaveItem(){
    if(this.validationCheck() ){
    this.saving = true;  
    if (this.attachmentPaths != null) {
      this.newAttachementPath = [];
      this.attachmentPaths.forEach(element => {
      this.newAttachementPath.push(element.toString())
  });

  this.itemizedDto.attachmentsPath = this.newAttachementPath;
}
    this.itemizedDto.chartsofAccountId = this.accountId;
      this.itemizedDto.creationTime =history.state.data.selectedDate
    this._itemizationServiceProxy.createOrEdit(this.itemizedDto).pipe(finalize(() => { this.saving = false; })).subscribe(response => {
      this.notify.success(this.l('Item Successfully Created.'));
      this.redirectToItemsList();
    })
  
  }
}
  getExtensionImagePath(str) {

    var extension = str.split('.')[1];
    extension = extension + ".svg";
    return extension;
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
                    this.getDetailsofItem(this.ItemizedItemId);
                  });
                }
            }
        );
  }
  getDetailsofItem(id):void{
      this._itemizationServiceProxy.getEdit(id).subscribe(result=>{
      this.itemizedDto = result;
      this.itemizedDto.amount = history.state.data.amount;
      this.attachments = result.attachments;
      this.attachments.forEach(element => {
        var attachmentName = element.attachmentPath.substring(element.attachmentPath.lastIndexOf("/") + 1, element.attachmentPath.lastIndexOf("zinlo"));
        element["attachmentExtension"] = this.getExtensionImagePath(element.attachmentPath)
        element["attachmentName"] = attachmentName
        element["attachmentUrl"] = AppConsts.remoteServiceBaseUrl+"/" + element.attachmentPath
      });

      });
    
  }

}


