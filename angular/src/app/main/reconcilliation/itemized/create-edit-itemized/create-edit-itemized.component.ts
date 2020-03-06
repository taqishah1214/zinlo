import { Component, OnInit, ViewChild, Injector } from '@angular/core';
import { ChartsofAccountServiceProxy, CreateOrEditItemizationDto, ItemizationServiceProxy, GetEditionEditOutput } from '@shared/service-proxies/service-proxies';
import { Router } from '@angular/router';
import { UserListComponentComponent } from '@app/main/checklist/user-list-component/user-list-component.component';
import { AppComponentBase } from '@shared/common/app-component-base';
import { UppyConfig } from 'uppy-angular';
import { AppConsts } from '@shared/AppConsts';

@Component({
  selector: 'app-create-edit-itemized',
  templateUrl: './create-edit-itemized.component.html',
  styleUrls: ['./create-edit-itemized.component.css']
  
})
export class CreateEditItemizedComponent extends AppComponentBase implements OnInit  {
  itemizedDto : CreateOrEditItemizationDto = new CreateOrEditItemizationDto();
  minDate: Date = new Date();
  title = "Add an Item";
  Save  = "Save";
  accountName : any;
  accountNo : any;
  accountId : any;
  @ViewChild(UserListComponentComponent, { static: false }) selectedUserId: UserListComponentComponent;
  recordId: number;
  constructor(private _itemizationServiceProxy : ItemizationServiceProxy,
    private _chartOfAccountService: ChartsofAccountServiceProxy, private _router: Router, injector: Injector) {
    super(injector)
  }

  ngOnInit() {   
    this.accountId = history.state.data.accountId
    this.recordId = history.state.data.id;
    this.accountName = history.state.data.accountName
    this.accountNo = history.state.data.accountNo
    debugger;
    if(this.recordId != 0)
    {
      this.title = "Edit Item";
      this.Save = "Update";
      this.GetEdit(this.recordId);
    }
  
  }
  redirectToItemsList () : void {
    this._router.navigate(['/app/main/reconcilliation/itemized'],{ state: { data: { accountId :this.accountId , accountName :this.accountName ,accountNo: this.accountNo}} });
  }
  settings: UppyConfig = {
    uploadAPI: {
      endpoint:  AppConsts.remoteServiceBaseUrl + '/api/services/app/Attachments/PostAttachmentFile',
    },
    plugins: {
      Webcam: false
    }
  }

  onOpenCalendar(container) {
    container.monthSelectHandler = (event: any): void => {
      container._store.dispatch(container._actions.select(event.date));
    };
    container.setViewMode('month');
  }
  SaveChanges(){
    this.itemizedDto.chartsofAccountId = 1;
    this._itemizationServiceProxy.createOrEdit(this.itemizedDto).subscribe(response => {
      this.notify.success(this.l('Item Successfully Created.'));
      this.redirectToItemsList();
    })
  }
  GetEdit(id):void{

    if(id !=0)
    {
      this._itemizationServiceProxy.getEdit(id).subscribe(response=>{
      this.itemizedDto = response;
      });
    }
  }

}
