using System;
using System.Collections.Generic;
using System.Text;
using Abp.BackgroundJobs;
using Abp.Dependency;
using Abp.Domain.Uow;
using Abp.Runtime.Session;
using Abp.Threading;
using Zinlo.ClosingChecklist;
using Zinlo.TimeManagements.Dto;

namespace Zinlo.TimeManagements
{
    public class OpenMonthJob : BackgroundJob<OpenMonthArgs>, ITransientDependency
    {
        private readonly IClosingChecklistAppService _checklistService;
        private readonly IAbpSession _session;
        public OpenMonthJob(IClosingChecklistAppService checklistService, IAbpSession session)
        {
            _checklistService = checklistService;
            _session = session;
        }
        [UnitOfWork]
        public override void Execute(OpenMonthArgs args)
        {
            using (_session.Use(args.UserIdentifier.TenantId,args.UserIdentifier.UserId))
            {
                var last13MonthTaskByManagement = AsyncHelper.RunSync(() => _checklistService.GetTaskTimeDuration(args.Month));
                foreach (var task in last13MonthTaskByManagement)
                {
                    task.ClosingMonth = task.ClosingMonth.AddDays(-1);
                    AsyncHelper.RunSync(() => _checklistService.TaskIteration(task, args.Month, false));
                }
            }
            
        }
    }
}
