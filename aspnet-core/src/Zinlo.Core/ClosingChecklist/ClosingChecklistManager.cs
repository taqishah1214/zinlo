﻿using System;
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
            var entity = new ClosingChecklist();
            entity = input;
            var id= await _closingChecklistRepository.InsertAndGetIdAsync(input);
            return id;
        }

        public IQueryable<ClosingChecklist> GetAll()
        {
            return _closingChecklistRepository.GetAll();
        }

        public async Task UpdateVersionIds(long? versionId, Guid groupId)
        {
            var taskList = await _closingChecklistRepository.GetAllListAsync(p => p.GroupId == groupId);
            foreach (var task in taskList)
            {
                task.VersionId = versionId;
                await UpdateAsync(task);
            }
        }
        public async Task UpdateTask(long? versionId, Guid groupId)
        {
            var taskList = await _closingChecklistRepository.GetAllListAsync(p => p.GroupId == groupId);
            foreach (var task in taskList)
            {
                task.VersionId = versionId;
                await UpdateAsync(task);
            }
        }
        public async Task UpdateAsync(ClosingChecklist entity)
        {
            await _closingChecklistRepository.UpdateAsync(entity);
        }

        public async Task DeleteTask(ClosingChecklist entity)
        {
            await _closingChecklistRepository.DeleteAsync(entity);
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
            return await _closingChecklistRepository.GetAll().Include(p => p.Version).Include(a => a.Assignee)
                .Include(a => a.Category).FirstOrDefaultAsync(x => x.Id == id);

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


        public int GetTotalDaysByMonth(DateTime dateTime)
        {
            return DateTime.DaysInMonth(dateTime.Year, dateTime.Month);
        }

        public DateTime GetDueDate(DaysBeforeAfter daysBeforeAfter, DateTime closingMonth, int numberOfDays,
            bool endsOfMonth)
        {
            var nextDate = closingMonth;
            if (endsOfMonth)
            {
                nextDate = new DateTime(closingMonth.Year, closingMonth.Month, (GetTotalDaysByMonth(closingMonth)));
            }
            else if (daysBeforeAfter == DaysBeforeAfter.DaysBefore)
            {
                nextDate = new DateTime(closingMonth.Year, closingMonth.Month,
                    (GetTotalDaysByMonth(closingMonth) - numberOfDays));
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

        public bool CheckTaskExist(DateTime closingMonth, Guid? groupId)
        {
            if (groupId == null) return false;
                var ifTaskExist = _closingChecklistRepository.FirstOrDefault(p =>
               p.GroupId == groupId &&
               p.ClosingMonth.Year == closingMonth.Year
               && p.ClosingMonth.Month == closingMonth.Month);
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

    }
}
