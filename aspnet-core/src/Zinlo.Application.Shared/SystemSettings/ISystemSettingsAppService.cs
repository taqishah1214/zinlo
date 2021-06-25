using Abp.Application.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Zinlo.SystemSettings.Dtos;

namespace Zinlo.SystemSettings
{
    public interface ISystemSettingsAppService : IApplicationService
    {
        Task SetDefaultMonth(CreateOrEditDefaultMonthDto Input);
        Task<CreateOrEditDefaultMonthDto> GetDefaultMonth();
        Task<string> Get();
    }
}
