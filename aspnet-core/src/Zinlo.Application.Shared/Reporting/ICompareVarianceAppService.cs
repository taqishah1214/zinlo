using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Zinlo.Dto;
using Zinlo.Reporting.Dtos;

namespace Zinlo.Reporting
{
    public interface ICompareVarianceAppService
    {
        Task<PagedResultDto<CompareVarianceViewDto>> GetCompareTrialBalances(CompareVarianceInputDto input);
        Task<FileDto> GetInToExcel(List<CompareVarianceViewDto> input,DateTime FirstMonth , DateTime SecondMonth);
    }
}
