import { Component, OnInit, Injector } from '@angular/core';
import { Router } from '@angular/router';
import { UppyConfig } from 'uppy-angular';
import { ClosingChecklistServiceProxy,CategoriesServiceProxy, AuditLogServiceProxy,AttachmentsServiceProxy, PostAttachmentsPathDto, CommentServiceProxy, CreateOrEditCommentDto, DetailsClosingCheckListDto } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import { UserInformation } from '@app/main/CommonFunctions/UserInformation';
import { AppConsts } from '@shared/AppConsts';
import { StoreDateService } from "../../../services/storedate.service";


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
  isDeleted:boolean;
  historyOfTask: any = [];
  assigniHistory : any = [];
  statusHistory: any = [];
  users : any = [];
  commentShow = true;
  historyList : any =[];
  AssigniColorBox: any = ["bg-purple", "bg-golden", "bg-sea-green", "bg-gray"," .bg-brown",".bg-blue","bg-magenta"]
  StatusColorBox: any = ["bg-purple", "bg-golden", "bg-sea-green", "bg-gray"]
  buttonColorForComment : any = "bg-grey"
  buttonColorForHistory : any = "bg-lightgrey"
  categoriesList : any = []


  constructor(
    injector: Injector,
    private _router: Router,
    private _closingChecklistService: ClosingChecklistServiceProxy,
    private _commentServiceProxy: CommentServiceProxy,
    private _attachmentService: AttachmentsServiceProxy,
    private userInfo: UserInformation,
    private _auditLogService : AuditLogServiceProxy,
    private storeData: StoreDateService,
    private _categoriesService: CategoriesServiceProxy,
    
  ) {
    super(injector);
  }

  ngOnInit() {
    this.storeData.allUsersInformationofTenant.subscribe(userList => this.users = userList)
    this.storeData.allCategories.subscribe(categoriesList => this.categoriesList = categoriesList)

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
      this.historyOfTask.forEach((element,index) => {
        switch (element.propertyName) {
          case "AssigneeId":         
            element["result"] =  this.setAssigniHistoryParam(element,index)
            break;
            case "Status":          
            element["result"] = this.setStatusHistoryParam(element)
            break;
            case "TaskName":          
            element["result"] = this.setTaskNameHistoryParam(element)
            break;
            case "DueDate":          
            element["result"] = this.setDueDateHistoryParam(element)
            break;
            case "DayBeforeAfter":          
            element["result"] = this.setDaysBeforeAfterHistoryParam(element)
            break;
            case "CategoryId":          
            element["result"] = this.setDaysCategoryIdHistoryParam(element)
            debugger;
            break;
            
          default:
            console.log("not found");
            break;
        }
        ;
      });
    })
  }


  




  findTheUserFromList(id) : number{
  return this.users.findIndex(x => x.id === id);
  }

  setDaysCategoryIdHistoryParam(item){
  let array : any = []
   array["ChangeOccurUser"] = this.users[this.findTheUserFromList(item.userId)]; 
   array["NewValue"] = this.categoriesList[this.getCategoryTitleWithName(parseInt(item.newValue))]; 
   array["PreviousValue"] = this.categoriesList[this.getCategoryTitleWithName(parseInt(item.originalValue))]; 
   array["NewValueColor"] = "bg-lightgrey"; 
   array["PreviousValueColor"] = "bg-lightgrey"; 
   return array
  }

  getRandomNo() {
    let a =  Math.random() * (6 - 0) + 0;
    return ;
  }
  
  getCategoryTitleWithName(id) : number {
  return this.categoriesList.findIndex(x => x.value === id);
  }

  getDaysBeforeAfterNameWith(id) {
    switch (id) {
      case 1:
        return "None"
      case 2:
        return "DaysBefore"
      case 3:
        return "DaysAfter"
      default:
        return ""
    }
  }

  setDaysBeforeAfterHistoryParam(item){
    let array : any = []
   array["ChangeOccurUser"] = this.users[this.findTheUserFromList(item.userId)]; 
   array["NewValue"] = this.getDaysBeforeAfterNameWith(parseInt(item.newValue)); 
   array["PreviousValue"] = this.getDaysBeforeAfterNameWith(parseInt(item.originalValue)); 
   return array
  }
  
  setDueDateHistoryParam(item){
    let array : any = []
   array["ChangeOccurUser"] = this.users[this.findTheUserFromList(item.userId)]; 
   array["NewValue"] = item.newValue; 
   array["PreviousValue"] = item.originalValue; 
   return array
  }

 setAssigniHistoryParam(item,index){
   let array : any = []
  array["ChangeOccurUser"] = this.users[this.findTheUserFromList(item.userId)]; 
  array["NewValue"] = this.users[this.findTheUserFromList(parseInt(item.newValue))];
  array["colorNewValue"] = "bg-magenta"
  array["PreviousValue"] = this.users[this.findTheUserFromList(parseInt(item.originalValue))]; 
  array["colorPreviousValue"] = "bg-purple"
  return array
 }

 setStatusHistoryParam(item){
  let array : any = []
  array["ChangeOccurUser"] = this.users[this.findTheUserFromList(item.userId)]; 
  array["NewValue"] = this.findStatusName(parseInt(item.newValue)); 
  array["NewValueColor"] = this.getStatusColor(parseInt(item.newValue)); 
  array["PreviousValue"] = this.findStatusName(parseInt(item.originalValue)); 
  array["PreviousValueColor"] = this.getStatusColor(parseInt(item.originalValue)); 
  return array
}

setTaskNameHistoryParam(item){
  let array : any = []
  array["ChangeOccurUser"] = this.users[this.findTheUserFromList(item.userId)]; 
  array["NewValue"] = item.newValue; 
  array["PreviousValue"] = item.originalValue; 
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

  getStatusColor (value) {
    if (value === 1) {
       return this.StatusColorBox[3]
    }
    else if (value === 2) {
      return this.StatusColorBox[0]
    }
    else if (value === 3) {
      return this.StatusColorBox[1]
    }
    else if (value === 4) {
      return  this.StatusColorBox[2]
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
      console.log(this.taskDetailObject);
      this.commentsData = this.taskDetailObject.comments;
      this.assigneeId = this.taskDetailObject.assigneeId;
      this.taskStatus = this.taskDetailObject.taskStatus;
      this.monthStatus = this.taskDetailObject.monthStatus;
      this.isDeleted = this.taskDetailObject.isDeleted;
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
    this.buttonColorForHistory = "bg-lightgrey"
    this.buttonColorForComment = "bg-grey"
    this.commentShow = true
   }else {
    this.buttonColorForHistory = "bg-grey"
    this.buttonColorForComment = "bg-lightgrey"
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
  restoreTask():void{
    this._closingChecklistService.restoreTask(this.recordId).subscribe(result=>{
     this.notify.success(this.l('TaskRestoredSuccessfully'));
     this._router.navigate(['/app/main/checklist']);
    })
}

}
