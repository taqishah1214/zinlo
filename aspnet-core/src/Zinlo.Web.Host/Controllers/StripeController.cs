using Zinlo.MultiTenancy.Payments.Stripe;

namespace Zinlo.Web.Controllers
{
    public class StripeController : StripeControllerBase
    {
        public StripeController(
            StripeGatewayManager stripeGatewayManager,
            StripePaymentGatewayConfiguration stripeConfiguration,
            IStripePaymentAppService stripePaymentAppService) 
            : base(stripeGatewayManager, stripeConfiguration, stripePaymentAppService)
        {
        }
    }
}
