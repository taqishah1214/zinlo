using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using Abp.Linq.Extensions;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Zinlo.TimeManagements.Dto;
using Abp.Application.Services.Dto;
using Zinlo.Authorization;
using Abp.Authorization;
using Abp.UI;
using Microsoft.EntityFrameworkCore;

namespace Zinlo.TimeManagements
{
    [AbpAuthorize(AppPermissions.Pages_Administration_TimeManagements)]
    public class TimeManagementsAppService : ZinloAppServiceBase, ITimeManagementsAppService
    {
        private readonly IRepository<TimeManagement, long> _timeManagementRepository;


        public TimeManagementsAppService(IRepository<TimeManagement, long> timeManagementRepository)
        {
            _timeManagementRepository = timeManagementRepository;

        }

        public async Task<PagedResultDto<GetTimeManagementForViewDto>> GetAll(GetAllTimeManagementsInput input)
        {

            var filteredTimeManagements = _timeManagementRepository.GetAll()
                        .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false);

            var pagedAndFilteredTimeManagements = filteredTimeManagements
                .OrderBy(input.Sorting ?? "id asc")
                .PageBy(input);

            var timeManagements = from o in pagedAndFilteredTimeManagements
                                  select new GetTimeManagementForViewDto()
                                  {
                                      TimeManagement = new TimeManagementDto
                                      {
                                          Month = o.Month,
                                          Status = o.Status,
                                          Id = o.Id
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
            var timeManagement = await _timeManagementRepository.FirstOrDefaultAsync(input.Id);

            var output = new GetTimeManagementForEditOutput { TimeManagement = ObjectMapper.Map<CreateOrEditTimeManagementDto>(timeManagement) };

            return output;
        }

        public async Task CreateOrEdit(CreateOrEditTimeManagementDto input)
        {
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
            var geTimeManagement = await _timeManagementRepository.FirstOrDefaultAsync(p => p.Month.Month == month.Month && p.Month.Year == month.Year);
            if (geTimeManagement!=null)
            {
                return true;
            }

            return false;
        }

        [AbpAuthorize(AppPermissions.Pages_Administration_TimeManagements_Create)]
        protected virtual async Task Create(CreateOrEditTimeManagementDto input)
        {
            var timeManagement = ObjectMapper.Map<TimeManagement>(input);


            if (AbpSession.TenantId != null)
            {
                timeManagement.TenantId = (int)AbpSession.TenantId;
            }


            await _timeManagementRepository.InsertAsync(timeManagement);
        }

        [AbpAuthorize(AppPermissions.Pages_Administration_TimeManagements_Edit)]
        protected virtual async Task Update(CreateOrEditTimeManagementDto input)
        {
            var timeManagement = await GetManagement((long)input.Id);
            ObjectMapper.Map(input, timeManagement);
        }

        [AbpAuthorize(AppPermissions.Pages_Administration_TimeManagements_Delete)]
        public async Task Delete(EntityDto<long> input)
        {
            await _timeManagementRepository.DeleteAsync(input.Id);
        }
        [AbpAuthorize(AppPermissions.Pages_Administration_TimeManagements_Status)]
        public async Task ChangeStatus(long id)
        {
            var timeManagement = await GetManagement(id);

            timeManagement.Status = !timeManagement.Status;
            await _timeManagementRepository.UpdateAsync(timeManagement);
        }

        public async Task<bool> GetMonthStatus(DateTime dateTime)
        {
            var management = await _timeManagementRepository.FirstOrDefaultAsync(p =>
                p.Month.Month.Equals(dateTime.Month) && p.Month.Year.Equals(dateTime.Year));
            return management.Status;
        }

        protected virtual async Task<TimeManagement> GetManagement(long id)
        {
            return await _timeManagementRepository.FirstOrDefaultAsync(id);

        }

    }
}