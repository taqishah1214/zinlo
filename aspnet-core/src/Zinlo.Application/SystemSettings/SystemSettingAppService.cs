using Abp.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zinlo.SystemSettings.Dtos;

namespace Zinlo.SystemSettings
{
    public class SystemSettingAppService : ZinloAppServiceBase, ISystemSettingsAppService
    {

        private readonly IRepository<SystemSettings, long> _systemSettingsRepositry;
       
        public SystemSettingAppService(IRepository<SystemSettings, long> systemSettingsRepositry)
        {
            _systemSettingsRepositry = systemSettingsRepositry;
        }
        public async Task<CreateOrEditDefaultMonthDto> GetDefaultMonth()
        {
            CreateOrEditDefaultMonthDto obj = new CreateOrEditDefaultMonthDto();
            var tenantId = CurrentUnitOfWork.GetTenantId();
            if (tenantId != 0)
            {
                var result = _systemSettingsRepositry.GetAll().Where(p => p.TenantId == tenantId && p.SettingType == SettingType.DefaultMonth).ToList();
                if (result.Count == 0)
                {
                    obj.Id = 0;
                    return obj;
                }
                else
                {
                    obj.Id = result[0].Id;
                    obj.Month = result[0].Month;
                    obj.IsWeekEndEnable = result[0].IsWeekEndEnable;
                    return obj;
                }
            }
            else
            {
                obj.Id = 0;
                return obj;
            }
           
        }

        public async Task SetDefaultMonth(CreateOrEditDefaultMonthDto input)
        {
            input.Month = input.Month.AddDays(1);
            if (input.Id != 0)
            {
               var result =  await _systemSettingsRepositry.FirstOrDefaultAsync(p => p.TenantId == (int)AbpSession.TenantId && p.Id == input.Id && p.SettingType == SettingType.DefaultMonth);
                result.Month = input.Month;
                result.IsWeekEndEnable = input.IsWeekEndEnable;
               await _systemSettingsRepositry.UpdateAsync(result);


            }
            else
            {
                SystemSettings systemSettings = new SystemSettings();
                systemSettings.TenantId = (int)AbpSession.TenantId;
                systemSettings.Month = input.Month;
                systemSettings.IsWeekEndEnable = input.IsWeekEndEnable;
                systemSettings.SettingType = SettingType.DefaultMonth;
                await _systemSettingsRepositry.InsertAsync(systemSettings);
            }
           


        }
    }
}
