using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Abp;
using Abp.BackgroundJobs;
using Abp.Dependency;
using Abp.Domain.Uow;
using Abp.Localization;
using Abp.Localization.Sources;
using Abp.Runtime.Session;
using Abp.Threading;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Zinlo.ClosingChecklist;
using Zinlo.Notifications;
using Zinlo.TimeManagements.Dto;

namespace Zinlo.TimeManagements
{
    public class OpenMonthJob : BackgroundJob<OpenMonthArgs>, ITransientDependency
    {
        private readonly IClosingChecklistAppService _checklistService;
        private readonly IAbpSession _session;
        private readonly IAppNotifier _appNotifier;
        private readonly ILocalizationSource _localizationSource;
        public OpenMonthJob(IClosingChecklistAppService checklistService,
                            IAbpSession session,
                            IAppNotifier appNotifier,
                            ILocalizationManager localizationManager)
        {
            _checklistService = checklistService;
            _session = session;
            _appNotifier = appNotifier;
            _localizationSource = localizationManager.GetSource(ZinloConsts.LocalizationSourceName); ;
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

                AsyncHelper.RunSync(() => _appNotifier.SendMessageAsync(
                    args.UserIdentifier,
                    _localizationSource.GetString("MonthOpenedMessage",args.Month),
                    Abp.Notifications.NotificationSeverity.Success));
            }
            
        }
    }
}
