using System.Threading.Tasks;
using Abp.Application.Services;
using Zinlo.Sessions.Dto;

namespace Zinlo.Sessions
{
    public interface ISessionAppService : IApplicationService
    {
        Task<GetCurrentLoginInformationsOutput> GetCurrentLoginInformations();

        Task<UpdateUserSignInTokenOutput> UpdateUserSignInToken();
    }
}
