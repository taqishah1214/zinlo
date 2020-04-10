using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Zinlo.InstructionVersions.Dto;

namespace Zinlo.InstructionVersions
{
    public class InstructionAppService : ZinloAppServiceBase, IInstructionAppService
    {
        private readonly IRepository<InstructionVersion, long> _versionRepository;
        public InstructionAppService(IRepository<InstructionVersion, long> versionRepository)
        {
            _versionRepository = versionRepository;
        }

        public async Task<long> CreateOrEdit(CreateOrEditInstructionVersion input)
        {
            if (input.Id == 0) return await Create(input);
            return await Update(input);
        }

        public async Task<bool> Comparison(long? id, string instruction)
        {
            if (id != null)
            {
                var version =
                    await _versionRepository.FirstOrDefaultAsync(p =>
                        p.Id == id);
                if (instruction.Equals(version.Body))
                {
                    return true;
                }
                
            }
            return false;

        }

        protected virtual async Task<long> Create(CreateOrEditInstructionVersion input)
        {
            var version = ObjectMapper.Map<InstructionVersion>(input);

            return await _versionRepository.InsertAndGetIdAsync(version);

        }

        protected virtual async Task<long> Update(CreateOrEditInstructionVersion input)
        {
            if (input.Id == 0) return input.Id;
            var version = await _versionRepository.FirstOrDefaultAsync((int)input.Id);
            ObjectMapper.Map(input, version);

            return input.Id;
        }

    }
}
