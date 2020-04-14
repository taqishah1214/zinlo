using System.Threading.Tasks;
using Abp.Application.Services;
using Zinlo.InstructionVersions.Dto;

namespace Zinlo.InstructionVersions
{
    public interface IInstructionAppService : IApplicationService
    {
        Task<long> CreateOrEdit(CreateOrEditInstructionVersion input);
        Task<bool> Comparison(long? id,string instruction);
        Task<GetInstruction> GetInstruction(long id);
    }
}
