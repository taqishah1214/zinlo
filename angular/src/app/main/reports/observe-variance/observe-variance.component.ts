import { Component, Injector, ViewChild, Output, EventEmitter } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { Table } from 'primeng/components/table/table';
import { Paginator } from 'primeng/components/paginator/paginator';
import { LazyLoadEvent } from 'primeng/components/common/lazyloadevent';
import * as _ from 'lodash';
import { AppConsts } from '@shared/AppConsts';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { TrialBalanceReportingServiceProxy, ChartsofAccountServiceProxy, CompareVarianceServiceProxy } from '@shared/service-proxies/service-proxies';
import { StoreDateService } from "../../../services/storedate.service";
import { add, subtract } from 'add-subtract-date';
import { FileDownloadService } from '@shared/utils/file-download.service';
import * as moment from 'moment';


@Component({
  selector: 'app-observe-variance',
  templateUrl: './observe-variance.component.html',
  styleUrls: ['./observe-variance.component.css']
})
export class ObserveVarianceComponent extends AppComponentBase {

  @ViewChild('dataTable', { static: true }) dataTable: Table;
  @ViewChild('paginator', { static: true }) paginator: Paginator;
  @Output() recordid = new EventEmitter<number>();
  advancedFiltersAreShown = false;
  filterText = '';
  fileUrl;
  exceptionsList: any;
  FirstMonth: any = "";
  SecondMonth: any = "";
  modalButtonText: any = "Compare";
  users: any;
  firstMonthData : any = [];
  secondMonthData : any = [];
  compareTable  = false;
  firstMonthid = 0;
  secondMonthid = 0;
  compareBalanceList : any = []
  constructor(
    injector: Injector,
    private _trialBalanceServiceProxy: TrialBalanceReportingServiceProxy,private _chartOfAccountService: ChartsofAccountServiceProxy,private  _compareVarianceServiceProxy: CompareVarianceServiceProxy , private userDate: StoreDateService,private _fileDownloadService: FileDownloadService) {
    super(injector);
  }
  ngOnInit() {
    this.userDate.allUsersInformationofTenant.subscribe(userList => this.users = userList)
  }
  redirectToVariance(){
    this.compareTable=false
    this.ngOnInit()
    this.getAllImportLog();
  }
  getAllImportLog(event?: LazyLoadEvent) {
    if (this.compareTable == false)
    {
      if (this.primengTableHelper.shouldResetPaging(event)) {
        this.paginator.changePage(0);
        return;
      }
      this.primengTableHelper.showLoadingIndicator();
      this._chartOfAccountService.getAll(
        "",
        0,
        moment(new Date()),
        0,
        false,
        false,
        this.primengTableHelper.getSorting(this.dataTable),
        this.primengTableHelper.getSkipCount(this.paginator, event),
        this.primengTableHelper.getMaxResultCount(this.paginator, event)
      ).subscribe(result => {
        this.primengTableHelper.totalRecordsCount = result.totalCount;
        let data = result.items;
        data.forEach(i => {
          i["assigneeName"] =  this.users[this.getUserIndex(i.assigneeId)].name;
          i["profilePicture"] =  this.users[this.getUserIndex(i.assigneeId)].profilePicture;
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
      this._compareVarianceServiceProxy.getCompareTrialBalances(
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

  firstmonth(event) {
    this.FirstMonth = new Date(add(event , 2, "day"));
    this._trialBalanceServiceProxy.getTrialBalancesofSpecficMonth(this.FirstMonth).subscribe(result => {
      this.firstMonthData = result;
      this.firstMonthData.forEach(i => {
        var attachmentName = i.name.substring(i.name.lastIndexOf("/") + 1, i.name.lastIndexOf("zinlo"));
        i["attachmentName"] = attachmentName  
        var creationDate=i.creationTime 
        i["creationTime"]=creationDate 
      }) 
    })
  }

  secondMonth(event){
    this.SecondMonth = new Date(add(event, 2, "day"));
    this._trialBalanceServiceProxy.getTrialBalancesofSpecficMonth(this.SecondMonth).subscribe(result => {
      this.secondMonthData = result
      this.secondMonthData.forEach(i => {
        var attachmentName = i.name.substring(i.name.lastIndexOf("/") + 1, i.name.lastIndexOf("zinlo"));
        i["attachmentName"] = attachmentName  
        var creationDate=i.creationTime 
        i["creationTime"]=creationDate   
        this.modalButtonText = "Compare"
      })
      ;  
    })
  }

  DownloadInExcel() {
    this._compareVarianceServiceProxy.getInToExcel(this.compareBalanceList,moment(this.FirstMonth),moment (this.SecondMonth)).subscribe(resp => {
      this._fileDownloadService.downloadTempFile(resp);
    })
  }
  LoadandCompareTrialBalance() {
    if (this.modalButtonText == "Compare")
    {
      if (this.firstMonthid != 0 && this.secondMonthid != 0 ){
        this.compareTable = true;
        this.getAllImportLog();
      }
      else{
        this.notify.error(this.l('Please select the version in the month.'))
      }
    }
    else {
      if (this.FirstMonth == "" || this.SecondMonth == "") {
        this.notify.error(this.l('Please select the month.'))
      }
    }   
  }

  reloadPage(): void {
    this.paginator.changePage(this.paginator.getPage());
  }
}

