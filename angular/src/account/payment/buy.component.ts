import { Component, Injector, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { accountModuleAnimation } from '@shared/animations/routerTransition';
import { AppComponentBase } from '@shared/common/app-component-base';
import {
    EditionSelectDto,
    PaymentServiceProxy,
    TenantRegistrationServiceProxy,
    PaymentGatewayModel,
    CreatePaymentDto,
    EditionPaymentType,
    PaymentPeriodType,
    SubscriptionPaymentGatewayType,
    ResolveTenantIdInput,
    AccountServiceProxy,
    CustomTenantRequestLinkResolveInput
} from '@shared/service-proxies/service-proxies';
import { AppConsts } from '@shared/AppConsts';
import { PaymentHelperService } from './payment-helper.service';

@Component({
    templateUrl: './buy.component.html',
    animations: [accountModuleAnimation()]
})

export class BuyEditionComponent extends AppComponentBase implements OnInit {


    editionPaymentType: EditionPaymentType;
    edition: EditionSelectDto = new EditionSelectDto();
    tenantId: number = abp.session.tenantId;
    paymentPeriodType = PaymentPeriodType;
    selectedPaymentPeriodType: PaymentPeriodType;
    subscriptionPaymentGateway = SubscriptionPaymentGatewayType;
    paymentGateways: PaymentGatewayModel[];
    supportsRecurringPayments = false;
    recurringPaymentEnabled = false;
    editionId = 0;
  price = 0;

    constructor(
        injector: Injector,
        private _activatedRoute: ActivatedRoute,
        private _router: Router,
        private _accountService: AccountServiceProxy,
        private _paymnetHelperService: PaymentHelperService,
        private _paymentAppService: PaymentServiceProxy,
        private _tenantRegistrationService: TenantRegistrationServiceProxy) {
        super(injector);
    }

    ngOnInit():
        void {
        if (this._activatedRoute.snapshot.queryParams['c']) {
            var c = this._activatedRoute.snapshot.queryParams['c'];
            this._accountService.linkResolve(new CustomTenantRequestLinkResolveInput({ c: c })).subscribe((response) => {
                abp.multiTenancy.setTenantIdCookie(response.tenantId);
                this.editionPaymentType = response.editionPaymentType;
                this.editionId = response.editionId;
                this.price = response.price;
                this._tenantRegistrationService.getEdition(this.editionId)
                    .subscribe((result: EditionSelectDto) => {
                        this.edition = result;
                    });
                    this.selectedPaymentPeriodType = response.commitment;
                this._paymentAppService.getActiveGateways(undefined)
                    .subscribe((result: PaymentGatewayModel[]) => {
                        this.paymentGateways = result;
                        this.supportsRecurringPayments = result.some((pg) => pg.supportsRecurringPayments);
                    });


            });

        } else {


            let tenantId = parseInt(this._activatedRoute.snapshot.queryParams['tenantId']);
            abp.multiTenancy.setTenantIdCookie(tenantId);

            this.editionPaymentType = this._activatedRoute.snapshot.queryParams['editionPaymentType'];
            this.editionId = this._activatedRoute.snapshot.queryParams['editionId'];

            this._tenantRegistrationService.getEdition(this.editionId)
                .subscribe((result: EditionSelectDto) => {
                    this.edition = result;
                    this.selectedPaymentPeriodType = this._paymnetHelperService.getInitialSelectedPaymentPeriodType(this.edition);
                });

            this._paymentAppService.getActiveGateways(undefined)
                .subscribe((result: PaymentGatewayModel[]) => {
                    this.paymentGateways = result;
                    this.supportsRecurringPayments = result.some((pg) => pg.supportsRecurringPayments);
                });
        }
    }

    checkout(gatewayType) {
        let input = {} as CreatePaymentDto;
        input.editionId = this.editionId;
        input.editionPaymentType =0;// ((this.editionPaymentType) as any);
        input.paymentPeriodType = ((this.selectedPaymentPeriodType) as any);
        input.recurringPaymentEnabled = this.recurringPaymentEnabled;
        input.subscriptionPaymentGatewayType = gatewayType;
        input.price = this.price;
        input.successUrl = AppConsts.remoteServiceBaseUrl + '/api/services/app/payment/' + this._paymnetHelperService.getEditionPaymentType(this.editionPaymentType) + 'Succeed';
        input.errorUrl = AppConsts.remoteServiceBaseUrl + '/api/services/app/payment/PaymentFailed';

        this._paymentAppService.createPayment(input)
            .subscribe((paymentId: number) => {
                this._router.navigate(['account/' + this.getPaymentGatewayType(gatewayType).toLocaleLowerCase() + '-purchase'],
                    {
                        queryParams: {
                            paymentId: paymentId,
                            redirectUrl: 'account/register-tenant-result'
                        }
                    });
            });
    }

    getPaymentGatewayType(gatewayType): string {
        return this._paymnetHelperService.getPaymentGatewayType(gatewayType);
    }

    onPaymentPeriodChangeChange(selectedPaymentPeriodType) {
        this.selectedPaymentPeriodType = selectedPaymentPeriodType;
    }
}
