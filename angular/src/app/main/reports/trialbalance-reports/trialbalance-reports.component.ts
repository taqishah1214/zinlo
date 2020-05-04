import { Component, Injector, ViewChild, Output, EventEmitter } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { Table } from 'primeng/components/table/table';
import { Paginator } from 'primeng/components/paginator/paginator';
import { LazyLoadEvent } from 'primeng/components/common/lazyloadevent';
import * as _ from 'lodash';
import { AppConsts } from '@shared/AppConsts';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { TrialBalanceReportingServiceProxy } from '@shared/service-proxies/service-proxies';
import { StoreDateService } from "../../../services/storedate.service";
import { add, subtract } from 'add-subtract-date';
import { FileDownloadService } from '@shared/utils/file-download.service';


@Component({
  selector: 'app-trialbalance-reports',
  templateUrl: './trialbalance-reports.component.html',
  styleUrls: ['./trialbalance-reports.component.css']
})
export class TrialbalanceReportsComponent extends AppComponentBase {

  @ViewChild('dataTable', { static: true }) dataTable: Table;
  @ViewChild('paginator', { static: true }) paginator: Paginator;
  @Output() recordid = new EventEmitter<number>();
  advancedFiltersAreShown = false;
  filterText = '';
  fileUrl;
  exceptionsList: any;
  FirstMonth: any = "";
  SecondMonth: any = "";
  modalButtonText: any = "Next";
  users: any;
  firstMonthData : any = [];
  secondMonthData : any = [];
  compareTable  = false;
  firstMonthid
  secondMonthid
  compareBalanceList : any = []
  constructor(
    injector: Injector,
    private _trialBalanceServiceProxy: TrialBalanceReportingServiceProxy, private userDate: StoreDateService,private _fileDownloadService: FileDownloadService) {
    super(injector);
  }
  ngOnInit() {
    this.userDate.allUsersInformationofTenant.subscribe(userList => this.users = userList)
  }

  getAllImportLog(event?: LazyLoadEvent) {
    if (this.compareTable == false)
    {

      if (this.primengTableHelper.shouldResetPaging(event)) {
        this.paginator.changePage(0);
        return;
      }
      this.primengTableHelper.showLoadingIndicator();
      this._trialBalanceServiceProxy.getAll(
        this.filterText,
        this.primengTableHelper.getSorting(this.dataTable),
        this.primengTableHelper.getSkipCount(this.paginator, event),
        this.primengTableHelper.getMaxResultCount(this.paginator, event)
      ).subscribe(result => {
        this.primengTableHelper.totalRecordsCount = result.totalCount;
        let data = result.items;
        data.forEach(i => {
          i["assigniName"] = this.users[this.getUserIndex(i.createdById)].name;
          i["profilePicture"] = this.users[this.getUserIndex(i.createdById)].profilePicture;
  
          // var attachmentName = i.filePath.substring(i.filePath.lastIndexOf("/") + 1, i.filePath.lastIndexOf("zinlo"));
          // i["attachmentName"] = attachmentName
          // i["attachmentUrl"] =  i.filePath
        })
  
        this.primengTableHelper.records = data;
        this.primengTableHelper.hideLoadingIndicator();
        this.exceptionsList = result.items;
        
        this.primengTableHelper.records = this.exceptionsList;
      });

    }
    else {  
      if (this.primengTableHelper.shouldResetPaging(event)) {
        this.paginator.changePage(0);
        return;
      }
      this.primengTableHelper.showLoadingIndicator();
      this._trialBalanceServiceProxy.getCompareTrialBalances(
        this.firstMonthid,
        this.secondMonthid,
        this.primengTableHelper.getSorting(this.dataTable),
        this.primengTableHelper.getSkipCount(this.paginator, event),
        this.primengTableHelper.getMaxResultCount(this.paginator, event)
      ).subscribe(result => {
        this.primengTableHelper.totalRecordsCount = result.totalCount;
        this.primengTableHelper.records =  result.items;
        this.compareBalanceList = result.items
        this.primengTableHelper.hideLoadingIndicator();  
        this.exceptionsList = result.items;      
        this.primengTableHelper.records = this.exceptionsList;
      });

    }
    
  }
  downloadFile(path) {
    window.open(AppConsts.remoteServiceBaseUrl + "/" + path);
  }
  getExtensionImagePath(str) {

    var extension = str.split('.')[1];
    extension = extension + ".svg";
    return extension;
  }
 

  getUserIndex(id) {
    return this.users.findIndex(x => x.id === id);
  }
  onOpenCalendar(container) {
    container.monthSelectHandler = (event: any): void => {
      container._store.dispatch(container._actions.select(event.date));
    };
    container.setViewMode('month');
  }

  onSelectionChange (key) {
   this.firstMonthid = key;
  }

   onSelectionSecondMonthChange(key){
   this.secondMonthid = key
  }

  DownloadInExcel() {
    this._trialBalanceServiceProxy.getInToExcel(this.compareBalanceList).subscribe(resp => {
      this._fileDownloadService.downloadTempFile(resp);
    })
  }
  LoadandCompareTrialBalance() {
    if (this.modalButtonText == "Compare")
    {
        this.compareTable = true;
        this.getAllImportLog();
    }
    else {
      if (this.FirstMonth == "" || this.SecondMonth == "") {
        this.notify.error(this.l('Please select the month.'))
      }
      else {
        this.FirstMonth = new Date(add(this.FirstMonth , 2, "day"));
        this.SecondMonth = new Date(add(this.SecondMonth, 2, "day"));
        this.modalButtonText = "Compare"
        this._trialBalanceServiceProxy.getTrialBalancesofSpecficMonth(this.FirstMonth).subscribe(result => {
          this.firstMonthData = result;
          this.firstMonthData.forEach(i => {
            var attachmentName = i.name.substring(i.name.lastIndexOf("/") + 1, i.name.lastIndexOf("zinlo"));
            i["attachmentName"] = attachmentName
          })       
          this._trialBalanceServiceProxy.getTrialBalancesofSpecficMonth(this.SecondMonth).subscribe(result => {
            this.secondMonthData = result
            this.secondMonthData.forEach(i => {
              var attachmentName = i.name.substring(i.name.lastIndexOf("/") + 1, i.name.lastIndexOf("zinlo"));
              i["attachmentName"] = attachmentName  
            })
            ;  
          })
        })
  
      }
    }

    
  }

  reloadPage(): void {
    this.paginator.changePage(this.paginator.getPage());
  }
  rollBack(id): void {
    // this.message.confirm(
    //     this.l('RollBackWarningMessage'),
    //     this.l('AreYouSure'),
    //     (isConfirmed) => {
    //         if (isConfirmed) {
    //             this._trialBalanceServiceProxy.rollBackTrialBalance(id)
    //                 .subscribe(() => {
    //                     this.getAllImportLog();
    //                     this.notify.success(this.l('SuccessfullyRolledBack'));
    //                 });
    //         }
    //     }
    // );
  }
}

