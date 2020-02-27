import { Component, OnInit, ViewChild, Injector, ViewEncapsulation, OnChanges } from '@angular/core';
import { Router, Data } from '@angular/router';
import { CreateOrEditClosingChecklistDto, ClosingChecklistServiceProxy } from '@shared/service-proxies/service-proxies';
import { CategorieDropDownComponent } from '@app/main/categories/categorie-drop-down/categorie-drop-down.component';
import { UserListComponentComponent } from '../user-list-component/user-list-component.component';
import { IgxMonthPickerComponent } from "igniteui-angular";
import { UppyConfig } from 'uppy-angular';
import { AppComponentBase } from '@shared/common/app-component-base';
import { AppConsts } from '@shared/AppConsts';
import {UserInformation} from "../../CommonFunctions/UserInformation"
@Component({
  selector: 'app-createtask',
  templateUrl: './createtask.component.html',
  styleUrls: ['./createtask.component.css']
})
export class CreatetaskComponent extends AppComponentBase implements OnInit {
  
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
  endOnIsEnabled: boolean = true;
  SelectionMsg: string = "";
  userName : string;
  attachmentPaths: any = [];
  newAttachementPath: string[] = [];
  public isChecked: boolean = false;
  days: any;
  users: any;
  checklist: CreateOrEditClosingChecklistDto = new CreateOrEditClosingChecklistDto();
  minDate: Date = new Date()
  categoryTitle : any;
  @ViewChild(CategorieDropDownComponent, { static: false }) selectedCategoryId: CategorieDropDownComponent;
  @ViewChild(UserListComponentComponent, { static: false }) selectedUserId: UserListComponentComponent;
  @ViewChild(IgxMonthPickerComponent, { static: true }) monthPicker: IgxMonthPickerComponent;

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
    this.loadDaysDropdown();
   
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
    this.userName = this.appSession.user.name.toString();
    this.commantBox = true;
    this.closingMonthInputBox = true;
    this.closingMonthModalBox = false;
    this.enableValue = false;
    this.isChecked = true;
    this.categoryTitle = history.state.data.categoryTitle == "" ? "Select Category" : history.state.data.categoryTitle;
    if (history.state.data.categoryid != 0 )
    {
      this.checklist.categoryId = history.state.data.categoryid 
    }
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
  EndofMonthSelected(): void {
    this.checklist.endOfMonth = true;
  }
  EndofMonthUnselected(): void {
    this.checklist.endOfMonth = false;
  }
  onCreateTask(): void {
     if (this.checklist.endOfMonth) {
      this.checklist.endOfMonth = true;
    }
    else {
      this.checklist.endOfMonth = false;
    }

    this.checklist.dueOn = Number(this.checklist.dueOn);
    this.checklist.frequency = Number(this.checklist.frequency);
    this.checklist.status = 1
    this.checklist.assigneeId = Number(this.selectedUserId.selectedUserId);
    if (this.selectedCategoryId.categoryId != undefined)
    {
      this.checklist.categoryId = Number(this.selectedCategoryId.categoryId);
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
    this._closingChecklistService.createOrEdit(this.checklist).subscribe(() => {
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
  onDayChange() {
    this.checklist.endOfMonth = false;
    this.isChecked = true;
  }
  handleRadioChange() {
    this.checklist.dayBeforeAfter = null;
    this.checklist.dueOn = 0;
    this.SelectionMsg = "";
    this.isChecked = false;
  }
  onDaysClick(valu) {
    this.isChecked = true;
    if (valu == "true") {
      this.SelectionMsg = "Days Before";
      this.checklist.dayBeforeAfter = true
    }
    else if (valu == "false") {
      this.SelectionMsg = "Days After";
      this.checklist.dayBeforeAfter = false

    }
  }
  loadDaysDropdown(): void {
    this._closingChecklistService.getCurrentMonthDays().subscribe(result => {
      this.days = result;
     
    });
  }
}
