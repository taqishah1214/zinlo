import { Component, OnInit, ViewChild, Injector } from '@angular/core';
import { Router } from '@angular/router';
import { CategoriesServiceProxy, CreateOrEditClosingChecklistDto, ClosingChecklistServiceProxy } from '@shared/service-proxies/service-proxies';
import * as moment from "moment";
import { CategorieDropDownComponent } from '@app/main/categories/categorie-drop-down/categorie-drop-down.component';
import { UserListComponentComponent } from '../user-list-component/user-list-component.component';
import { IgxMonthPickerComponent } from "igniteui-angular";
import { UppyConfig } from 'uppy-angular';
import { AppComponentBase } from '@shared/common/app-component-base';
import { AbpSessionService } from 'abp-ng2-module/dist/src/session/abp-session.service';
import { AppSessionService } from '@shared/common/session/app-session.service';
@Component({
  selector: 'app-create-or-edit-task',
  templateUrl: './create-or-edit-task.component.html',
  styleUrls: ['./create-or-edit-task.component.css']
})
export class CreateOrEditTaskComponent extends AppComponentBase implements OnInit {
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
  userSignInName: string;
  enableValue: boolean = false;
  endOnIsEnabled:boolean = true;
  SelectionMsg: string = "";
  checklist: CreateOrEditClosingChecklistDto = new CreateOrEditClosingChecklistDto();
  @ViewChild(CategorieDropDownComponent, { static: false }) selectedCategoryId: CategorieDropDownComponent;
  @ViewChild(UserListComponentComponent, { static: false }) selectedUserId: UserListComponentComponent;
  @ViewChild(IgxMonthPickerComponent, { static: true }) monthPicker: IgxMonthPickerComponent;
  constructor
    (private _router: Router,
      private _closingChecklistService: ClosingChecklistServiceProxy,
      injector: Injector) {
    super(injector)
  }
  ngOnInit() {
    this.userSignInName = this.appSession.user.name.toString().charAt(0).toUpperCase();
    this.commantBox = true;
    this.closingMonthInputBox = true;
    this.closingMonthModalBox = false;
    this.enableValue = false;
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
  BackToTaskList(): void {
    this._router.navigate(['/app/main/TasksCheckList/tasks']);
  }
  EndofMonthSelected(): void {
    this.checklist.endOfMonth = true;
  }
  EndofMonthUnselected(): void {
    this.checklist.endOfMonth = false;
  }
  onCreateTask(): void {
    if (this.checklist.dayBeforeAfter) {
      this.checklist.dayBeforeAfter = true;
    }
    else {
      this.checklist.dayBeforeAfter = false;
    }

    if (this.checklist.endOfMonth) {
      this.checklist.endOfMonth = true;
    }
    else {
      this.checklist.endOfMonth = false;
    }

    this.checklist.dueOn = Number(this.checklist.dueOn);
    this.checklist.frequency = Number(this.checklist.frequency);
    this.checklist.status = 1
    this.checklist.assigneeId = Number(this.selectedUserId.selectedUserId.value);
    this.checklist.categoryId = Number(this.selectedCategoryId.categoryId);

    if (this.checklist.noOfMonths != undefined && this.checklist.noOfMonths != null) {
      this.checklist.noOfMonths = Number(this.checklist.noOfMonths);
    }
    else {
      this.checklist.noOfMonths = 0;
    }
   this._closingChecklistService.createOrEdit(this.checklist).subscribe(() => {
      this.RedirectToList();
      this.notify.success(this.l('SavedSuccessfully'));
    });
  }
  RedirectToList(): void {
    this._router.navigate(['/app/main/TasksCheckList/tasks']);
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
      endpoint: ``,
      headers: {
        Authorization: 'Bearer ' + localStorage.getItem('userToken')
      }
    },
    plugins: {
      Webcam: false
    }
  }
  onChange(val) {
    if(val == 5)
    {
      this.endOnIsEnabled = false;
    }
    else{
      this.endOnIsEnabled = true;
    }
    if (val == 4) {
      this.enableValue = true;
    }
    else {
      this.enableValue = false;
    }
  }
  onValueChange() {
    this.checklist.endOfMonth = false;
  }
  handleChange(evt) {
  }
  handleRadioChange(val) {
    this.checklist.dayBeforeAfter = null;
    this.checklist.dueOn = 0;
    this.SelectionMsg = "";
  }
  onDaysClick(valu) {
    if (valu == "true") {
      this.SelectionMsg = "Days Before";
    }
    else if (valu == "false") {
      this.SelectionMsg = "Days After";
    }
  }
}

