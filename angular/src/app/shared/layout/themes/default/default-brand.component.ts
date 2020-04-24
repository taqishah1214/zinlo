import { Injector, Component, ViewEncapsulation, Inject } from '@angular/core';
import { AppConsts } from '@shared/AppConsts';
import { AppComponentBase } from '@shared/common/app-component-base';
import { DOCUMENT } from '@angular/common';
import { AccountServiceProxy } from '@shared/service-proxies/service-proxies';

@Component({
    templateUrl: './default-brand.component.html',
    selector: 'default-brand',
    encapsulation: ViewEncapsulation.None
})
export class DefaultBrandComponent extends AppComponentBase {

    defaultLogo = AppConsts.appBaseUrl + '/assets/common/images/app-logo-on-' + this.currentTheme.baseSettings.menu.asideSkin + '.svg';
    remoteServiceBaseUrl: string = AppConsts.remoteServiceBaseUrl;

    constructor(
        injector: Injector,
        private proxy:AccountServiceProxy,
        @Inject(DOCUMENT) private document: Document
    ) {
        super(injector);
    }

    toggleLeftAside(): void {
        this.document.body.classList.toggle('kt-aside--minimize');
    }
}
