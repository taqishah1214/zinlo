import { Component, OnInit, ViewChild, Injector } from '@angular/core';
import { Router } from '@angular/router';
import { CreateOrEditClosingChecklistDto, ClosingChecklistServiceProxy, CategoriesServiceProxy } from '@shared/service-proxies/service-proxies';
import { CategorieDropDownComponent } from '@app/main/categories/categorie-drop-down/categorie-drop-down.component';
import { IgxMonthPickerComponent } from "igniteui-angular";
import { UppyConfig } from 'uppy-angular';
import { AppComponentBase } from '@shared/common/app-component-base';
import { UserListComponentComponent } from '../user-list-component/user-list-component.component';
import { AppConsts } from '@shared/AppConsts';
@Component({
  selector: 'app-duplicate-task',
  templateUrl: './duplicate-task.component.html',
  styleUrls: ['./duplicate-task.component.css']
})
export class DuplicateTaskComponent extends AppComponentBase implements OnInit {
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
  endOnIsEnabled: boolean = true;
  SelectionMsg: string = "";
  attachmentPaths: any = [];
  newAttachementPath: string[] = [];
  taskId: any;
  taskDetails: any;
  categoriesList: any;
  categoryName: any;
  public isChecked: boolean = false;
  days: any;
  checklist: CreateOrEditClosingChecklistDto = new CreateOrEditClosingChecklistDto();
  @ViewChild(CategorieDropDownComponent, { static: false }) selectedCategoryId: CategorieDropDownComponent;
  @ViewChild(UserListComponentComponent, { static: false }) selectedUserId: UserListComponentComponent;
  @ViewChild(IgxMonthPickerComponent, { static: true }) monthPicker: IgxMonthPickerComponent;
  constructor
    (private _router: Router,
      private _closingChecklistService: ClosingChecklistServiceProxy,
      injector: Injector,
      private _categoryService: CategoriesServiceProxy) {
    super(injector)
  }
  ngOnInit() {
    this.getDetailsOfTasks();
    this.initializePageParameters();
    this.loadDaysDropdown();
  }
  initializePageParameters(): void {
    this.userSignInName = this.appSession.user.name.toString();
    this.commantBox = true;
    this.closingMonthInputBox = true;
    this.closingMonthModalBox = false;
    this.enableValue = false;
    this.isChecked = true;
  }

  getDetailsOfTasks(): void {
    this.taskId = history.state.data.id;
    this._closingChecklistService.getTaskForEdit(this.taskId).subscribe(result => {
      this.taskDetails = result;
      this.checklist.assigneeId = this.taskDetails.assigneeId;
      this.checklist.categoryId = this.taskDetails.categoryId;
      this._categoryService.categoryDropDown().subscribe(result => {
        this.categoriesList = result;
      });
    })
  }

  categoryClick(id, name): void {
    this.categoryName = name;
    this.checklist.categoryId = id;
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
  backToTaskList(): void {
    this._router.navigate(['/app/main/checklist']);
  }
  EndofMonthSelected(): void {
    this.checklist.endOfMonth = true;
  }
  EndofMonthUnselected(): void {
    this.checklist.endOfMonth = false;
  }
  onDuplicateTask(): void {

    debugger;
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
    if (this.selectedUserId.selectedUserId != undefined)
    {
      this.checklist.assigneeId = Number(this.selectedUserId.selectedUserId);
    }
    this.checklist.taskName = this.taskDetails.taskName;
    this.checklist.instruction = this.taskDetails.instruction;
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
      this.backToTaskList();
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
      endpoint: AppConsts.remoteServiceBaseUrl + '/api/services/app/Attachments/PostAttachmentFile',
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
  OnFrequencyChange(val) {
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
    }
    else if (valu == "false") {
      this.SelectionMsg = "Days After";
    }
  }

  routeToAddNewCategory() : void {
    this._router.navigate(['/app/main/categories/create-or-edit-category'], { state: { data: { id: 0, redirectPath: "duplicate", "checklistTask": this.taskId } } });

  }

  loadDaysDropdown(): void {
    this._closingChecklistService.getCurrentMonthDays(this.checklist.closingMonth).subscribe(result => {
      this.days = result;
    });
  }
  loadDaysByMonth(event):void{
    this.loadDaysDropdown();
  }
}

