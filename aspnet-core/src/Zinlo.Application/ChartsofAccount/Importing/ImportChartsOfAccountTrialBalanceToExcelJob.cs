using System.Linq;
using Abp.BackgroundJobs;
using Abp.Dependency;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Localization;
using Abp.Localization.Sources;
using Abp.ObjectMapping;
using Abp.Threading;
using Abp.UI;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Zinlo.Authorization.Users;
using Zinlo.ChartsofAccount.Dtos;
using Zinlo.Notifications;
using Zinlo.Storage;
namespace Zinlo.ChartsofAccount.Importing
{
    public class ImportChartsOfAccountTrialBalanceToExcelJob : BackgroundJob<ImportChartsOfAccountTrialBalanceFromExcelJobArgs>, ITransientDependency
    {
         private readonly IChartsOfAccontTrialBalanceListExcelDataReader _chartsOfAccontTrialBalanceListExcelDataReader;
         private readonly IChartsofAccountAppService _chartsofAccountAppService;
        private readonly IAppNotifier _appNotifier;
        private readonly IBinaryObjectManager _binaryObjectManager;
        private readonly ILocalizationSource _localizationSource;
        private readonly IObjectMapper _objectMapper;
        public UserManager userManager { get; set; }

        public ImportChartsOfAccountTrialBalanceToExcelJob(

        IChartsOfAccontTrialBalanceListExcelDataReader chartsOfAccontTrialBalanceListExcelDataReader,
        IChartsofAccountAppService chartsofAccountAppService,
        IAppNotifier appNotifier,
            IBinaryObjectManager binaryObjectManager,
            ILocalizationManager localizationManager,
            IObjectMapper objectMapper

            )
        {
            _chartsOfAccontTrialBalanceListExcelDataReader = chartsOfAccontTrialBalanceListExcelDataReader;
            _appNotifier = appNotifier;
            _binaryObjectManager = binaryObjectManager;
            _objectMapper = objectMapper;
            _localizationSource = localizationManager.GetSource(ZinloConsts.LocalizationSourceName);
            _chartsofAccountAppService = chartsofAccountAppService;
        }

        [UnitOfWork]
        public override void Execute(ImportChartsOfAccountTrialBalanceFromExcelJobArgs args)
        {
            using (CurrentUnitOfWork.SetTenantId(args.TenantId))
            {
                var chartsofaccount = GetAccountsTrialBalanceListFromExcelOrNull(args);
                if (chartsofaccount == null || !chartsofaccount.Any())
                {
                    SendInvalidExcelNotification(args);
                    return;
                }

                CreateChartsOfAccountsTrialBalance(args, chartsofaccount);
            }
        }

        private List<ChartsOfAccountsTrialBalanceExcellImportDto> GetAccountsTrialBalanceListFromExcelOrNull(ImportChartsOfAccountTrialBalanceFromExcelJobArgs args)
        {
            try
            {
                var file = AsyncHelper.RunSync(() => _binaryObjectManager.GetOrNullAsync(args.BinaryObjectId));
                 return _chartsOfAccontTrialBalanceListExcelDataReader.GetAccountsTrialBalanceFromExcel(file.Bytes);
            }
            catch (Exception)
            {
                return null;
            }
        }

        private void CreateChartsOfAccountsTrialBalance(ImportChartsOfAccountTrialBalanceFromExcelJobArgs args, List<ChartsOfAccountsTrialBalanceExcellImportDto> accounts)
        {
            var invalidAccounts = new List<ChartsOfAccountsTrialBalanceExcellImportDto>();

            foreach (var account in accounts)
            {
                if (account.CanBeImported())
                {
                    try
                    {
                        AsyncHelper.RunSync(() => CreateChartsOfAccountTrialBalanceAsync(account));
                    }
                    catch (UserFriendlyException exception)
                    {
                        account.Exception = exception.Message;
                        invalidAccounts.Add(account);
                    }
                    catch (Exception exception)
                    {
                        account.Exception = exception.ToString();
                        invalidAccounts.Add(account);
                    }
                }
                else
                {
                    invalidAccounts.Add(account);
                }
            }
            foreach (var item in accounts)
            {
                AsyncHelper.RunSync(() => CreateChartsOfAccountTrialBalanceAsync(item));
            }
            AsyncHelper.RunSync(() => ProcessImportAccountsTrialBalanceResultAsync(args, invalidAccounts));
        }

        private async Task CreateChartsOfAccountTrialBalanceAsync(ChartsOfAccountsTrialBalanceExcellImportDto input)
        {
            var tenantId = CurrentUnitOfWork.GetTenantId();
            ChartsofAccount account = new ChartsofAccount();
            account.TenantId = (int)tenantId;
         var result =   await _chartsofAccountAppService.CheckAccountForTrialBalance(input.AccountName, input.AccountNumber,input.Balance);
        }

        private async Task ProcessImportAccountsTrialBalanceResultAsync(ImportChartsOfAccountTrialBalanceFromExcelJobArgs args, List<ChartsOfAccountsTrialBalanceExcellImportDto> invalidAccounts)
        {
            await _appNotifier.SendMessageAsync(
                   args.User,
                   _localizationSource.GetString("AllAccountsTrialBalanceSuccessfullyImportedFromExcel"),
                   Abp.Notifications.NotificationSeverity.Success);         
        }

        private void SendInvalidExcelNotification(ImportChartsOfAccountTrialBalanceFromExcelJobArgs args)
        {
            AsyncHelper.RunSync(() => _appNotifier.SendMessageAsync(
                args.User,
                _localizationSource.GetString("FileCantBeConvertedToAccountsTrialBalanceList"),
                Abp.Notifications.NotificationSeverity.Warn));
        }
        #region|Helpers|
        public async Task<long> GetUserIdByEmail(string emailAddress)
        {
            var user = await userManager.FindByEmailAsync(emailAddress);
            if (user != null)
            {
                return user.Id;
            }
            else
            {
                return 3;
            }

        }
        #endregion
    }
}
