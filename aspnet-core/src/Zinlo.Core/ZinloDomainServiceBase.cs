using Abp.Domain.Services;

namespace Zinlo
{
    public abstract class ZinloDomainServiceBase : DomainService
    {
        /* Add your common members for all your domain services. */

        protected ZinloDomainServiceBase()
        {
            LocalizationSourceName = ZinloConsts.LocalizationSourceName;
        }
    }
}
