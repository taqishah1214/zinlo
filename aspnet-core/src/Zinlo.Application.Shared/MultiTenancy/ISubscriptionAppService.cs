using System.Threading.Tasks;
using Abp.Application.Services;

namespace Zinlo.MultiTenancy
{
    public interface ISubscriptionAppService : IApplicationService
    {
        Task DisableRecurringPayments();

        Task EnableRecurringPayments();
    }
}
