using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Zinlo.Reconciliation.Dtos;

namespace Zinlo.Reconciliation
{
  public  interface IAmortizationAppService
    {
        Task CreateOrEdit(CreateOrEditAmortizationDto input);
        Task Delete(long Id);
        Task<PagedResultDto<AmortizedListDto>> GetAll(GetAllAmortizationInput input);
        Task <CreateOrEditAmortizationDto> GetAmortizedItemDetails (long Id);



    }
}
