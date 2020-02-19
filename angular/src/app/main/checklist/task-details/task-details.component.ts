import { Component, OnInit, Injector } from '@angular/core';
import { Router } from '@angular/router';
import { UppyConfig } from 'uppy-angular';
import { ClosingChecklistServiceProxy, AttachmentsServiceProxy, PostAttachmentsPathDto, CommentServiceProxy, CreateOrEditCommentDto } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';

@Component({
  selector: 'app-task-details',
  templateUrl: './task-details.component.html',
  styleUrls: ['./task-details.component.css']
})
export class TaskDetailsComponent extends AppComponentBase implements OnInit {
  taskObject: any;
  taskDetailObject: any;
  recordId: number = 0;
  commantBox: boolean;
  attachments: any;
  newAttachmentPaths: any = [];
  comment: CreateOrEditCommentDto = new CreateOrEditCommentDto();
  postAttachment: PostAttachmentsPathDto = new PostAttachmentsPathDto();
  userSignInName: any;

  constructor(
    injector: Injector,
    private _router: Router,
    private _closingChecklistService: ClosingChecklistServiceProxy,
    private _commentServiceProxy: CommentServiceProxy,
    private _attachmentService: AttachmentsServiceProxy
  ) {
    super(injector);
  }

  ngOnInit() {
    this.userSignInName = this.appSession.user.name.toString().toUpperCase();
    this.commantBox = true;

    this.recordId = history.state.data.id;
    this.getTaskDetails(this.recordId);
  }
  commentClick(): void {
    this.commantBox = false;
  }

  onComment(): void {
    this.commantBox = true;
    this.SaveComments();
    this.getTaskDetails(this.recordId);
  }
  SaveComments(): void {
    this.comment.typeId = this.recordId;
    this._commentServiceProxy.create(this.comment).subscribe(result => {
      this.comment.body = "";
      this.notify.success(this.l('Comment is successfully posted'));
    });
  }
  onCancelComment(): void {
    this.commantBox = true;
  }

  RedirectToEditTaskPage(): void {
    this._router.navigate(['/app/main/checklist/edit-task'], { state: { data: { id: this.recordId } } })
  }
  duplicateTask(): void {
    this._router.navigate(['/app/main/checklist/duplicate-task'], { state: { data: { id: this.recordId } } });
  }

  BackToTaskList(): void {
    this._router.navigate(['/app/main/checklist']);
  }

getTaskDetails(id) : void{

   this._closingChecklistService.getDetails(id).subscribe(result=>{
    if(result.comments == null){
      result.comments = [];
    }
   this.taskDetailObject = result;
   
   this.attachments = result.attachments;
   this.attachments.forEach(element => {
    var attachmentName = element.attachmentPath.substring(element.attachmentPath.lastIndexOf("/") + 1, element.attachmentPath.lastIndexOf("#"));
    element["attachmentExtension"] = this.getExtensionImagePath(element.attachmentPath)
    element["attachmentName"] = attachmentName
    element["attachmentUrl"] = "http://localhost:22742/" + element.attachmentPath
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
    extension = extension + ".png";
    return extension;
  }

  settings: UppyConfig = {
    uploadAPI: {
      endpoint: "http://localhost:22742/api/services/app/Attachments/PostAttachmentFile",
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
