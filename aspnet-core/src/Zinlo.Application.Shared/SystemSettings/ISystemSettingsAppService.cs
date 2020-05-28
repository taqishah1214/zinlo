using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Zinlo.SystemSettings.Dtos;

namespace Zinlo.SystemSettings
{
    public interface ISystemSettingsAppService
    {
        Task SetDefaultMonth(CreateOrEditDefaultMonthDto Input);
        Task<CreateOrEditDefaultMonthDto> GetDefaultMonth();
    }
}
