using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Zinlo.Reconciliation.Dtos;

namespace Zinlo.Reconciliation
{
   public interface IItemizationAppService
    {
        Task CreateOrEdit(CreateOrEditItemizationDto input);
        Task<PagedResultDto<ItemizedListDto>> GetAll(GetAllItemizationInput input);
        Task<CreateOrEditItemizationDto> GetEdit(long Id);
    }
}
