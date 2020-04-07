using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Zinlo.Versions.Dto;

namespace Zinlo.Versions
{
    public class VersionAppService : ZinloAppServiceBase, IVersionAppService

    {
        private readonly IRepository<Version, long> _versionRepository;
        public VersionAppService(IRepository<Version, long> versionRepository)
        {
            _versionRepository = versionRepository;
        }

        public async Task CreateOrEdit(CreateOrEditVersion input)
        {
            if (input.Id == 0)
            {
                await Create(input);
            }
            else
            {
                await Update(input);
            }
        }

        public async Task<GetVersion> GetActiveVersion(int type,long typeId)
        {
            var version =
                await _versionRepository.FirstOrDefaultAsync(p =>
                    p.Type == (Type) type && p.TypeId == typeId && p.Active);
            return ObjectMapper.Map<GetVersion>(version);
        }

        public async Task<bool> ActiveVersion(long id)
        {
            var getInActiveVersion = await _versionRepository.FirstOrDefaultAsync(p => p.Id == id);
            var getActiveVersion = GetActiveVersion((int)getInActiveVersion.Type,getInActiveVersion.TypeId);
            getInActiveVersion.Active = false;
            return true;



        }

        protected virtual async Task<long> Create(CreateOrEditVersion input)
        {
            var version = ObjectMapper.Map<Version>(input);

            if (AbpSession.TenantId != null)
            {
                version.TenantId = (int)AbpSession.TenantId;
            }
            var id = await _versionRepository.InsertAndGetIdAsync(version);
            return id;
        }

        protected virtual async Task<long> Update(CreateOrEditVersion input)
        {
            if (input.Id != 0)
            {
                var version = await _versionRepository.FirstOrDefaultAsync((int)input.Id);
                ObjectMapper.Map(input, version);
            }

            return input.Id;
        }

    }
}
