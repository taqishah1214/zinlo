import { Component, OnInit, ViewChild, Injector } from '@angular/core';
import { ChartsofAccountServiceProxy, CreateOrEditItemizationDto, ItemizationServiceProxy, GetEditionEditOutput, AttachmentsServiceProxy } from '@shared/service-proxies/service-proxies';
import { Router } from '@angular/router';
import { UserListComponentComponent } from '@app/main/checklist/user-list-component/user-list-component.component';
import { AppComponentBase } from '@shared/common/app-component-base';
import { UppyConfig } from 'uppy-angular';
import { AppConsts } from '@shared/AppConsts';
import { finalize } from 'rxjs/operators';


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
  ItemizedItemId : any;
  attachmentPaths: any = [];
  recordId: number;
  newAttachementPath: string[] = [];
  attachments: any;
  saving = false;


  constructor(private _itemizationServiceProxy : ItemizationServiceProxy,
    private _attachmentService : AttachmentsServiceProxy, private _router: Router, injector: Injector) {
    super(injector)
  }

  ngOnInit() {   
    this.accountId = history.state.data.accountId
    this.ItemizedItemId = history.state.data.ItemizedItemId;
    this.accountName = history.state.data.accountName
    this.accountNo = history.state.data.accountNo
    if(this.ItemizedItemId != 0)
    {
      this.title = "Edit Item";
      this.Save = "Update";
      this.getDetailsofItem(this.ItemizedItemId);
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
  fileUploadedResponse(value): void {
    var response = value.successful
    response.forEach(i => {
      this.attachmentPaths.push(i.response.body.result);
    });
    this.notify.success(this.l('Attachments are SavedSuccessfully Upload'));
  }

  onOpenCalendar(container) {
    container.monthSelectHandler = (event: any): void => {
      container._store.dispatch(container._actions.select(event.date));
    };
    container.setViewMode('month');
  }
  SaveItem(){
    this.saving = true;  
    if (this.attachmentPaths != null) {
      this.newAttachementPath = [];
      this.attachmentPaths.forEach(element => {
      this.newAttachementPath.push(element.toString())
  });

  this.itemizedDto.attachmentsPath = this.newAttachementPath;
}
    this.itemizedDto.chartsofAccountId = this.accountId;
    this._itemizationServiceProxy.createOrEdit(this.itemizedDto).pipe(finalize(() => { this.saving = false; })).subscribe(response => {
      this.notify.success(this.l('Item Successfully Created.'));
      this.redirectToItemsList();
    })
  }
  getExtensionImagePath(str) {

    var extension = str.split('.')[1];
    extension = extension + ".svg";
    return extension;
  }

  deleteAttachment(id): void {
    let self = this;
        self.message.confirm(
            "",
            this.l('AreYouSure'),
            isConfirmed => {
                if (isConfirmed) {
                  this._attachmentService.deleteAttachmentPath(id)
                  .subscribe(() => {
                    this.notify.success(this.l('Attachment is successfully removed'));
                    this.getDetailsofItem(this.ItemizedItemId);
                  });
                }
            }
        );
  }
  getDetailsofItem(id):void{
      this._itemizationServiceProxy.getEdit(id).subscribe(result=>{
      this.itemizedDto = result;
      this.attachments = result.attachments;
      this.attachments.forEach(element => {
        var attachmentName = element.attachmentPath.substring(element.attachmentPath.lastIndexOf("/") + 1, element.attachmentPath.lastIndexOf("zinlo"));
        element["attachmentExtension"] = this.getExtensionImagePath(element.attachmentPath)
        element["attachmentName"] = attachmentName
        element["attachmentUrl"] = AppConsts.remoteServiceBaseUrl+"/" + element.attachmentPath
      });

      });
    
  }

}


