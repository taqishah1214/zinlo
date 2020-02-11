using Abp.Application.Services;
using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Zinlo.AccountSubType.Dtos;

namespace Zinlo.AccountSubType
{
    public interface IAccountSubTypeAppService : IApplicationService
    {
        Task CreateOrEdit(CreateOrEditAccountSubTypeDto input);
        Task Delete(long id);

        Task<PagedResultDto<GetAccountSubTypeForViewDto>> GetAll(GetAllAccountSubTypeInput input);

        Task<CreateOrEditAccountSubTypeDto> GetAccountSubTypeForEdit(long id);
    }
}
