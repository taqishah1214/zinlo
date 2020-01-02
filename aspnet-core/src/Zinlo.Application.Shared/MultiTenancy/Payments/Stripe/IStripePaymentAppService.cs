using System.Threading.Tasks;
using Abp.Application.Services;
using Zinlo.MultiTenancy.Payments.Dto;
using Zinlo.MultiTenancy.Payments.Stripe.Dto;

namespace Zinlo.MultiTenancy.Payments.Stripe
{
    public interface IStripePaymentAppService : IApplicationService
    {
        Task ConfirmPayment(StripeConfirmPaymentInput input);

        StripeConfigurationDto GetConfiguration();

        Task<SubscriptionPaymentDto> GetPaymentAsync(StripeGetPaymentInput input);

        Task<string> CreatePaymentSession(StripeCreatePaymentSessionInput input);
    }
}