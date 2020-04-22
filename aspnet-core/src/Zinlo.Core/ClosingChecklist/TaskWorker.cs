using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using Abp.Dependency;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Threading.BackgroundWorkers;
using Abp.Threading.Timers;

namespace Zinlo.ClosingChecklist
{
    public class TaskWorker : PeriodicBackgroundWorkerBase, ISingletonDependency
    {
        private readonly IRepository<ClosingChecklist, long> _closingChecklistRepository;
        public TaskWorker(AbpTimer timer,
                          IRepository<ClosingChecklist, long> closingChecklistRepository) : base(timer)
        {
            Timer.Period = 60000;
            _closingChecklistRepository = closingChecklistRepository;
        }

        public int GetLastDateofMonth()
        {
            DateTime now = DateTime.Now;
            var lastDate = DateTime.DaysInMonth(now.Year, now.Month);
            return lastDate;
        }

        public bool CheckLastHourOfMonth()
        {
            DateTime now = DateTime.Now;
            var currentDate = new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Second);
            int lastDate = GetLastDateofMonth();
            if (lastDate == currentDate.Day)
            {
                if (currentDate.Hour == 22 || currentDate.Hour == 23)
                {
                    return true;
                }

                return false;

            }

            return false;
        }
        [UnitOfWork]
        protected override void DoWork()
        { /*if (!CheckLastHourOfMonth()) return;*/
            using (CurrentUnitOfWork.DisableFilter(AbpDataFilters.MayHaveTenant))
            {
                var allTasks = _closingChecklistRepository.GetAll().ToList();
                foreach (var task in allTasks)
                {
                    bool taskExist;
                    switch (task.Frequency)
                    {
                        case Frequency.Annually:
                            task.ClosingMonth = task.ClosingMonth.AddYears(1);
                            task.DueDate = GetDueDate(task.DayBeforeAfter, task.ClosingMonth,
                                task.DueOn, task.EndOfMonth);
                            taskExist = TaskExist(task.ClosingMonth, task.GroupId);
                            if (!taskExist)
                            {
                                var newTask = new ClosingChecklist()
                                {
                                    ClosingMonth = task.ClosingMonth,
                                    DueDate = task.DueDate,
                                    AssigneeId = task.AssigneeId,
                                    Frequency = task.Frequency,
                                    Status = task.Status,
                                    VersionId = task.VersionId,
                                    CategoryId = task.CategoryId,
                                    CreatorUserId = task.CreatorUserId,
                                    DayBeforeAfter = task.DayBeforeAfter,
                                    DueOn = task.DueOn,
                                    EndOfMonth = task.EndOfMonth,
                                    TaskName = task.TaskName,
                                    TenantId = task.TenantId,
                                    GroupId = task.GroupId,
                                    NoOfMonths = task.NoOfMonths,
                                };
                                _closingChecklistRepository.Insert(newTask);
                            }

                            break;
                        case Frequency.Monthly:

                            task.ClosingMonth = task.ClosingMonth.AddMonths(1);
                            task.DueDate = GetDueDate(task.DayBeforeAfter, task.ClosingMonth,
                                task.DueOn, task.EndOfMonth);
                            taskExist = TaskExist(task.ClosingMonth, task.GroupId);
                            if (!taskExist)
                            {
                                var newTask = new ClosingChecklist()
                                {
                                    ClosingMonth = task.ClosingMonth,
                                    DueDate = task.DueDate,
                                    AssigneeId = task.AssigneeId,
                                    Frequency = task.Frequency,
                                    Status = task.Status,
                                    VersionId = task.VersionId,
                                    CategoryId = task.CategoryId,
                                    CreatorUserId = task.CreatorUserId,
                                    DayBeforeAfter = task.DayBeforeAfter,
                                    DueOn = task.DueOn,
                                    EndOfMonth = task.EndOfMonth,
                                    TaskName = task.TaskName,
                                    TenantId = task.TenantId,
                                    GroupId = task.GroupId,
                                    NoOfMonths = task.NoOfMonths,
                                };
                                _closingChecklistRepository.Insert(newTask);
                            }

                            break;
                        case Frequency.Quarterly:

                            task.ClosingMonth = task.ClosingMonth.AddMonths(3);
                            task.DueDate = GetDueDate(task.DayBeforeAfter, task.ClosingMonth,
                                task.DueOn, task.EndOfMonth);
                            taskExist = TaskExist(task.ClosingMonth, task.GroupId);
                            if (!taskExist)
                            {
                                var newTask = new ClosingChecklist()
                                {
                                    ClosingMonth = task.ClosingMonth,
                                    DueDate = task.DueDate,
                                    AssigneeId = task.AssigneeId,
                                    Frequency = task.Frequency,
                                    Status = task.Status,
                                    VersionId = task.VersionId,
                                    CategoryId = task.CategoryId,
                                    CreatorUserId = task.CreatorUserId,
                                    DayBeforeAfter = task.DayBeforeAfter,
                                    DueOn = task.DueOn,
                                    EndOfMonth = task.EndOfMonth,
                                    TaskName = task.TaskName,
                                    TenantId = task.TenantId,
                                    GroupId = task.GroupId,
                                    NoOfMonths = task.NoOfMonths,
                                };
                                _closingChecklistRepository.Insert(newTask);
                            }

                            break;
                    }
                }
                CurrentUnitOfWork.SaveChanges();
            }
        }
        protected DateTime GetDueDate(DaysBeforeAfter daysBeforeAfter, DateTime closingMonth, int numberOfDays, bool endsOfMonth)
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
        protected int GetTotalDaysByMonth(DateTime dateTime)
        {
            return DateTime.DaysInMonth(dateTime.Year, dateTime.Month);
        }

        protected bool TaskExist(DateTime closingMonth, Guid groupId)
        {
            var taskExist = _closingChecklistRepository.FirstOrDefault(p => p.GroupId == groupId &&
                                                                            p.ClosingMonth.Year.Equals(closingMonth.Year) &&
                                                                            p.ClosingMonth.Month.Equals(closingMonth.Month));
            return taskExist != null;
        }
    }
}
