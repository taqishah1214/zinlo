using System.Threading.Tasks;
using Abp.Application.Services;
using Zinlo.Versions.Dto;

namespace Zinlo.Versions
{
    public interface IVersionAppService : IApplicationService
    {
        Task CreateOrEdit(CreateOrEditVersion input);
        Task<GetVersion> GetActiveVersion(int type, long typeId);
        Task<bool> ActiveVersion(long id);
    }
}
