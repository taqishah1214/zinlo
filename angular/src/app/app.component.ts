import { Component, Injector, OnInit, ViewChild } from '@angular/core';
import { AppConsts } from '@shared/AppConsts';
import { UrlHelper } from '@shared/helpers/UrlHelper';
import { SubscriptionStartType, SystemSettingsServiceProxy } from '@shared/service-proxies/service-proxies';
import { ChatSignalrService } from 'app/shared/layout/chat/chat-signalr.service';
import * as moment from 'moment';
import { AppComponentBase } from 'shared/common/app-component-base';
import { SignalRHelper } from 'shared/helpers/SignalRHelper';
import { LinkedAccountsModalComponent } from '@app/shared/layout/linked-accounts-modal.component';
import { LoginAttemptsModalComponent } from '@app/shared/layout/login-attempts-modal.component';
import { ChangePasswordModalComponent } from '@app/shared/layout/profile/change-password-modal.component';
import { ChangeProfilePictureModalComponent } from '@app/shared/layout/profile/change-profile-picture-modal.component';
import { MySettingsModalComponent } from '@app/shared/layout/profile/my-settings-modal.component';
import { NotificationSettingsModalComponent } from '@app/shared/layout/notifications/notification-settings-modal.component';
import { UserNotificationHelper } from '@app/shared/layout/notifications/UserNotificationHelper';
import { UserServiceProxy,AccountSubTypeServiceProxy ,CategoriesServiceProxy} from '@shared/service-proxies/service-proxies';
import { StoreDateService } from './services/storedate.service';

@Component({
    templateUrl: './app.component.html',
    styleUrls: ['./app.component.less']
})
export class AppComponent extends AppComponentBase implements OnInit {

    subscriptionStartType = SubscriptionStartType;
    theme: string;
    installationMode = true;
    userList :any = []
    categoriesList :any = []
    accountSubTypeList :any = []
    defaultMonth : any;
    @ViewChild('loginAttemptsModal', {static: true}) loginAttemptsModal: LoginAttemptsModalComponent;
    @ViewChild('linkedAccountsModal', {static: false}) linkedAccountsModal: LinkedAccountsModalComponent;
    @ViewChild('changePasswordModal', {static: true}) changePasswordModal: ChangePasswordModalComponent;
    @ViewChild('changeProfilePictureModal', {static: true}) changeProfilePictureModal: ChangeProfilePictureModalComponent;
    @ViewChild('mySettingsModal', {static: true}) mySettingsModal: MySettingsModalComponent;
    @ViewChild('notificationSettingsModal', {static: true}) notificationSettingsModal: NotificationSettingsModalComponent;
    @ViewChild('chatBarComponent', {static: false}) chatBarComponent;
    isQuickThemeSelectEnabled: boolean = this.setting.getBoolean('App.UserManagement.IsQuickThemeSelectEnabled');
    IsSessionTimeOutEnabled: boolean = this.setting.getBoolean('App.UserManagement.SessionTimeOut.IsEnabled') && this.appSession.userId != null;

    public constructor(
        injector: Injector,
        private _chatSignalrService: ChatSignalrService,
        private _userNotificationHelper: UserNotificationHelper,
        private storeData: StoreDateService,private _userService: UserServiceProxy,private _categoryService: CategoriesServiceProxy, private _accountSubtypeService :AccountSubTypeServiceProxy,
        private _systemSettingsService: SystemSettingsServiceProxy
    ) {
        super(injector);
    }

    ngOnInit(): void {
        this.realoadApp();      
        this._userNotificationHelper.settingsModal = this.notificationSettingsModal;
        this.theme = abp.setting.get('App.UiManagement.Theme').toLocaleLowerCase();
        this.installationMode = UrlHelper.isInstallUrl(location.href);

        this.registerModalOpenEvents();

        if (this.appSession.application) {
            SignalRHelper.initSignalR(() => { this._chatSignalrService.init(); });
        }
    }

    realoadApp() {
        this._systemSettingsService.getDefaultMonth().subscribe(result => { 
            this.defaultMonth = result
            this.storeData.setDefaultMonth(this.defaultMonth)
        })  
        this._userService.getAllUsers().subscribe(result => { 
            this.userList = result
            this.storeData.setUserList(this.userList)
        })  
        this._categoryService.categoryDropDown().subscribe(result => { 
            this.categoriesList = result
            this.storeData.setCategoriesList(this.categoriesList)
        })  
        this._accountSubtypeService.accountSubTypeDropDown().subscribe(result => { 
            this.accountSubTypeList = result
            this.storeData.setAccountSubTypeList(this.accountSubTypeList)
        })  
      }
    subscriptionStatusBarVisible(): boolean {
        return this.appSession.tenantId > 0 &&
            (this.appSession.tenant.isInTrialPeriod ||
                this.subscriptionIsExpiringSoon());
    }

    subscriptionIsExpiringSoon(): boolean {
        if (this.appSession.tenant.subscriptionEndDateUtc) {
            return moment().utc().add(AppConsts.subscriptionExpireNootifyDayCount, 'days') >= moment(this.appSession.tenant.subscriptionEndDateUtc);
        }

        return false;
    }

    registerModalOpenEvents(): void {
        abp.event.on('app.show.loginAttemptsModal', () => {
            this.loginAttemptsModal.show();
        });

        abp.event.on('app.show.linkedAccountsModal', () => {
            this.linkedAccountsModal.show();
        });

        abp.event.on('app.show.changePasswordModal', () => {
            this.changePasswordModal.show();
        });

        abp.event.on('app.show.changeProfilePictureModal', () => {
            this.changeProfilePictureModal.show();
        });

        abp.event.on('app.show.mySettingsModal', () => {
            this.mySettingsModal.show();
        });
    }

    getRecentlyLinkedUsers(): void {
        abp.event.trigger('app.getRecentlyLinkedUsers');
    }

    onMySettingsModalSaved(): void {
        abp.event.trigger('app.onMySettingsModalSaved');
    }
}
