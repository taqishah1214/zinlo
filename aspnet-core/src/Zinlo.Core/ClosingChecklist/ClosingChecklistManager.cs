using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.Application.Services.Dto;
using Abp.Dependency;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Zinlo.Authorization.Users;
using Zinlo.TimeManagements;

namespace Zinlo.ClosingChecklist
{
    public class ClosingChecklistManager : ITransientDependency
    {

        private readonly IRepository<ClosingChecklist, long> _closingChecklistRepository;
        private readonly TimeManagementManager _managementManager;
        private readonly IUnitOfWorkManager _unitOfWorkManager;


        public ClosingChecklistManager(IRepository<ClosingChecklist, long> closingChecklistRepository,
                                          TimeManagementManager managementManager,
                                          IUnitOfWorkManager unitOfWorkManager)
        {
            _closingChecklistRepository = closingChecklistRepository;
            _managementManager = managementManager;
            _unitOfWorkManager = unitOfWorkManager;
        }


        public async Task DeleteAsync(long id)
        {
            await _closingChecklistRepository.DeleteAsync(id);
        }

        public async Task<long> InsertAndGetIdAsync(ClosingChecklist input)
        {
            return await _closingChecklistRepository.InsertAndGetIdAsync(input);
        }
         public IQueryable<ClosingChecklist> GetAll()
        {
            return _closingChecklistRepository.GetAll();
        }
        public int GetMonthDifference(DateTime start, DateTime end)
        {
            return ((end.Year * 12 + end.Month) - (start.Year * 12 + start.Month)) + 1;
        }
        public int GetDifferenceInYears(DateTime startDate, DateTime endDate)
        {
            int years = endDate.Year - startDate.Year;

            if (startDate.Month == endDate.Month && endDate.Day < startDate.Day
                || endDate.Month < startDate.Month)
            {
                years--;
            }

            return years;
        }

        public async Task<ClosingChecklist> GetDetailById(long id)
        {
            return await _closingChecklistRepository.GetAll().Include(p => p.Version).Include(a => a.Assignee).Include(a => a.Category).FirstOrDefaultAsync(x => x.Id == id);

        }
        public async Task<ClosingChecklist> GetById(long id)
        {
            return await _closingChecklistRepository.FirstOrDefaultAsync(x => x.Id == id);

        }
        public async Task Delete(long id)
        {
            await _closingChecklistRepository.DeleteAsync(id);
        }


        public async Task ChangeAssignee(long taskId, long assigneeId)
        {
            var task = await _closingChecklistRepository.FirstOrDefaultAsync(taskId);
            if (task != null)
            {
                task.AssigneeId = assigneeId;
                _closingChecklistRepository.Update(task);
            }
        }
        public async Task ChangeStatus(long taskId, long statusId)
        {
            var task = await _closingChecklistRepository.FirstOrDefaultAsync(taskId);
            if (task != null)
            {
                task.Status = (Status)statusId;
                _closingChecklistRepository.Update(task);
            }
        }
        public List<NameValueDto<string>> GetCurrentMonthDays(DateTime dateTime)
        {
            if (dateTime == DateTime.MinValue)
            {
                var list = new List<NameValueDto<string>>();
                var now = DateTime.Now;
                var startDate = new DateTime(now.Year, now.Month, 1);
                var endDate = startDate.AddMonths(1).AddDays(-1);
                for (var date = startDate; date <= endDate; date = date.AddDays(1))
                {
                    var nameValueDto = new NameValueDto<string>
                    {
                        Value = date.Day.ToString(),
                        Name = date.Day.ToString()
                    };
                    list.Add(nameValueDto);
                }
                return list;

            }
            else
            {
                var list = new List<NameValueDto<string>>();
                var now = dateTime; //  DateTime.Now;
                var startDate = new DateTime(now.Year, now.Month, 1);
                var endDate = startDate.AddMonths(1).AddDays(-1);
                for (var date = startDate; date <= endDate; date = date.AddDays(1))
                {
                    var nameValueDto = new NameValueDto<string>
                    {
                        Value = date.Day.ToString(),
                        Name = date.Day.ToString()
                    };
                    list.Add(nameValueDto);
                }
                return list;

            }


        }
        public string GenerateGroupId(long id, int iteration)
        {
            return id + "-" + iteration;
        }

        public int GetTotalDaysByMonth(DateTime dateTime)
        {
            return DateTime.DaysInMonth(dateTime.Year, dateTime.Month);
        }
        public DateTime GetDueDate(DaysBeforeAfter daysBeforeAfter, DateTime closingMonth, int numberOfDays, bool endsOfMonth)
        {
            var nextDate = closingMonth;
            if (endsOfMonth)
            {
                nextDate = new DateTime(closingMonth.Year, closingMonth.Month, (GetTotalDaysByMonth(closingMonth)));
            }
            else if (daysBeforeAfter == DaysBeforeAfter.DaysBefore)
            {
                nextDate = new DateTime(closingMonth.Year, closingMonth.Month, (GetTotalDaysByMonth(closingMonth) - numberOfDays));
            }
            else
            {
                nextDate = nextDate.AddDays(-nextDate.Day);
                var number = GetTotalDaysByMonth(closingMonth) + numberOfDays;
                nextDate = nextDate.AddDays(number);

            }
            return nextDate;

        }
        protected virtual async Task<bool> GetMonthStatus(DateTime dateTime)
        {
            return await _managementManager.GetMonthStatus(dateTime);
        }

        public async Task<bool> CheckTaskExist(DateTime closingMonth, Guid groupId)
        {
            var ifTaskExist = await _closingChecklistRepository.FirstOrDefaultAsync(p =>
                p.GroupId == groupId && p.ClosingMonth.Month == closingMonth.Month &&
                p.ClosingMonth.Year == closingMonth.Year);
            if (ifTaskExist != null) return true;
            return false;
        }
        public async Task RestoreTask(long id)
        {
            using (_unitOfWorkManager.Current.DisableFilter(AbpDataFilters.SoftDelete))
            {
                var task = await _closingChecklistRepository.FirstOrDefaultAsync(id);
                task.IsDeleted = false;
                await _closingChecklistRepository.UpdateAsync(task);
            }

        }

        public async Task RevertInstruction(long id, long instructionId)
        {
            var task = await _closingChecklistRepository.FirstOrDefaultAsync(id);
            task.VersionId = instructionId;
            await _closingChecklistRepository.UpdateAsync(task);
        }
        public async Task<long> TaskIteration(ClosingChecklist input,DateTime openingMonth)
        {
            Guid? oldGroupId = null;
            var forEdit = false;
            if (input.Id == 0)
            {
                input.ClosingMonth = input.ClosingMonth.AddDays(1);
                input.GroupId = Guid.NewGuid();
            }
            else
            {
                oldGroupId = input.GroupId;
                forEdit = true;
            }
            if (input.EndOfMonth) input.DueOn = 0;
            switch (input.Frequency)
            {
                case Frequency.None:
                    {
                        input.DueDate = GetDueDate(input.DayBeforeAfter, input.ClosingMonth, input.DueOn, input.EndOfMonth);

                        if (input.Id == 0 || (!await CheckTaskExist(input.ClosingMonth, (Guid)oldGroupId)))
                        {
                            if (forEdit) input.Id = 0;
                            return await InsertAndGetIdAsync(input);
                        }
                        break;
                    }
                case Frequency.Monthly:
                    {
                        var monthDifference = GetMonthDifference(input.ClosingMonth, openingMonth);
                        for (int i = 1; i <= monthDifference; i++)
                            input.DueDate = GetDueDate(input.DayBeforeAfter, input.ClosingMonth,
                            input.DueOn, input.EndOfMonth);
                        if (input.Id == 0 || (!await CheckTaskExist(input.ClosingMonth, (Guid)input.GroupId)))
                        {
                            if (forEdit) input.Id = 0;
                            return await InsertAndGetIdAsync(input);
                        }
                        break;
                    }
                case Frequency.Quarterly:
                    {
                        var monthDifference = GetMonthDifference(input.ClosingMonth, openingMonth);
                        for (int i = 1; i <= monthDifference; i += 3)
                        {
                            input.DueDate = GetDueDate(input.DayBeforeAfter, input.ClosingMonth,
                                input.DueOn, input.EndOfMonth);
                            if (input.Id == 0 || (!await CheckTaskExist(input.ClosingMonth, (Guid) input.GroupId)))
                            {
                                if (forEdit) input.Id = 0;
                                return await InsertAndGetIdAsync(input);
                            }
                        }

                        break;
                    }
                case Frequency.Annually:
                    {
                        var yearsDifference = GetDifferenceInYears(input.ClosingMonth, openingMonth);
                        for (int i = 0; i <= yearsDifference; i++)
                        {
                            input.DueDate = GetDueDate(input.DayBeforeAfter, input.ClosingMonth,
                                input.DueOn, input.EndOfMonth);
                            if (input.Id == 0 || (!await CheckTaskExist(input.ClosingMonth, (Guid) input.GroupId)))
                            {
                                if (forEdit) input.Id = 0;
                                return await InsertAndGetIdAsync(input);
                            }
                        }

                        break;
                    }
                case Frequency.XNumberOfMonths:
                    {

                        input.DueDate = GetDueDate(input.DayBeforeAfter, input.ClosingMonth,
                            input.DueOn, input.EndOfMonth);
                        if (input.Id == 0 || (!await CheckTaskExist(input.ClosingMonth, (Guid)input.GroupId)))
                        {
                            if (forEdit) input.Id = 0;
                            return await InsertAndGetIdAsync(input);
                        }
                        //var nextMonth = input.ClosingMonth.AddMonths(input.NoOfMonths);
                        //input.ClosingMonth = nextMonth;
                        break;
                    }
            }
            return 0;
        }
    }
}
