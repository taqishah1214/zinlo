import { AbpModule } from '@abp/abp.module';
import * as ngCommon from '@angular/common';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { HttpClientModule, HttpClientJsonpModule } from '@angular/common/http';
import { ChatSignalrService } from '@app/shared/layout/chat/chat-signalr.service';
import { LinkAccountModalComponent } from '@app/shared/layout/link-account-modal.component';
import { LinkedAccountsModalComponent } from '@app/shared/layout/linked-accounts-modal.component';
import { LoginAttemptsModalComponent } from '@app/shared/layout/login-attempts-modal.component';
import { ChangePasswordModalComponent } from '@app/shared/layout/profile/change-password-modal.component';
import { ChangeProfilePictureModalComponent } from '@app/shared/layout/profile/change-profile-picture-modal.component';
import { MySettingsModalComponent } from '@app/shared/layout/profile/my-settings-modal.component';
import { SmsVerificationModalComponent } from '@app/shared/layout/profile/sms-verification-modal.component';
import { ServiceProxyModule } from '@shared/service-proxies/service-proxy.module';
import { UtilsModule } from '@shared/utils/utils.module';
import { FileUploadModule } from 'ng2-file-upload';
import { ModalModule, TabsModule, TooltipModule, BsDropdownModule, PopoverModule, BsDatepickerModule } from 'ngx-bootstrap';
import { FileUploadModule as PrimeNgFileUploadModule } from 'primeng/fileupload';
import { PaginatorModule } from 'primeng/paginator';
import { ProgressBarModule } from 'primeng/progressbar';
import { TableModule } from 'primeng/table';
import { ImpersonationService } from './admin/users/impersonation.service';
import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { DefaultLayoutComponent } from './shared/layout/themes/default/default-layout.component';
import { AppCommonModule } from './shared/common/app-common.module';
import { ChatBarComponent } from './shared/layout/chat/chat-bar.component';
import { ThemeSelectionPanelComponent } from './shared/layout/theme-selection/theme-selection-panel.component';
import { ChatFriendListItemComponent } from './shared/layout/chat/chat-friend-list-item.component';
import { ChatMessageComponent } from './shared/layout/chat/chat-message.component';
import { FooterComponent } from './shared/layout/footer.component';
import { StackedFooterComponent } from './shared/layout/stacked-footer.component';
import { LinkedAccountService } from './shared/layout/linked-account.service';
import { SideBarMenuComponent } from './shared/layout/nav/side-bar-menu.component';
import { TopBarMenuComponent } from './shared/layout/nav/top-bar-menu.component';
import { TopBarComponent } from './shared/layout/topbar.component';
import { DefaultBrandComponent } from './shared/layout/themes/default/default-brand.component';
import { UserNotificationHelper } from './shared/layout/notifications/UserNotificationHelper';
import { HeaderNotificationsComponent } from './shared/layout/notifications/header-notifications.component';
import { NotificationSettingsModalComponent } from './shared/layout/notifications/notification-settings-modal.component';
import { NotificationsComponent } from './shared/layout/notifications/notifications.component';
import { NgxChartsModule } from '@swimlane/ngx-charts';
import { TextMaskModule } from 'angular2-text-mask';
import { ImageCropperModule } from 'ngx-image-cropper';



// Metronic
import { PerfectScrollbarModule, PERFECT_SCROLLBAR_CONFIG, PerfectScrollbarConfigInterface } from 'ngx-perfect-scrollbar';
const DEFAULT_PERFECT_SCROLLBAR_CONFIG: PerfectScrollbarConfigInterface = {
    // suppressScrollX: true
};

import { CoreModule } from '@metronic/app/core/core.module';
import { SessionTimeoutModalComponent } from './shared/common/session-timeout/session-timeout-modal-component';
import { SessionTimeoutComponent } from './shared/common/session-timeout/session-timeout.component';
import { AutoCompleteModule } from 'primeng/autocomplete';
import { MenuSearchBarComponent } from './shared/layout/nav/menu-search-bar/menu-search-bar.component';
import { NgxSpinnerModule, NgxSpinnerComponent } from 'ngx-spinner';
import { ScrollTopComponent } from './shared/layout/scroll-top.component';
import { ServiceWorkerModule } from '@angular/service-worker';
import { environment } from '../environments/environment';

import { ReactiveFormsModule } from '@angular/forms';
import { AngularEditorModule } from '@kolkov/angular-editor';


@NgModule({
    declarations: [
        AppComponent,
        DefaultLayoutComponent,
        HeaderNotificationsComponent,
        SideBarMenuComponent,
        TopBarMenuComponent,
        FooterComponent,
        ScrollTopComponent,
        StackedFooterComponent,
        LoginAttemptsModalComponent,
        LinkedAccountsModalComponent,
        LinkAccountModalComponent,
        ChangePasswordModalComponent,
        ChangeProfilePictureModalComponent,
        MySettingsModalComponent,
        SmsVerificationModalComponent,
        NotificationsComponent,
        ChatBarComponent,
        ThemeSelectionPanelComponent,
        ChatFriendListItemComponent,
        NotificationSettingsModalComponent,
        ChatMessageComponent,
        TopBarComponent,
        DefaultBrandComponent,
        SessionTimeoutModalComponent,
        SessionTimeoutComponent,
        MenuSearchBarComponent,
        
    ],
    imports: [
        ngCommon.CommonModule,
        ReactiveFormsModule,
        FormsModule,
        AngularEditorModule,
        HttpClientModule,
        HttpClientJsonpModule,
        ModalModule.forRoot(),
        TooltipModule.forRoot(),
        TabsModule.forRoot(),
        BsDropdownModule.forRoot(),
        PopoverModule.forRoot(),
        BsDatepickerModule.forRoot(),
        FileUploadModule,
        AbpModule,
        AppRoutingModule,
        UtilsModule,
        AppCommonModule.forRoot(),
        ServiceProxyModule,
        TableModule,
        PaginatorModule,
        PrimeNgFileUploadModule,
        ProgressBarModule,
        PerfectScrollbarModule,
        CoreModule,
        NgxChartsModule,
        TextMaskModule,
        ImageCropperModule,
        AutoCompleteModule,
        NgxSpinnerModule,
        ServiceWorkerModule.register('ngsw-worker.js', { enabled: environment.production }),
        
       
    ],
    providers: [
        ImpersonationService,
        LinkedAccountService,
        UserNotificationHelper,
        ChatSignalrService,
        {
            provide: PERFECT_SCROLLBAR_CONFIG,
            useValue: DEFAULT_PERFECT_SCROLLBAR_CONFIG
        }
    ],
    entryComponents: [NgxSpinnerComponent]
})
export class AppModule {

}
