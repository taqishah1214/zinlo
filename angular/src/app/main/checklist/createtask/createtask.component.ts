import { Component, OnInit, ViewChild, Injector } from '@angular/core';
import { Router } from '@angular/router';
import { CreateOrEditClosingChecklistDto, ClosingChecklistServiceProxy } from '@shared/service-proxies/service-proxies';
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
@Component({
  selector: 'app-createtask',
  templateUrl: './createtask.component.html',
  styleUrls: ['./createtask.component.css']
})
export class CreatetaskComponent extends AppComponentBase implements OnInit {
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
  newAttachementPath: string[] = [];
  public isChecked: boolean = false;
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
  constructor
    (private _router: Router,
      private _closingChecklistService: ClosingChecklistServiceProxy,
      private userInfo: UserInformation,
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


  onCreateTask(): void {
  
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
    this.checklist.endOfMonth = false;
    this.isChecked = true;
  }
 
  onDaysClick(value) {
    if (value == 2) {
      this.SelectionMsg = "Days Before";
      this.checklist.endOfMonth = false;
      this.isChecked = true;
    }
    else if (value == 3) {
      this.SelectionMsg = "Days After";
      this.checklist.endOfMonth = false;
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
  loadDaysByMonth(event):void{
    var month = moment(new Date(add(this.checklist.closingMonth, 2, "day")));
    this._closingChecklistService.getCurrentMonthDays(month).subscribe(result => {
      this.days = result;
      this.checklist.dueOn =1
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
