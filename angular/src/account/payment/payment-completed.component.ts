import { Component, Injector } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { AppComponentBase } from '@shared/common/app-component-base';
import { PaymentServiceProxy, StripePaymentServiceProxy, SubscriptionPaymentStatus } from '@shared/service-proxies/service-proxies';
import { AbpSessionService } from 'abp-ng2-module/dist/src/session/abp-session.service';

@Component({
  selector: 'payment-completed',
  templateUrl: './payment-completed.component.html'
})
export class PaymentCompletedComponent extends AppComponentBase {
  sessionId: string;
  paymentId: number;
  paymentResult :boolean;
  controlTimeout = 1000 * 5;
  maxControlCount = 5;
  constructor(
    _injector: Injector,
    private _paymentService: PaymentServiceProxy,
    private _stripePaymentService: StripePaymentServiceProxy,
    private _activatedRoute: ActivatedRoute,
    private _router: Router,
    private _sessionService: AbpSessionService) {
    super(_injector);
  }
  ngOnInit() {
    let searchParams = new URLSearchParams(window.location.search)
    this.sessionId = searchParams.get('sessionId');
    this._stripePaymentService.getPayment(this.sessionId)
      .subscribe(payment => {
        if (this._sessionService.tenantId !== payment.tenantId) {

          this._router.navigate(['']);
        }
        this.paymentResult = false;
        this.paymentId = payment.id;
        this.getPaymentResult();
      });
  }
  getPaymentResult(): void {
    this._paymentService.updatePaymentStatus(this.paymentId,SubscriptionPaymentStatus.Completed).subscribe(
      paymentResult => {
        if (paymentResult) {
          this.paymentResult = true;
          abp.multiTenancy.setTenantIdCookie(null);
        } else {
          this.controlAgain();
        }
      },
      err => {
        this.controlAgain();
      }
    );
  }

  controlAgain() {
    if (this.maxControlCount === 0) {
      return;
    }

    setTimeout(this.getPaymentResult, this.controlTimeout);
    this.controlTimeout *= 2;
    this.maxControlCount--;
  }
}
