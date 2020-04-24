using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using Abp.Linq.Extensions;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Zinlo.TimeManagements.Dto;
using Abp.Application.Services.Dto;
using Zinlo.Authorization;
using Abp.Authorization;
using Abp.Timing;
using Abp.UI;
using Microsoft.EntityFrameworkCore;
using Zinlo.ChartsofAccount;
using Zinlo.ClosingChecklist;
using NUglify.Helpers;

namespace Zinlo.TimeManagements
{
    [AbpAuthorize(AppPermissions.Pages_Administration_TimeManagements)]
    public class TimeManagementsAppService : ZinloAppServiceBase, ITimeManagementsAppService
    {
        private readonly TimeManagementManager _timeManagementManager;
        private readonly IClosingChecklistAppService _checklistService;


        public TimeManagementsAppService(TimeManagementManager timeManagementManager, IClosingChecklistAppService checklistAppService, IClosingChecklistAppService checklistService)
        {
            _timeManagementManager = timeManagementManager;
            _checklistService = checklistService;
        }

        public async Task<PagedResultDto<GetTimeManagementForViewDto>> GetAll(GetAllTimeManagementsInput input)
        {

            var filteredTimeManagements = _timeManagementManager.GetAll()
                        .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false);

            var pagedAndFilteredTimeManagements = filteredTimeManagements
                .OrderBy(input.Sorting ?? "month asc")
                .PageBy(input);

            var timeManagements = from o in pagedAndFilteredTimeManagements
                                  select new GetTimeManagementForViewDto()
                                  {
                                      TimeManagement = new TimeManagementDto
                                      {
                                          Month = o.Month,
                                          Status = o.Status,
                                          Id = o.Id,
                                          IsClosed = o.IsClosed
                                      }
                                  };

            var totalCount = await filteredTimeManagements.CountAsync();

            return new PagedResultDto<GetTimeManagementForViewDto>(
                totalCount,
                await timeManagements.ToListAsync()
            );
        }

        [AbpAuthorize(AppPermissions.Pages_Administration_TimeManagements_Edit)]
        public async Task<GetTimeManagementForEditOutput> GetTimeManagementForEdit(EntityDto<long> input)
        {
            var timeManagement = await _timeManagementManager.GetById(input.Id);

            var output = new GetTimeManagementForEditOutput { TimeManagement = ObjectMapper.Map<CreateOrEditTimeManagementDto>(timeManagement) };

            return output;
        }

        public async Task CreateOrEdit(CreateOrEditTimeManagementDto input)
        {
            input.Month = input.Month.AddDays(1);
            var checkMonth = await CheckMonth(input.Month);
            if (checkMonth) throw new UserFriendlyException(L("ThisMonthIsAlreadyDefine"));
            if (input.Id == null)
            {
                await Create(input);
            }
            else
            {
                await Update(input);
            }
        }

        protected virtual async Task<bool> CheckMonth(DateTime month)
        {
            return await _timeManagementManager.CheckMonth(month);

        }

        [AbpAuthorize(AppPermissions.Pages_Administration_TimeManagements_Create)]
        protected virtual async Task Create(CreateOrEditTimeManagementDto input)
        {
            var timeManagement = ObjectMapper.Map<TimeManagement>(input);

            if (AbpSession.TenantId != null)
            {
                timeManagement.TenantId = (int)AbpSession.TenantId;
            }
            await _timeManagementManager.CreateAsync(timeManagement);
        }

        [AbpAuthorize(AppPermissions.Pages_Administration_TimeManagements_Edit)]
        protected virtual async Task Update(CreateOrEditTimeManagementDto input)
        {
            var timeManagement = await GetManagement((long)input.Id);
            await _timeManagementManager.UpdateAsync(ObjectMapper.Map<TimeManagement>(input));
        }

        [AbpAuthorize(AppPermissions.Pages_Administration_TimeManagements_Delete)]
        public async Task Delete(EntityDto<long> input)
        {
            await _timeManagementManager.DeleteAsync(input.Id);
        }
        [AbpAuthorize(AppPermissions.Pages_Administration_TimeManagements_Status)]
        public async Task ChangeStatus(long id)
        {
            var timeManagement = await _timeManagementManager.GetManagement(id);
            if (!timeManagement.IsClosed && !timeManagement.Status)
            {
                var last13MonthTaskByManagement = await _checklistService.GetTaskTimeDuration(timeManagement.Month);
                foreach (var task in last13MonthTaskByManagement)
                {
                    task.ClosingMonth = task.ClosingMonth.AddDays(-1);
                    await _checklistService.TaskIteration(task, timeManagement.Month, false);
                }

            }
            if (timeManagement.Status)
            {
                timeManagement.IsClosed = true;
            }
            timeManagement.Status = !timeManagement.Status;
            await _timeManagementManager.UpdateAsync(timeManagement);
        }

        public async Task<bool> GetMonthStatus(DateTime dateTime)
        {
            var management = await _timeManagementManager.GetByDate(dateTime);
            if (management == null)
            {
                if (dateTime.Year.Equals(DateTime.Now.Year) && dateTime.Month.Equals(DateTime.Now.Month))
                {
                    var createManagement = new CreateOrEditTimeManagementDto()
                    {
                        Month = dateTime,
                        Status = false,
                    };
                    await Create(createManagement);
                }

                return false;
            }
            return management.Status;
        }

        public List<TimeManagementDto> GetOpenManagement()
        {
            var query = _timeManagementManager.GetOpenManagement();
            return ObjectMapper.Map<List<TimeManagementDto>>(query);

        }

        public async Task<bool> CheckManagementExist(DateTime dateTime)
        {
            return await CheckMonth(dateTime);
        }

        protected virtual async Task<TimeManagement> GetManagement(long id)
        {
            return await _timeManagementManager.GetById(id);

        }

    }
}