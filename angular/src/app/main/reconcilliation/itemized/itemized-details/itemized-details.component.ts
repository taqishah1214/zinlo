import { Component, OnInit, Injector } from '@angular/core';
import { Router } from '@angular/router';
import { UppyConfig } from 'uppy-angular';
import {  AttachmentsServiceProxy, PostAttachmentsPathDto, CreateOrEditCommentDto, DetailsClosingCheckListDto, ItemizationServiceProxy, CreateOrEditAmortizationDto, CreateOrEditItemizationDto } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import { UserInformation } from '@app/main/CommonFunctions/UserInformation';
import { AppConsts } from '@shared/AppConsts';
@Component({
  selector: 'app-itemized-details',
  templateUrl: './itemized-details.component.html',
  styleUrls: ['./itemized-details.component.css']
})
export class ItemizedDetailsComponent extends AppComponentBase implements OnInit {
  taskObject: any;
  taskDetailObject: DetailsClosingCheckListDto = new DetailsClosingCheckListDto();
  recordId: number = 0;
  taskStatus: any = "";
  commantBox: boolean = true;
  attachments: any;
  commentsData: any = [];
  newAttachmentPaths: any = [];
  comment: CreateOrEditCommentDto = new CreateOrEditCommentDto();
  postAttachment: PostAttachmentsPathDto = new PostAttachmentsPathDto();
  userSignInName: any;
  assigneeId: any = 0;
  UserProfilePicture: any;
  accountId : any;
  accountName:any;
  accountNo:any;
  itemizedItemId:any;
  itemizationDto: CreateOrEditItemizationDto = new CreateOrEditItemizationDto()
  netAmount : any;
  accuredAmount : any;

  constructor(
    injector: Injector,
    private _router: Router,
    private _itemizationService :ItemizationServiceProxy ,
    private _attachmentService: AttachmentsServiceProxy,
    private userInfo: UserInformation
  ) {
    super(injector);
  }

  ngOnInit() {
    this.userSignInName = this.appSession.user.name.toString().toUpperCase();
    this.accountId = history.state.data.accountId
    this.accountName = history.state.data.accountName
    this.accountNo = history.state.data.accountNo
    this.itemizedItemId = history.state.data.ItemizedItemId
    this.getTaskDetails();
    this.getProfilePicture();
  }

  getProfilePicture() {
    this.userInfo.getProfilePicture();
    this.userInfo.profilePicture.subscribe(
      data => {
        this.UserProfilePicture = data.valueOf();
      });
    if (this.UserProfilePicture == undefined) {
      this.UserProfilePicture = "";
    }
  }
  commentClick(): void {
    this.commantBox = false;
  }
  updateDetails(): void {
    this.getTaskDetails();
  }
  onComment(): void {
    this.commantBox = true;
    this.SaveComments();
    this.getTaskDetails();
  }
  SaveComments(): void {
   
  }
  onCancelComment(): void {
    this.commantBox = true;
  }

  editItemizedItem() : void {
    this._router.navigate(['/app/main/reconcilliation/itemized/create-edit-itemized'],{ state: { data: { accountId : this.accountId ,accountName :this.accountName ,accountNo: this.accountNo,ItemizedItemId : this.itemizedItemId}} });
  }
  BackToList() : void {
    this._router.navigate(['/app/main/reconcilliation/itemized'],{ state: { data: { accountId : this.accountId ,accountName :this.accountName ,accountNo: this.accountNo}} });
  }
  

  getTaskDetails(): void {
    
    this._itemizationService.getEdit(this.itemizedItemId).subscribe(result => {
      this.itemizationDto = result
      this.attachments = result.attachments;
      this.attachments.forEach(element => {
        var attachmentName = element.attachmentPath.substring(element.attachmentPath.lastIndexOf("/") + 1, element.attachmentPath.lastIndexOf("zinlo"));
        element["attachmentExtension"] = this.getExtensionImagePath(element.attachmentPath)
        element["attachmentName"] = attachmentName
        element["attachmentUrl"] = AppConsts.remoteServiceBaseUrl+"/" + element.attachmentPath
      });
    })

  }

 

  fileUploadedResponse(value): void {
    var response = value.successful
    response.forEach(i => {
      var resp = i.response.body.result
      this.newAttachmentPaths.push(resp.toString());
    });
    this.postAttachment.filePath = this.newAttachmentPaths;
    this.postAttachment.typeId = this.itemizedItemId;
    this.postAttachment.type = 3;
    this._attachmentService.postAttachmentsPath(this.postAttachment).subscribe(response => {
      this.notify.success(this.l('Attachment is successfully Uploaded'));
      this.getTaskDetails();
    })
  }

  deleteAttachment(id): void {
    this.message.confirm(
      '',
      this.l('AreYouSure'),
      (isConfirmed) => {
        if (isConfirmed) {
          this._attachmentService.deleteAttachmentPath(id)
            .subscribe(() => {
              this.notify.success(this.l('Attachment is successfully removed'));
              this.getTaskDetails();

            });
        }
      }
    );

  }
  getExtensionImagePath(str) {

    var extension = str.split('.')[1];
    extension = extension + ".svg";
    return extension;
  }
  settings: UppyConfig = {
    uploadAPI: {
      endpoint: AppConsts.remoteServiceBaseUrl + '/api/services/app/Attachments/PostAttachmentFile',
    },
    plugins: {
      Webcam: false
    }
  }

  deleteItem(): void {
    this.message.confirm(
      '',
      this.l('AreYouSure'),
      (isConfirmed) => {
        if (isConfirmed) {
          this._itemizationService.delete(this.itemizedItemId).subscribe(() => {
            this.notify.success(this.l('Amortized Item Successfuly Deleted'));
            this.BackToList()
          });
        }
      }
    );
  }


}
