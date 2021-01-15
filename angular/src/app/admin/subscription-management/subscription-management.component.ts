import { Component, Injector, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { AppComponentBase } from '@shared/common/app-component-base';
import {
    ApplicationInfoDto,
    InvoiceServiceProxy,
    PaymentServiceProxy,
    SessionServiceProxy,
    SubscriptionServiceProxy,
    TenantLoginInfoDto,
    UserLoginInfoDto,
    CreateInvoiceDto,
    EditionPaymentType,
    SubscriptionStartType,
    SubscriptionPaymentType,
    TenantRegistrationServiceProxy,
    EditionsSelectOutput,
    EditionSelectDto,
    SubscriptionPaymentListDto
} from '@shared/service-proxies/service-proxies';

import { LazyLoadEvent } from 'primeng/components/common/lazyloadevent';
import { Paginator } from 'primeng/components/paginator/paginator';
import { Table } from 'primeng/components/table/table';
import { finalize } from 'rxjs/operators';
import { AppAuthService } from '@app/shared/common/auth/app-auth.service';

@Component({
    templateUrl: './subscription-management.component.html',
    animations: [appModuleAnimation()],
    styleUrls: ['./subscription.component.css'],
})

export class SubscriptionManagementComponent extends AppComponentBase implements OnInit {

    @ViewChild('dataTable', {static: true}) dataTable: Table;
    @ViewChild('paginator', {static: true}) paginator: Paginator;

    subscriptionStartType: typeof SubscriptionStartType = SubscriptionStartType;
    subscriptionPaymentType: typeof SubscriptionPaymentType = SubscriptionPaymentType;
    lastPayment : SubscriptionPaymentListDto =new SubscriptionPaymentListDto();
    loading: boolean;
    user: UserLoginInfoDto = new UserLoginInfoDto();
    tenant: TenantLoginInfoDto = new TenantLoginInfoDto();
    application: ApplicationInfoDto = new ApplicationInfoDto();
    editionPaymentType: typeof EditionPaymentType = EditionPaymentType;
    editionsSelectOutput: EditionsSelectOutput = new EditionsSelectOutput();

    filterText = '';

    constructor(
        injector: Injector,
        private _sessionService: SessionServiceProxy,
        private _paymentServiceProxy: PaymentServiceProxy,
        private _tenantRegistrationService: TenantRegistrationServiceProxy,
        private _invoiceServiceProxy: InvoiceServiceProxy,
        private _subscriptionServiceProxy: SubscriptionServiceProxy,
        private _activatedRoute: ActivatedRoute,
        private _router: Router,
        private _authService: AppAuthService,
    ) {
        super(injector);
        this.filterText = this._activatedRoute.snapshot.queryParams['filterText'] || '';
    }

    ngOnInit(): void {
        this.getSettings();
        this._tenantRegistrationService.getEditionsForSelect('')
            .subscribe((result) => {
                this.editionsSelectOutput = result;
            });
    }

    createOrShowInvoice(paymentId: number, invoiceNo: string): void {
        if (invoiceNo) {
            window.open('/app/admin/invoice/' + paymentId, '_blank');
        } else {
            this._invoiceServiceProxy.createInvoice(new CreateInvoiceDto({ subscriptionPaymentId: paymentId })).subscribe(() => {
                this.getPaymentHistory();
                window.open('/app/admin/invoice/' + paymentId, '_blank');
            });
        }
    }

    getSettings(): void {
        this.loading = true;
        this.appSession.init().then(() => {
            this.loading = false;
            this.user = this.appSession.user;
            this.tenant = this.appSession.tenant;
            this.application = this.appSession.application;
        });
    }

    CancelSubscription()
    {
        this._tenantRegistrationService.unRegisterTenant(this.tenant.id).subscribe(result => {
            
            //send to login
          this._authService.logout();

           });
    }

    getPaymentHistory(event?: LazyLoadEvent) {
        if (this.primengTableHelper.shouldResetPaging(event)) {
            this.paginator.changePage(0);

            return;
        }
        this.getLastPaymentHistory();

        this.primengTableHelper.showLoadingIndicator();

        this._paymentServiceProxy.getPaymentHistory(
            this.primengTableHelper.getSorting(this.dataTable),
            this.primengTableHelper.getMaxResultCount(this.paginator, event),
            this.primengTableHelper.getSkipCount(this.paginator, event)
        ).pipe(finalize(() => this.primengTableHelper.hideLoadingIndicator())).subscribe(result => {
            this.primengTableHelper.totalRecordsCount = result.totalCount;
            this.primengTableHelper.records = result.items;
            this.primengTableHelper.hideLoadingIndicator();
        });
    }
    getLastPaymentHistory()
    {
        this._paymentServiceProxy.getLastPaymentHistory().subscribe(result => {
         this.lastPayment=result;
        });
    }

    disableRecurringPayments() {
        this._subscriptionServiceProxy.disableRecurringPayments().subscribe(result => {
            this.tenant.subscriptionPaymentType = this.subscriptionPaymentType.RecurringManual;
        });
    }

    enableRecurringPayments() {
        this._subscriptionServiceProxy.enableRecurringPayments().subscribe(result => {
            this.tenant.subscriptionPaymentType = this.subscriptionPaymentType.RecurringAutomatic;
        });
    }

    hasRecurringSubscription(): boolean {
        return this.tenant.subscriptionPaymentType !== this.subscriptionPaymentType.Manual;
    }

    upgrade(upgradeEdition: EditionSelectDto, editionPaymentType: EditionPaymentType): void {
        this._router.navigate(['/account/upgrade'], { queryParams: { upgradeEditionId: upgradeEdition.id, editionPaymentType: editionPaymentType } });
    }
}
