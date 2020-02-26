using Abp.Application.Services;
using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Zinlo.ChartsofAccount.Dtos;

namespace Zinlo.ChartsofAccount
{
   public interface IChartsofAccountAppService : IApplicationService
    {
        Task CreateOrEdit(CreateOrEditChartsofAccountDto input);
        Task Delete(long id);
        Task<GetAccountForEditDto> GetAccountForEdit(long id);
        Task<PagedResultDto<ChartsofAccoutsForViewDto>> GetAll(GetAllChartsofAccountInput input);
        Task ChangeAccountsAssignee(long accountId, long assigneeId);

    }
}
