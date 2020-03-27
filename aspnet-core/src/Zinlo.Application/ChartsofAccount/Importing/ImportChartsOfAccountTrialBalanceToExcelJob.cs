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
using Zinlo.ImportPaths.Dto;
using Zinlo.ImportPaths;

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
        private readonly IInvalidAccountsTrialBalanceExporter _invalidAccountsTrialBalanceExporter;
        private readonly IImportPathsAppService _importPathsAppService;
        public long TenantId = 0;
        public long UserId = 0;
        public int SuccessRecordsCount = 0;
        public ImportChartsOfAccountTrialBalanceToExcelJob(

        IChartsOfAccontTrialBalanceListExcelDataReader chartsOfAccontTrialBalanceListExcelDataReader,
        IInvalidAccountsTrialBalanceExporter invalidAccountsTrialBalanceExporter,
        IChartsofAccountAppService chartsofAccountAppService,
        IAppNotifier appNotifier,
            IBinaryObjectManager binaryObjectManager,
            ILocalizationManager localizationManager,
            IObjectMapper objectMapper,
            IImportPathsAppService importPathsAppService

            )
        {
            _chartsOfAccontTrialBalanceListExcelDataReader = chartsOfAccontTrialBalanceListExcelDataReader;
            _appNotifier = appNotifier;
            _binaryObjectManager = binaryObjectManager;
            _objectMapper = objectMapper;
            _localizationSource = localizationManager.GetSource(ZinloConsts.LocalizationSourceName);
            _chartsofAccountAppService = chartsofAccountAppService;
            _invalidAccountsTrialBalanceExporter = invalidAccountsTrialBalanceExporter;
            _importPathsAppService = importPathsAppService;
        }

        [UnitOfWork]
        public override void Execute(ImportChartsOfAccountTrialBalanceFromExcelJobArgs args)
        {
            TenantId = (int)args.TenantId;
            UserId = args.User.UserId;
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
                var result = _chartsOfAccontTrialBalanceListExcelDataReader.GetAccountsTrialBalanceFromExcel(file.Bytes);
                if (result.Count > 0)
                {

                    long sum = result.Sum(x => Convert.ToInt64(x.Balance));
                    if (sum == 0)
                    {
                        return result;
                    }
                }
                return new List<ChartsOfAccountsTrialBalanceExcellImportDto>();
            }
            catch (Exception ex)
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
                        var result = CheckNullValues(account);
                        if (result == false)
                        {
                            AsyncHelper.RunSync(() => CreateChartsOfAccountTrialBalanceAsync(account));
                        }
                        else
                        {

                            account.Exception = _localizationSource.GetString("NullValuesAreNotAllowed");
                            invalidAccounts.Add(account);
                        }

                    }

                    catch (UserFriendlyException exception)
                    {
                        account.Exception = exception.Message;
                        invalidAccounts.Add(account);
                    }
                    //catch (Exception exception)
                    //{
                    //    account.Exception = exception.ToString();
                    //    invalidAccounts.Add(account);
                    //}
                }
                else
                {
                    invalidAccounts.Add(account);
                }
            }
            List<ChartsOfAccountsTrialBalanceExcellImportDto> ValidRows = accounts.Except(invalidAccounts).ToList();
            SuccessRecordsCount = ValidRows.Count;
            foreach (var item in ValidRows)
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
            var result = await _chartsofAccountAppService.CheckAccountForTrialBalance(input.AccountName, input.AccountNumber, input.Balance);
        }

        private async Task ProcessImportAccountsTrialBalanceResultAsync(ImportChartsOfAccountTrialBalanceFromExcelJobArgs args, List<ChartsOfAccountsTrialBalanceExcellImportDto> invalidAccounts)
        {
            await _appNotifier.SendMessageAsync(
                   args.User,
                   _localizationSource.GetString("AllAccountsTrialBalanceSuccessfullyImportedFromExcel"),
                   Abp.Notifications.NotificationSeverity.Success);
            if (invalidAccounts.Any())
            {
                var url = _invalidAccountsTrialBalanceExporter.ExportToFile(invalidAccounts);
                ImportPathDto pathDto = new ImportPathDto();
                pathDto.FilePath = url;
                pathDto.Type = FileTypes.TrialBalance.ToString();
                pathDto.TenantId = (int)TenantId;
                pathDto.CreatorId = UserId;
                pathDto.FailedRecordsCount = invalidAccounts.Count;
                pathDto.SuccessRecordsCount = SuccessRecordsCount;
                await _importPathsAppService.SaveFilePath(pathDto);
                //  await _hubcontext.Clients.All.SendAsync("chartOfAccount", file, "file");
              //  await _appNotifier.SomeUsersCouldntBeImported(args.User, file.FileToken, file.FileType, file.FileName);
            }
            else
            {
                await _appNotifier.SendMessageAsync(
                    args.User,
                    _localizationSource.GetString("AllAccountsSuccessfullyImportedFromExcel"),
                    Abp.Notifications.NotificationSeverity.Success);
            }
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
        public bool CheckNullValues(ChartsOfAccountsTrialBalanceExcellImportDto input)
        {
            // bool isValid = false;
            if (string.IsNullOrEmpty(input.AccountName))
            {
                return true;
            }
            else if (string.IsNullOrEmpty(input.AccountNumber))
            {
                return true;
            }
            else if (string.IsNullOrEmpty(input.Balance))
            {
                return true;
            }
            else
            {
                return false;
            }

        }
        #endregion
    }
}
