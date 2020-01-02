using System.Threading.Tasks;
using Abp.Application.Services;
using Zinlo.MultiTenancy.Payments.PayPal.Dto;

namespace Zinlo.MultiTenancy.Payments.PayPal
{
    public interface IPayPalPaymentAppService : IApplicationService
    {
        Task ConfirmPayment(long paymentId, string paypalOrderId);

        PayPalConfigurationDto GetConfiguration();
    }
}
