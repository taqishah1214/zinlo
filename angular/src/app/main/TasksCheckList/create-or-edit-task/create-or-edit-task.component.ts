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
  assigneeName: string;
  comment: string;
  closingMonth: string;
  frequenct: string;
  commantModal: boolean;
  commantBox: boolean;
  closingMonthInputBox: boolean;
  closingMonthModalBox: boolean;
  userSignInName : string;
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

    this.userSignInName =  this.appSession.user.name.toString().charAt(0).toUpperCase();
    this.commantBox = true;
    this.closingMonthInputBox = true;
    this.closingMonthModalBox = false;
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


  onChangeAssigniName(value): void {
    this.checklist.assigneeNameId = value
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
    this.checklist.assigneeNameId = Number(this.selectedUserId.selectedUserId.value);
    this.checklist.categoryId = Number(this.selectedCategoryId.categoryId);
    this.checklist.noOfMonths = Number(this.checklist.noOfMonths);
    this._closingChecklistService.createOrEdit(this.checklist).subscribe(() => {
      this.notify.success(this.l('SavedSuccessfully'));
    });
  }

  commentClick() : void {
      this.commantModal = true;
      this.commantBox = false;
    }

  onComment() : void {
      this.commantModal = false;
      this.commantBox = true;
    }
  onCancelComment() : void {
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


}

