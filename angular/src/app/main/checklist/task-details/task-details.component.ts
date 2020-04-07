import { Component, OnInit, Injector } from '@angular/core';
import { Router } from '@angular/router';
import { UppyConfig } from 'uppy-angular';
import { ClosingChecklistServiceProxy, AuditLogServiceProxy,AttachmentsServiceProxy, PostAttachmentsPathDto, CommentServiceProxy, CreateOrEditCommentDto, DetailsClosingCheckListDto } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import { UserInformation } from '@app/main/CommonFunctions/UserInformation';
import { AppConsts } from '@shared/AppConsts';
import { UserDateService } from "../../../services/user-date.service";


@Component({
  selector: 'app-task-details',
  templateUrl: './task-details.component.html',
  styleUrls: ['./task-details.component.css']
})
export class TaskDetailsComponent extends AppComponentBase implements OnInit {
  taskObject: any;
  taskDetailObject: DetailsClosingCheckListDto = new DetailsClosingCheckListDto();
  recordId: number = 0;
  taskStatus: any = "";
  commantBox: boolean;
  attachments: any;
  commentsData: any = [];
  newAttachmentPaths: any = [];
  comment: CreateOrEditCommentDto = new CreateOrEditCommentDto();
  postAttachment: PostAttachmentsPathDto = new PostAttachmentsPathDto();
  userSignInName: any;
  assigneeId: any = 0;
  UserProfilePicture: any;
  monthStatus : boolean;
  historyOfTask: any = [];
  assigniHistory : any = [];
  statusHistory: any = [];
  users : any = [];
  commentShow = true;
  HistoryList : any =[];


  constructor(
    injector: Injector,
    private _router: Router,
    private _closingChecklistService: ClosingChecklistServiceProxy,
    private _commentServiceProxy: CommentServiceProxy,
    private _attachmentService: AttachmentsServiceProxy,
    private userInfo: UserInformation,
    private _auditLogService : AuditLogServiceProxy,
    private userDate: UserDateService
  ) {
    super(injector);
  }

  ngOnInit() {
    this.userDate.allUsersInformationofTenant.subscribe(userList => this.users = userList)
    this.userSignInName = this.appSession.user.name.toString().toUpperCase();
    this.commantBox = true;
    this.recordId = history.state.data.id;
    this.getTaskDetails(this.recordId);
    this.getProfilePicture();
    this.getAuditLogOfTask();
  }


  getAuditLogOfTask() {
    this._auditLogService.getEntityHistory(this.recordId.toString(), "Zinlo.ClosingChecklist.ClosingChecklist").subscribe(resp => {
      this.historyOfTask = resp
      this.historyOfTask.forEach(element => {
        switch (element.propertyName) {
          case "AssigneeId":         
            element["result"] =  this.setAssigniHistoryParam(element)
            debugger
            break;
          case "Status":          
            element["result"] = this.setStatusHistoryParam(element)
            debugger
            break;
          default:
            console.log("not found");
            break;
        }
        ;
      });
    })
    console.log("r", this.historyOfTask)
  }

  findTheUserFromList(id) : number{
  return this.users.findIndex(x => x.id === id);
  }

 setAssigniHistoryParam(item){
   let array : any = []
  array["ChangeOccurUser"] = this.users[this.findTheUserFromList(item.userId)]; 
  array["NewValue"] = this.users[this.findTheUserFromList(parseInt(item.newValue))]; 
  array["PreviousValue"] = this.users[this.findTheUserFromList(parseInt(item.originalValue))]; 
  return array
 }

 setStatusHistoryParam(item){
  let array : any = []

  array["ChangeOccurUser"] = this.users[this.findTheUserFromList(item.userId)]; 
  array["NewValue"] = this.findStatusName(parseInt(item.newValue)); 
  array["PreviousValue"] = this.findStatusName(parseInt(item.originalValue)); 
  return array
}

  findStatusName(value): string {

    switch (value) {
      case 1:
        return "Not Started"
      case 2:
        return "In Process"
      case 3:
        return "On Hold"
      case 4:
        return "Completed"
      default:
        return ""
    }

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
    this.getTaskDetails(this.recordId);
  }
  onComment(): void {
    this.commantBox = true;
    this.SaveComments();
    this.getTaskDetails(this.recordId);
  }
  SaveComments(): void {
    this.comment.typeId = this.recordId;
    this.comment.type = 1;
    this._commentServiceProxy.create(this.comment).subscribe(result => {
      this.comment.body = "";
      this.notify.success(this.l('Comment is successfully posted'));
    });
  }
  onCancelComment(): void {
    this.commantBox = true;
  }

  RedirectToEditTaskPage(): void {
    this._router.navigate(['/app/main/checklist/edit-task'], { state: { data: { id: this.recordId, categoryid: 0, categoryTitle: "" } } })
  }
  duplicateTask(): void {
    this._router.navigate(['/app/main/checklist/createtask'], { state: { data: {assigneeId : this.taskDetailObject.assigneeId,categoryid: this.taskDetailObject.categoryId, title: this.taskDetailObject.taskName,categoryTitle : this.taskDetailObject.categoryName, createOrDuplicate: false } } });
  }

  BackToTaskList(): void {
    this._router.navigate(['/app/main/checklist']);
  }

  getTaskDetails(id): void {
    this._closingChecklistService.getDetails(id).subscribe(result => {
      this.taskDetailObject = result;
      this.commentsData = this.taskDetailObject.comments;
      this.assigneeId = this.taskDetailObject.assigneeId;
      this.taskStatus = this.taskDetailObject.taskStatus;
      this.monthStatus = this.taskDetailObject.monthStatus;

      this.initilizeStatus();
      this.attachments = result.attachments;
      this.attachments.forEach(element => {
        var attachmentName = element.attachmentPath.substring(element.attachmentPath.lastIndexOf("/") + 1, element.attachmentPath.lastIndexOf("zinlo"));
        element["attachmentExtension"] = this.getExtensionImagePath(element.attachmentPath)
        element["attachmentName"] = attachmentName
        element["attachmentUrl"] = AppConsts.remoteServiceBaseUrl + element.attachmentPath
      });
    })
  }

  initilizeStatus(): void {
    if (this.taskDetailObject.taskStatus === "NotStarted") {
      this.taskDetailObject.taskStatus = "Not Started"
    } else if (this.taskDetailObject.taskStatus === "InProcess") {
      this.taskDetailObject.taskStatus = "In Process"

    }
    else if (this.taskDetailObject.taskStatus === "OnHold") {
      this.taskDetailObject.taskStatus = "On Hold"

    }

  }

  onChangeCommentOrHistory(value){
    if (value == 1)
   {
    this.commentShow = true
   }else {
    this.commentShow = false
   }
  }

  fileUploadedResponse(value): void {
    var response = value.successful
    response.forEach(i => {
      var resp = i.response.body.result
      this.newAttachmentPaths.push(resp.toString());
    });
    this.postAttachment.filePath = this.newAttachmentPaths;
    this.postAttachment.typeId = this.recordId;
    this.postAttachment.type = 1;
    this._attachmentService.postAttachmentsPath(this.postAttachment).subscribe(response => {
      this.notify.success(this.l('Attachment is successfully Uploaded'));
      this.getTaskDetails(this.recordId);
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
              this.getTaskDetails(this.recordId);

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

  deleteTask(): void {
    this.message.confirm(
      '',
      this.l('AreYouSure'),
      (isConfirmed) => {
        if (isConfirmed) {
          this._closingChecklistService.delete(this.recordId)
            .subscribe(() => {
              this.notify.success(this.l('SuccessfullyDeleted'));
              this._router.navigate(['/app/main/checklist']);
            });
        }
      }
    );
  }


}
