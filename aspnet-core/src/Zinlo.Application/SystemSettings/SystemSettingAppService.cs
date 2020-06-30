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
                    obj.id = 0;
                    return obj;
                }
                else
                {
                    obj.id = result[0].Id;
                    obj.Month = result[0].Month;
                    return obj;
                }
            }
            else
            {
                obj.id = 0;
                return obj;
            }
           
        }

        public async Task SetDefaultMonth(CreateOrEditDefaultMonthDto input)
        {
            input.Month = input.Month.AddDays(1);
            if (input.id != 0)
            {
               var result =  await _systemSettingsRepositry.FirstOrDefaultAsync(p => p.TenantId == (int)AbpSession.TenantId && p.Id == input.id && p.SettingType == SettingType.DefaultMonth);
                result.Month = input.Month;
               await _systemSettingsRepositry.UpdateAsync(result);


            }
            else
            {
                SystemSettings systemSettings = new SystemSettings();
                systemSettings.TenantId = (int)AbpSession.TenantId;
                systemSettings.Month = input.Month;
                systemSettings.SettingType = SettingType.DefaultMonth;
                await _systemSettingsRepositry.InsertAsync(systemSettings);
            }
           


        }
    }
}
