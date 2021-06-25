import { IAjaxResponse } from '@abp/abpHttpInterceptor';
import { TokenService } from '@abp/auth/token.service';
import { Component, Injector, OnInit } from '@angular/core';
import { AppConsts } from '@shared/AppConsts';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { AppComponentBase } from '@shared/common/app-component-base';
import { SettingScopes, SendTestEmailInput, TenantSettingsEditDto, TenantSettingsServiceProxy,SystemSettingsServiceProxy, CreateOrEditDefaultMonthDto } from '@shared/service-proxies/service-proxies';
import { FileUploader, FileUploaderOptions } from 'ng2-file-upload';
import { finalize } from 'rxjs/operators';
import * as moment from 'moment';
import { StoreDateService } from '@app/services/storedate.service';

@Component({
    templateUrl: './tenant-settings.component.html',
    animations: [appModuleAnimation()]
})
export class TenantSettingsComponent extends AppComponentBase implements OnInit {

    usingDefaultTimeZone = false;
    initialTimeZone: string = null;
    testEmailAddress: string = undefined;
    selectedDefaultDate : any;
    isMultiTenancyEnabled: boolean = this.multiTenancy.isEnabled;
    showTimezoneSelection: boolean = abp.clock.provider.supportsMultipleTimezone;
    activeTabIndex: number = (abp.clock.provider.supportsMultipleTimezone) ? 0 : 1;
    loading = false;
    settings: TenantSettingsEditDto = undefined;
    defaultMonthDto : CreateOrEditDefaultMonthDto = new CreateOrEditDefaultMonthDto()
    logoUploader: FileUploader;
    customCssUploader: FileUploader;
    selectedDate : any = new Date ();
    defaultMonth : any;
    selectedDateId = 0;
    isWeekEndEnable:boolean=false;
    remoteServiceBaseUrl = AppConsts.remoteServiceBaseUrl;
    reload : any;
    defaultTimezoneScope: SettingScopes = SettingScopes.Tenant;

    constructor(
        injector: Injector,
        private _tenantSettingsService: TenantSettingsServiceProxy,
        private _tokenService: TokenService,
        private _systemSettingsService: SystemSettingsServiceProxy,
        private storeData: StoreDateService

    ) {
        super(injector);
    }

    ngOnInit(): void {
        this.testEmailAddress = this.appSession.user.emailAddress;
        this.getSettings();
        this.initUploaders();
        this.storeData.defaultgMonth.subscribe(defaultMonth => {
            this.defaultMonth = defaultMonth
            if (this.defaultMonth.id != 0) {
                this.selectedDate = new Date (this.defaultMonth.month);
                this.selectedDateId = this.defaultMonth.id;
                this.isWeekEndEnable=this.defaultMonth.isWeekEndEnable;
            }
            });
    }

    onWeekEndChange(isWeekEndEnable:boolean){
        this.defaultMonthDto.isWeekEndEnable=isWeekEndEnable;
        this.isWeekEndEnable=isWeekEndEnable;
    }

    getSettings(): void {
        this.loading = true;
        this._tenantSettingsService.getAllSettings()
            .pipe(finalize(() => { this.loading = false; }))
            .subscribe((result: TenantSettingsEditDto) => {
                this.settings = result;
              
                if (this.settings.general) {
                    this.initialTimeZone = this.settings.general.timezone;
                    this.usingDefaultTimeZone = this.settings.general.timezoneForComparison === abp.setting.values['Abp.Timing.TimeZone'];
                }
            });
    }
    onOpenCalendar(container) {
        container.monthSelectHandler = (event: any): void => {
          container._store.dispatch(container._actions.select(event.date));
        };
        container.setViewMode('month');
      }

    initUploaders(): void {
        this.logoUploader = this.createUploader(
            '/TenantCustomization/UploadLogo',
            result => {
                this.appSession.tenant.logoFileType = result.fileType;
                this.appSession.tenant.logoId = result.id;
            }
        );

        this.customCssUploader = this.createUploader(
            '/TenantCustomization/UploadCustomCss',
            result => {
                this.appSession.tenant.customCssId = result.id;

                let oldTenantCustomCss = document.getElementById('TenantCustomCss');
                if (oldTenantCustomCss) {
                    oldTenantCustomCss.remove();
                }

                let tenantCustomCss = document.createElement('link');
                tenantCustomCss.setAttribute('id', 'TenantCustomCss');
                tenantCustomCss.setAttribute('rel', 'stylesheet');
                tenantCustomCss.setAttribute('href', AppConsts.remoteServiceBaseUrl + '/TenantCustomization/GetCustomCss?tenantId=' + this.appSession.tenant.id);
                document.head.appendChild(tenantCustomCss);
            }
        );
    }

    createUploader(url: string, success?: (result: any) => void): FileUploader {
        const uploader = new FileUploader({ url: AppConsts.remoteServiceBaseUrl + url });

        uploader.onAfterAddingFile = (file) => {
            file.withCredentials = false;
        };

        uploader.onSuccessItem = (item, response, status) => {
            const ajaxResponse = <IAjaxResponse>JSON.parse(response);
            if (ajaxResponse.success) {
                this.notify.info(this.l('SavedSuccessfully'));
                if (success) {
                    success(ajaxResponse.result);
                }
            } else {
                this.message.error(ajaxResponse.error.message);
            }
        };

        const uploaderOptions: FileUploaderOptions = {};
        uploaderOptions.authToken = 'Bearer ' + this._tokenService.getToken();
        uploaderOptions.removeAfterUpload = true;
        uploader.setOptions(uploaderOptions);
        return uploader;
    }

    uploadLogo(): void {
        this.logoUploader.uploadAll();
    }

    uploadCustomCss(): void {
        this.customCssUploader.uploadAll();
    }

    clearLogo(): void {
        this._tenantSettingsService.clearLogo().subscribe(() => {
            this.appSession.tenant.logoFileType = null;
            this.appSession.tenant.logoId = null;
            this.notify.info(this.l('ClearedSuccessfully'));
        });
    }

    clearCustomCss(): void {
        this._tenantSettingsService.clearCustomCss().subscribe(() => {
            this.appSession.tenant.customCssId = null;

            let oldTenantCustomCss = document.getElementById('TenantCustomCss');
            if (oldTenantCustomCss) {
                oldTenantCustomCss.remove();
            }

            this.notify.info(this.l('ClearedSuccessfully'));
        });
    }

    saveAll(): void {
        this.defaultMonthDto.id = this.selectedDateId; 
        this.defaultMonthDto.month = moment(this.selectedDate);
        this._systemSettingsService.setDefaultMonth(this.defaultMonthDto).subscribe(() => {
            this._systemSettingsService.getDefaultMonth().subscribe(result => { 
                this.storeData.setDefaultMonth(result)
                this.reload.lock = true;
               this.storeData.setReloadLock(this.reload);
                this.notify.info(this.l('SavedSuccessfully'));

            }) 

        });
        this._tenantSettingsService.updateAllSettings(this.settings).subscribe(() => {
            this.notify.info(this.l('SavedSuccessfully'));

            if (abp.clock.provider.supportsMultipleTimezone && this.usingDefaultTimeZone && this.initialTimeZone !== this.settings.general.timezone) {
                this.message.info(this.l('TimeZoneSettingChangedRefreshPageNotification')).then(() => {
                    window.location.reload();
                });
            }
        });
    }

    sendTestEmail(): void {
        const input = new SendTestEmailInput();
        input.emailAddress = this.testEmailAddress;
        this._tenantSettingsService.sendTestEmail(input).subscribe(result => {
            this.notify.info(this.l('TestEmailSentSuccessfully'));
        });
    }
}
