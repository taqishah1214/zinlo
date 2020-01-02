using System.Collections.Generic;
using System.Threading.Tasks;
using Abp;
using Zinlo.Dto;

namespace Zinlo.Gdpr
{
    public interface IUserCollectedDataProvider
    {
        Task<List<FileDto>> GetFiles(UserIdentifier user);
    }
}
