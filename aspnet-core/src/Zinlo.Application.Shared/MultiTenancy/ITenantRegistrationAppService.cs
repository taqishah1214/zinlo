using System.Threading.Tasks;
using Abp.Application.Services;
using Zinlo.Editions.Dto;
using Zinlo.MultiTenancy.Dto;

namespace Zinlo.MultiTenancy
{
    public interface ITenantRegistrationAppService: IApplicationService
    {
        Task<RegisterTenantOutput> RegisterTenant(RegisterTenantInput input);

        Task<EditionsSelectOutput> GetEditionsForSelect();

        Task<EditionSelectDto> GetEdition(int editionId);
    }
}