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
        public long loggedFileId = 0;
        public bool isAccountsZero = false;
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
                var accountsTrialBalance = GetAccountsTrialBalanceListFromExcelOrNull(args);
               
                //var sumOfAllBalance =  accountsTrialBalance.Sum(p => long.Parse(p.Balance));

                //if (sumOfAllBalance != 0)
                //{
                //    throw new UserFriendlyException(L("Sum of All Account Balances is not equal to zero"));
                //}

                if (accountsTrialBalance == null || !accountsTrialBalance.Any())
                {
                    SendInvalidExcelNotification(args);
                    return;
                }
                AddTrialBalanceInAccounts(args, accountsTrialBalance);              
            }

        }

        private List<ChartsOfAccountsTrialBalanceExcellImportDto> GetAccountsTrialBalanceListFromExcelOrNull(ImportChartsOfAccountTrialBalanceFromExcelJobArgs args)
        {
            try
            {
                var file =  AsyncHelper.RunSync(() => _binaryObjectManager.GetOrNullAsync(args.BinaryObjectId));
                var isTrue = _chartsofAccountAppService.CheckAccounts();
                if (isTrue == false)
                {
                    isAccountsZero = true;

                    return new List<ChartsOfAccountsTrialBalanceExcellImportDto>();
                }

                var result = _chartsOfAccontTrialBalanceListExcelDataReader.GetAccountsTrialBalanceFromExcel(file.Bytes);
                return result;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        private void AddTrialBalanceInAccounts(ImportChartsOfAccountTrialBalanceFromExcelJobArgs args, List<ChartsOfAccountsTrialBalanceExcellImportDto> accounts)
        {
            var invalidAccounts = new List<ChartsOfAccountsTrialBalanceExcellImportDto>();
            var list = new List<ChartsOfAccountsTrialBalanceExcellImportDto>();
            var fileUrl = _invalidAccountsTrialBalanceExporter.ExportToFile(accounts);
            #region|Log intial info|
            ImportPathDto pathDto = new ImportPathDto();
            pathDto.FilePath = "";
            pathDto.Type = FileTypes.TrialBalance.ToString();
            pathDto.TenantId = (int)TenantId;
            pathDto.CreatorId = UserId;
            pathDto.FailedRecordsCount = 0;
            pathDto.SuccessRecordsCount = 0;
            pathDto.SuccessFilePath = fileUrl;
            loggedFileId = _importPathsAppService.SaveFilePath(pathDto);
            #endregion
            #region|mgs to  be deleted|
            _appNotifier.SendMessageAsync(
                  args.User,
                  "file logged",
                  Abp.Notifications.NotificationSeverity.Success);
            #endregion
            foreach (var account in accounts)
            {
                if (account.CanBeImported())
                {
                    try
                    {
                        var result =  CheckNullValues(account);
                        if (result.isTrue)
                        {
                            account.Exception = result.Exception;
                            invalidAccounts.Add(account);
                        }
                        else
                        {
                            list.Add(account);
                        }

                    }

                    catch (UserFriendlyException exception)
                    {
                        account.Exception = exception.Message;
                        invalidAccounts.Add(account);
                    }
                  
                }
                else
                {
                    invalidAccounts.Add(account);
                }
            }
            List<ChartsOfAccountsTrialBalanceExcellImportDto> ValidRows = accounts.Except(invalidAccounts).ToList();
            SuccessRecordsCount = list.Count;
            #region|Log intial info|
            ImportPathDto pathDtoUpdate = new ImportPathDto();
            pathDtoUpdate.Id = loggedFileId;
            pathDtoUpdate.FilePath = "";
            pathDtoUpdate.Type = FileTypes.TrialBalance.ToString();
            pathDtoUpdate.TenantId = (int)TenantId;
            pathDtoUpdate.CreatorId = UserId;
            pathDtoUpdate.FailedRecordsCount = 0;
            pathDtoUpdate.SuccessRecordsCount = 0;
            _importPathsAppService.UpdateFilePath(pathDtoUpdate);
            #endregion
            //list = list.Select(x => { x.VersionId = loggedFileId; return x; }).ToList();
            #region|mgs to  be deleted|
            _appNotifier.SendMessageAsync(
                  args.User,
                  "Insertion started",
                  Abp.Notifications.NotificationSeverity.Success);
            #endregion
            foreach (var item in list)
            {

                AsyncHelper.RunSync(() => CreateChartsOfAccountTrialBalanceAsync(item,args.selectedMonth));
            }
            AsyncHelper.RunSync(() => ProcessImportAccountsTrialBalanceResultAsync(args, invalidAccounts));
        }

        private  async Task CreateChartsOfAccountTrialBalanceAsync(ChartsOfAccountsTrialBalanceExcellImportDto input,DateTime selectedMonth)
        {
            var tenantId = CurrentUnitOfWork.GetTenantId();
            var output = new ChartsOfAccountsTrialBalanceExcellImportDto();
            output.AccountName = input.AccountName;
            output.AccountNumber = input.AccountNumber;
            output.Balance = input.Balance;
            output.selectedMonth = selectedMonth;
           await _chartsofAccountAppService.AddTrialBalanceInAccount(output);
        }



        private async Task ProcessImportAccountsTrialBalanceResultAsync(ImportChartsOfAccountTrialBalanceFromExcelJobArgs args, List<ChartsOfAccountsTrialBalanceExcellImportDto> invalidAccounts)
        {
            if (invalidAccounts.Any())
            {
                #region|Update log|
                var url = _invalidAccountsTrialBalanceExporter.ExportToFile(invalidAccounts);
                ImportPathDto pathDto = new ImportPathDto();
                pathDto.Id = loggedFileId;
                pathDto.FilePath = url;
                pathDto.FailedRecordsCount = invalidAccounts.Count;
                pathDto.SuccessRecordsCount = SuccessRecordsCount;
                await _importPathsAppService.UpdateFilePath(pathDto);
                #endregion
                await _appNotifier.SendMessageAsync(
                  args.User,
                  _localizationSource.GetString("SomeAccountsTrialBalanceSuccessfullyImportedFromExcel"),
                  Abp.Notifications.NotificationSeverity.Success);
            }
            else
            {
                #region|Update log|               
                ImportPathDto pathDto = new ImportPathDto();
                pathDto.Id = loggedFileId;
                pathDto.FilePath = "";
                pathDto.FailedRecordsCount = invalidAccounts.Count;
                pathDto.SuccessRecordsCount = SuccessRecordsCount;
                await _importPathsAppService.UpdateFilePath(pathDto);
                #endregion
                await _appNotifier.SendMessageAsync(
                    args.User,
                    _localizationSource.GetString("AllAccountsTrialBalanceSuccessfullyImportedFromExcel"),
                    Abp.Notifications.NotificationSeverity.Success);
            }
        }
      

        private void SendInvalidExcelNotification(ImportChartsOfAccountTrialBalanceFromExcelJobArgs args)
        {
            if(isAccountsZero)
            {
                isAccountsZero = false;
                #region ||
                _appNotifier.SendMessageAsync(
                   args.User,
                   _localizationSource.GetString("ZeroAccountsErrorMessaage"),
                   Abp.Notifications.NotificationSeverity.Success);
                #endregion
            }
            else
            {
                #region ||
                _appNotifier.SendMessageAsync(
                   args.User,
                  _localizationSource.GetString("FileCantBeConvertedToAccountsTrialBalanceList"),
                   Abp.Notifications.NotificationSeverity.Success);
                #endregion

            }


        }
        #region|Helpers|      
        private  ChartOfAccountsValidationDto CheckNullValues(ChartsOfAccountsTrialBalanceExcellImportDto input)
        {
            string errorMessage = string.Empty;
            bool isAccountNameIsNull = false;
            bool isAccountNumberIsNull = false;
            bool isBalanceIsNull = false;
            var result = new ChartOfAccountsValidationDto();
            if (string.IsNullOrEmpty(input.AccountName))
            {
                isAccountNameIsNull = true;
                errorMessage += "AccountName,";
               // return true;
            }
            else if (string.IsNullOrEmpty(input.AccountNumber))
            {
                isAccountNumberIsNull = true;
                errorMessage += "AccountNumber,";
                // return true;
            }
            else if (string.IsNullOrEmpty(input.Balance))
            {
                isBalanceIsNull = true;
                errorMessage += "Balance";
                // return true;
            }
           
            if(isAccountNameIsNull == true || isAccountNumberIsNull == true || isBalanceIsNull == true)
            {
                result.Exception = errorMessage + _localizationSource.GetString("EmptyValuesError");
                result.isTrue = true;
                return result;
            }
            else
            {
                result.isTrue = false;
                return result;
            }

        }
        #endregion
    }
}