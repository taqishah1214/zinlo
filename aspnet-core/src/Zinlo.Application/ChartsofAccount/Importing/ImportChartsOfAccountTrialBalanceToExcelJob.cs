using System.Linq;
using Abp.BackgroundJobs;
using Abp.Dependency;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Localization;
using Abp.Localization.Sources;
using Abp.Threading;
using Abp.UI;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Zinlo.Authorization.Users;
using Zinlo.ChartofAccounts;
using Zinlo.ChartsofAccount.Dtos;
using Zinlo.Notifications;
using Zinlo.Storage;
using Zinlo.ImportPaths.Dto;
using Zinlo.ImportPaths;
using Zinlo.ImportsPaths;
using Status = Zinlo.ImportPaths.Dto.Status;

namespace Zinlo.ChartsofAccount.Importing
{
    public class ImportChartsOfAccountTrialBalanceToExcelJob : BackgroundJob<ImportChartsOfAccountTrialBalanceFromExcelJobArgs>, IPerWebRequestDependency
    {
        private readonly IChartsOfAccontTrialBalanceListExcelDataReader _chartsOfAccontTrialBalanceListExcelDataReader;
        private readonly IAppNotifier _appNotifier;
        private readonly IRepository<ImportsPath, long> _importPathsRepository;
        private readonly IBinaryObjectManager _binaryObjectManager;
        private readonly ILocalizationSource _localizationSource;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly IRepository<AccountBalance, long> _accountBalanceRepository;
        private readonly IRepository<ChartofAccounts.ChartofAccounts, long> _chartsofAccountRepository;
        public UserManager userManager { get; set; }
        public long UserId = 0;
        private readonly IInvalidAccountsTrialBalanceExporter _invalidAccountsTrialBalanceExporter;
        private readonly IImportPathsAppService _importPathsAppService;
        public long TenantId = 0;
        public int SuccessRecordsCount = 0;
        public long loggedFileId = 0;
        public bool isAccountsZero = false;
        public ImportChartsOfAccountTrialBalanceToExcelJob(

        IChartsOfAccontTrialBalanceListExcelDataReader chartsOfAccontTrialBalanceListExcelDataReader,
        IInvalidAccountsTrialBalanceExporter invalidAccountsTrialBalanceExporter,
        IAppNotifier appNotifier,
            IBinaryObjectManager binaryObjectManager,
            ILocalizationManager localizationManager,
            IImportPathsAppService importPathsAppService,
            IUnitOfWorkManager unitOfWorkManager,
        IRepository<ChartofAccounts.ChartofAccounts, long> chartsofAccountRepository,
        IRepository<AccountBalance, long> accountBalanceRepository, IRepository<ImportsPath, long> importPathsRepository)
        {
            _chartsOfAccontTrialBalanceListExcelDataReader = chartsOfAccontTrialBalanceListExcelDataReader;
            _appNotifier = appNotifier;
            _binaryObjectManager = binaryObjectManager;
            _localizationSource = localizationManager.GetSource(ZinloConsts.LocalizationSourceName);
            _invalidAccountsTrialBalanceExporter = invalidAccountsTrialBalanceExporter;
            _importPathsAppService = importPathsAppService;
            _unitOfWorkManager = unitOfWorkManager;
            _chartsofAccountRepository = chartsofAccountRepository;
            _accountBalanceRepository = accountBalanceRepository;
            _importPathsRepository = importPathsRepository;
        }

        [UnitOfWork]
        public override void Execute(ImportChartsOfAccountTrialBalanceFromExcelJobArgs args)
        {
            TenantId = (int)args.TenantId;
            UserId = args.User.UserId;
            using (CurrentUnitOfWork.SetTenantId(args.TenantId))
            {
                var accountsTrialBalance = GetAccountsTrialBalanceListFromExcelOrNull(args);

                var fileUrl = _invalidAccountsTrialBalanceExporter.ExportToFile(accountsTrialBalance);
                ImportPathDto pathDto = new ImportPathDto();
                pathDto.FilePath = "";
                pathDto.Type = FileTypes.TrialBalance.ToString();
                pathDto.TenantId = (int)TenantId;
                pathDto.UploadedFilePath = args.url;
                pathDto.CreatorId = UserId;
                pathDto.FailedRecordsCount = 0;
                pathDto.SuccessRecordsCount = 0;
                pathDto.SuccessFilePath = fileUrl;
                pathDto.UploadMonth = args.selectedMonth;
                pathDto.FileStatus = Status.InProcess;
                loggedFileId = _importPathsAppService.SaveFilePath(pathDto);

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
                var file = AsyncHelper.RunSync(() => _binaryObjectManager.GetOrNullAsync(args.BinaryObjectId));
                var accountsCount = _chartsofAccountRepository.GetAll().Count();
                var isTrue = accountsCount > 0;
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

            #region|mgs to  be deleted|
            AsyncHelper.RunSync(() => _appNotifier.SendMessageAsync(
                  args.User,
                  "file logged",
                  Abp.Notifications.NotificationSeverity.Success));
            #endregion
            foreach (var account in accounts)
            {
                if (account.CanBeImported())
                {
                    try
                    {
                        var result = CheckNullValues(account);
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

            SuccessRecordsCount = list.Count;
            #region|Log intial info|

            var pathDtoUpdate = new ImportPathDto
            {
                Id = loggedFileId,
                FilePath = "",
                Type = FileTypes.TrialBalance.ToString(),
                TenantId = (int)TenantId,
                UploadedFilePath = args.url,
                CreatorId = UserId,
                FailedRecordsCount = 0,
                SuccessRecordsCount = 0,
                FileStatus = Status.InProcess,
                UploadMonth = args.selectedMonth
            };
            UpdateFilePath(pathDtoUpdate);
            #endregion
            #region|mgs to  be deleted|
            AsyncHelper.RunSync(() => _appNotifier.SendMessageAsync(
                  args.User,
                  "Insertion started",
                  Abp.Notifications.NotificationSeverity.Success));
            #endregion
            //using (var unitOfWork = _unitOfWorkManager.Begin())
            //{

            foreach (var item in list)
            {
                CreateChartsOfAccountTrialBalanceAsync(item, args.selectedMonth);

            }
            //unitOfWork.Complete();
            //}
            ProcessImportAccountsTrialBalanceResultAsync(args, invalidAccounts);
        }
        private void UpdateFilePath(ImportPathDto input)
        {
            var output = _importPathsRepository.FirstOrDefault(input.Id);
            if (output == null) return;
            output.FilePath = input.FilePath;
            output.FailedRecordsCount = input.FailedRecordsCount;
            output.SuccessRecordsCount = input.SuccessRecordsCount;
            output.FileStatus = (Zinlo.ImportsPaths.Status)input.FileStatus;
            _importPathsRepository.Update(output);

        }
        private void AddTrialBalanceInAccount(ChartsOfAccountsTrialBalanceExcellImportDto input)
        {
           
                var accountInformation = _chartsofAccountRepository.FirstOrDefault(x =>
                    x.AccountNumber.ToLower() == input.AccountNumber.Trim().ToLower() && x.IsDeleted == false &&
                    x.ChangeTime == null);
                if (accountInformation == null) return;
                {
                    var trialBalanceIsExistOrNot = _accountBalanceRepository.FirstOrDefault(x =>
                        x.AccountId == accountInformation.Id && x.Month.Month == input.selectedMonth.Month &&
                        x.Month.Year == input.selectedMonth.Year);
                    if (trialBalanceIsExistOrNot == null)
                    {
                        var accountBalance = new AccountBalance
                        {
                            AccountId = accountInformation.Id,
                            TrialBalance = Convert.ToDouble(input.Balance),
                            Month = input.selectedMonth
                        };
                        _accountBalanceRepository.Insert(accountBalance);
                    }
                    else
                    {
                        trialBalanceIsExistOrNot.TrialBalance = Convert.ToDouble(input.Balance);
                        _accountBalanceRepository.Update(trialBalanceIsExistOrNot);
                    }

                }
                
            

        }

            private void CreateChartsOfAccountTrialBalanceAsync(ChartsOfAccountsTrialBalanceExcellImportDto input, DateTime selectedMonth)
            {
                var output = new ChartsOfAccountsTrialBalanceExcellImportDto
                {
                    AccountName = input.AccountName,
                    AccountNumber = input.AccountNumber,
                    Balance = input.Balance,
                    selectedMonth = selectedMonth
                };
                AddTrialBalanceInAccount(output);
            }



            private void ProcessImportAccountsTrialBalanceResultAsync(ImportChartsOfAccountTrialBalanceFromExcelJobArgs args, List<ChartsOfAccountsTrialBalanceExcellImportDto> invalidAccounts)
            {
                if (invalidAccounts.Any())
                {
                    #region|Update log|tefile
                    var url = _invalidAccountsTrialBalanceExporter.ExportToFile(invalidAccounts);
                    ImportPathDto pathDto = new ImportPathDto();
                    pathDto.Id = loggedFileId;
                    pathDto.FilePath = url;
                    pathDto.UploadedFilePath = args.url;
                    pathDto.FailedRecordsCount = invalidAccounts.Count;
                    pathDto.SuccessRecordsCount = SuccessRecordsCount;
                    pathDto.UploadMonth = args.selectedMonth;
                    pathDto.FileStatus = Status.Completed;
                    UpdateFilePath(pathDto);
                #endregion
                AsyncHelper.RunSync(() => _appNotifier.SendMessageAsync(
                      args.User,
                      _localizationSource.GetString("SomeAccountsTrialBalanceSuccessfullyImportedFromExcel"),
                      Abp.Notifications.NotificationSeverity.Success));
                }
                else
                {
                    #region|Update log|               
                    ImportPathDto pathDto = new ImportPathDto();
                    pathDto.Id = loggedFileId;
                    pathDto.FilePath = "";
                    pathDto.UploadMonth = args.selectedMonth;
                    pathDto.UploadedFilePath = args.url;
                    pathDto.FailedRecordsCount = invalidAccounts.Count;
                    pathDto.SuccessRecordsCount = SuccessRecordsCount;
                    pathDto.FileStatus = Status.Completed;
                    UpdateFilePath(pathDto);
                #endregion
                AsyncHelper.RunSync(() => _appNotifier.SendMessageAsync(
                        args.User,
                        _localizationSource.GetString("AllAccountsTrialBalanceSuccessfullyImportedFromExcel"),
                        Abp.Notifications.NotificationSeverity.Success));
                }
            }


            private void SendInvalidExcelNotification(ImportChartsOfAccountTrialBalanceFromExcelJobArgs args)
            {
                if (isAccountsZero)
                {
                    isAccountsZero = false;
                #region ||
                AsyncHelper.RunSync(() => _appNotifier.SendMessageAsync(
                       args.User,
                       _localizationSource.GetString("ZeroAccountsErrorMessaage"),
                       Abp.Notifications.NotificationSeverity.Success));
                    #endregion
                }
                else
                {
                #region ||
                AsyncHelper.RunSync(() => _appNotifier.SendMessageAsync(
                       args.User,
                      _localizationSource.GetString("FileCantBeConvertedToAccountsTrialBalanceList"),
                       Abp.Notifications.NotificationSeverity.Success));
                    #endregion

                }


            }
            #region|Helpers|      
            private ChartOfAccountsValidationDto CheckNullValues(ChartsOfAccountsTrialBalanceExcellImportDto input)
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

                if (isAccountNameIsNull == true || isAccountNumberIsNull == true || isBalanceIsNull == true)
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