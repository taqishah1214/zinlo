import { Component, OnInit, ViewChild, Injector } from '@angular/core';
import { Router } from '@angular/router';
import { CreateOrEditClosingChecklistDto, ClosingChecklistServiceProxy, TimeManagementsServiceProxy } from '@shared/service-proxies/service-proxies';
import { CategorieDropDownComponent } from '@app/main/categories/categorie-drop-down/categorie-drop-down.component';
import { UserListComponentComponent } from '../user-list-component/user-list-component.component';
import { IgxMonthPickerComponent } from "igniteui-angular";
import { UppyConfig } from 'uppy-angular';
import { AppComponentBase } from '@shared/common/app-component-base';
import { AppConsts } from '@shared/AppConsts';
import {UserInformation} from "../../CommonFunctions/UserInformation"
import { finalize } from 'rxjs/operators';
import { add, subtract } from 'add-subtract-date';
import * as moment from 'moment';
import { HttpRequest, HttpClient } from '@angular/common/http';
import { ToolbarService, LinkService, ImageService, HtmlEditorService, QuickToolbarService } from '@syncfusion/ej2-angular-richtexteditor';
import { AngularEditorConfig } from '@kolkov/angular-editor';
import { IfStmt } from '@angular/compiler';

@Component({
  selector: 'app-createtask',
  templateUrl: './createtask.component.html',
  styleUrls: ['./createtask.component.css'], 
    providers: [ToolbarService, LinkService, ImageService, HtmlEditorService, QuickToolbarService]
})
export class CreatetaskComponent extends AppComponentBase implements OnInit {

  config: AngularEditorConfig = {
    editable: true,
    spellcheck: true,
    height: '224px',
    placeholder: 'Enter text here...',
    translate: 'no',
    defaultParagraphSeparator: 'p',
    defaultFontName: '',
    fonts: [
      {class: 'arial', name: 'Arial'},
      {class: 'times-new-roman', name: 'Times New Roman'},
      {class: 'calibri', name: 'Calibri'},
      {class: 'comic-sans-ms', name: 'Comic Sans MS'}
    ],
    toolbarHiddenButtons: [
      ['undo',
      'redo','strikeThrough',
      'subscript',
      'superscript', 'indent',
      'outdent','insertVideo',
      'insertHorizontalRule',
      'removeFormat',
      'toggleEditorMode',
      'link',
      'unlink','fontSize','fontName']
      ]
  };

  commentFiles:File[]=[];
  instructionFiles:File[]=[];
  saving = false;
  categories: any;
  Email: string;
  taskName: string;
  comment: string;
  closingMonth: string;
  frequenct: string;
  commantModal: boolean;
  commantBox: boolean;
  closingMonthInputBox: boolean;
  closingMonthModalBox: boolean;
  UserProfilePicture: any;
  enableValue: boolean = false;
  SelectionMsg: string = "\xa0";
  userName : string;
  attachmentPaths: any = [];
  endOnIsEnabled: boolean = true; 
  newAttachementPath: string[] = [];
  public isChecked: boolean = false;
  commentFilesPath:any[]=[];
  assigneeSelected=false
  dueOnSelected=false
  days: any;
  users: any;
  checklist: CreateOrEditClosingChecklistDto;
  minDate: Date = new Date()
  categoryTitle : any;
  active = false;
  dayBeforeAfter = 1;
  endOfMonth: string;
  createOrDuplicate : boolean = true;
  @ViewChild(CategorieDropDownComponent, { static: false }) selectedCategoryId: CategorieDropDownComponent;
  @ViewChild(UserListComponentComponent, { static: false }) selectedUserId: UserListComponentComponent;
  @ViewChild(IgxMonthPickerComponent, { static: true }) monthPicker: IgxMonthPickerComponent;
  daysBeforeAfter : any = 1;
  DaysByMonth: Date = new Date();
  errorMessage = "";
  monthStatus = true;
  CreateOrEdit = false;
  constructor
    (private _router: Router,
      private _closingChecklistService: ClosingChecklistServiceProxy,
      private _managementService: TimeManagementsServiceProxy,
      private userInfo: UserInformation,
      private http:HttpClient,
      injector: Injector) {
    super(injector)
  }
  ngOnInit() {
    this.getProfilePicture(); 
    this.initializePageParameters();

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

  initializePageParameters() {
    this.checklist = new CreateOrEditClosingChecklistDto();
    this.userName = this.appSession.user.name.toString();
    this.commantBox = true;
    this.closingMonthInputBox = true;
    this.closingMonthModalBox = false;
    this.enableValue = false;
    this.isChecked = true;
    this.dayBeforeAfter = 1;
    this.checklist.endOfMonth = false;
    this.checklist.assigneeId = 0;
    this.checklist.dueOn =1;
    if(history.state.data.createOrDuplicate != undefined){
      this.createOrDuplicate = history.state.data.createOrDuplicate;
      this.checklist.taskName = history.state.data.title;
      this.checklist.assigneeId = history.state.data.assigneeId
    }
    this.categoryTitle = history.state.data.categoryTitle == "" ? "Select Category" : history.state.data.categoryTitle;
    if (history.state.data.categoryid != 0 )
    {
      this.checklist.categoryId = history.state.data.categoryid 
    }
    this.active = true;
  }
  handleRadioChange() {
    this.daysBeforeAfter = 1;
    this.checklist.dueOn= 0;
    this.checklist.endOfMonth=true;
    this.dueOnSelected=true;
    this.isChecked = false;
    this.SelectionMsg = "\xa0"
  }
  onOpenCalendar(container) {
    container.monthSelectHandler = (event: any): void => {
      container._store.dispatch(container._actions.select(event.date));
    };
    container.setViewMode('month');
  }
  closingMonthClick(): void {
    this.closingMonthInputBox = false;
    this.closingMonthModalBox = true;
  }
  closingMonthModal(): void {
    this.closingMonthInputBox = true;
    this.closingMonthModalBox = false;
  }
  redirectToTaskList(): void {
    this._router.navigate(['/app/main/checklist']);
  }

  createTaskValidations(){
    if(this.checklist.assigneeId==undefined)
    {
      this.notify.error("Assignee is Required")
      return false
    }
    return true;
  }
  changeAssignee(){
    if(this.selectedCategoryId.categoryId!=undefined){
      this.assigneeSelected=true
    }
    else{
      this.assigneeSelected=false
    }
  }
  onCreateTask(): void {
    if(this.createTaskValidations()){
      if (this.checklist.endOfMonth) {
        this.checklist.dayBeforeAfter = 1;
        this.checklist.dueOn = 0;
      }
      else {
        this.checklist.dayBeforeAfter = this.daysBeforeAfter;
        this.checklist.endOfMonth = false;
      }
      this.checklist.dueOn = Number(this.checklist.dueOn);
      this.checklist.frequency = Number(this.checklist.frequency);
      this.checklist.status = 1
    
      if (this.selectedCategoryId.categoryId != undefined)
      {
        this.checklist.categoryId = Number(this.selectedCategoryId.categoryId);
      }
      if (this.selectedUserId.selectedUserId != undefined)
      {
        this.checklist.assigneeId = Number(this.selectedUserId.selectedUserId);
      }
      if (this.attachmentPaths != null) {
        this.newAttachementPath = [];
        this.attachmentPaths.forEach(element => {
          this.newAttachementPath.push(element.toString())
        });

        this.checklist.attachmentsPath = this.newAttachementPath;
      }
      if (this.checklist.noOfMonths != undefined && this.checklist.noOfMonths != null) {
        this.checklist.noOfMonths = Number(this.checklist.noOfMonths);
      }
      else {
        this.checklist.noOfMonths = 0;
      }
      this.errorMessage = "";
      this.saving = true;
      this._closingChecklistService.createOrEdit(this.checklist)
      .pipe(finalize(() => { this.saving = false; }))
      .subscribe(() => {
        this.redirectToTaskList();
        this.notify.success(this.l('SavedSuccessfully'));
      });
    }
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
    this.checklist.commentBody = "";
    this.commantModal = false;
    this.commantBox = true;
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
  onDayChange() {
    if(this.SelectionMsg == "Days Before"|| this.SelectionMsg == "Days After"){
      if(this.checklist.dueOn !=0){
        console.log(this.checklist.dueOn);
        this.dueOnSelected=true
      }
    }
    else{
      this.dueOnSelected=false;
    }
    this.checklist.endOfMonth = false;
    this.isChecked = true;
  }
 
  onDaysClick(value) {
    if (value == 2) {
      this.SelectionMsg = "Days Before";
      if(this.checklist.dueOn !=0){
        console.log(this.checklist.dueOn)
        this.dueOnSelected=true;
      }
      else{
        
        this.dueOnSelected=false;
      }
      this.checklist.endOfMonth = false;
      this.isChecked = true;
    }
    else if (value == 3) {
      this.SelectionMsg = "Days After";
      this.checklist.endOfMonth = false;
      if(this.checklist.dueOn !=0){
        this.dueOnSelected=true;
      }else{
        
        this.dueOnSelected=false;
      }
      this.isChecked = true;
    }  
    else if (value == 1) {
      this.SelectionMsg = "\xa0"
      this.isChecked = true;
    }
  }
  loadDaysDropdown(): void {
    var month = moment(new Date(add(this.checklist.closingMonth, 2, "day")));
    this._closingChecklistService.getCurrentMonthDays(month).subscribe(result => {
      this.days = result;
    });
  }
  onChange(val) {
    if (val == 5) {
      this.endOnIsEnabled = false;
    }
    else {
      this.endOnIsEnabled = true;
    }
    if (val == 4) {
      this.enableValue = true;
    }
    else {
      this.enableValue = false;
    }
  }
  uploadFile($event) {
    this.instructionFiles.push($event.target.files[0]);
  }
  removeFile(index)
  {
    this.instructionFiles.splice(index, 1);
  }
  attachmentPathsTrialBalance:any=[];
  uploadCommentFile($event) {
    this.commentFiles.push($event.target.files[0]);
    var response
    let formData= new FormData();
    for(let file of this.commentFiles){
      formData.append(file.name, file)
    }
    let uploadReq= new HttpRequest('POST',AppConsts.remoteServiceBaseUrl + '/api/services/app/Attachments/PostAttachmentFile',formData ,{
      reportProgress: true,
    })
    this.http.request(uploadReq).subscribe(event => {
        console.log(event)
      });
  }
  removeCommentFile(index)
  {
    this.commentFiles.splice(index, 1);
  }
  loadDaysByMonth():void{
    var month = moment(new Date(add(this.checklist.closingMonth, 2, "day")));
    this._closingChecklistService.getCurrentMonthDays(month).subscribe(result => {
      this.days = result;
      this.checklist.dueOn =1
    });
    this._managementService.checkMonthStatus(moment(new Date(add(this.checklist.closingMonth, 2, "day")))).subscribe(result => {
      this.monthStatus = result;
    });
  }
   getNoOfmonths(date1, date2) {
    var Nomonths;
    Nomonths= (date2.getFullYear() - date1.getFullYear()) * 12;
    Nomonths-= date1.getMonth() + 1;
    Nomonths+= date2.getMonth() +1; // we should add + 1 to get correct month number
    return Nomonths <= 0 ? 0 : Nomonths;
}
}
