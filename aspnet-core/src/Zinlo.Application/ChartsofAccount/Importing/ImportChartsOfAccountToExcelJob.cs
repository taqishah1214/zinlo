using Abp.BackgroundJobs;
using Abp.Dependency;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Localization;
using Abp.Localization.Sources;
using Abp.ObjectMapping;
using Abp.Runtime.Session;
using Abp.Threading;
using Abp.UI;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Zinlo.AccountSubType;
using Zinlo.Authorization.Roles;
using Zinlo.Authorization.Users;
using Zinlo.ChartsofAccount.Dtos;
using Zinlo.ChartsofAccount.Importing;
using Zinlo.ImportPaths;
using Zinlo.ImportPaths.Dto;
using Zinlo.Notifications;
using Zinlo.Storage;
namespace Zinlo.ChartsofAccount
{

    public class ImportChartsOfAccountToExcelJob : BackgroundJob<ImportChartsOfAccountFromExcelJobArgs>, ITransientDependency
    {
        private readonly IImportPathsAppService _importPathsAppService;
        private readonly IChartsOfAccontListExcelDataReader _chartsOfAccontListExcelDataReader;
        private readonly IAccountSubTypeAppService _accountSubTypeAppService;
        private readonly IInvalidAccountsExcellExporter _invalidAccountsExporter;
        private readonly IRepository<ChartsofAccount, long> _chartsOfAccountsrepository;
        private readonly IAppNotifier _appNotifier;
        private readonly IBinaryObjectManager _binaryObjectManager;
        private readonly ILocalizationSource _localizationSource;
        private readonly IObjectMapper _objectMapper;
        private readonly IHubContext<JobHub.JobHub> _hubcontext;

        public UserManager userManager { get; set; }
        public long TenantId = 0;
        public long UserId = 0;
        public int SuccessRecordsCount = 0;
        public long loggedFileId = 0;
        public ImportChartsOfAccountToExcelJob(

        IChartsOfAccontListExcelDataReader chartsOfAccontListExcelDataReader,
            IAccountSubTypeAppService accountSubTypeAppService,
            IRepository<ChartsofAccount, long> chartsOfAccountsrepository,
            IInvalidAccountsExcellExporter invalidAccountsExporter,
             IAppNotifier appNotifier,
            IBinaryObjectManager binaryObjectManager,
            ILocalizationManager localizationManager,
            IObjectMapper objectMapper,
            IImportPathsAppService importPathsAppService,
            IHubContext<JobHub.JobHub> hubContext

            )
        {
            _chartsOfAccontListExcelDataReader = chartsOfAccontListExcelDataReader;
            _chartsOfAccountsrepository = chartsOfAccountsrepository;
            _invalidAccountsExporter = invalidAccountsExporter;
            _accountSubTypeAppService = accountSubTypeAppService;
            _appNotifier = appNotifier;
            _binaryObjectManager = binaryObjectManager;
            _objectMapper = objectMapper;
            _localizationSource = localizationManager.GetSource(ZinloConsts.LocalizationSourceName);
            _importPathsAppService = importPathsAppService;
            _hubcontext = hubContext;
        }

        [UnitOfWork]
        public override void Execute(ImportChartsOfAccountFromExcelJobArgs args)
        {
            TenantId = (int)args.TenantId;
            UserId = args.User.UserId;

            using (CurrentUnitOfWork.SetTenantId(args.TenantId))
            {
                var chartsofaccount = GetAccountsListFromExcelOrNull(args);
                if (chartsofaccount == null || !chartsofaccount.Any())
                {
                    SendInvalidExcelNotification(args);
                    return;
                }

                CreateChartsOfAccounts(args, chartsofaccount);
            }
        }

        private List<ChartsOfAccountsExcellImportDto> GetAccountsListFromExcelOrNull(ImportChartsOfAccountFromExcelJobArgs args)
        {
            try
            {
                var file = AsyncHelper.RunSync(() => _binaryObjectManager.GetOrNullAsync(args.BinaryObjectId));
                return _chartsOfAccontListExcelDataReader.GetAccountsFromExcel(file.Bytes);
            }
            catch (Exception)
            {
                return null;
            }
        }

        private void CreateChartsOfAccounts(ImportChartsOfAccountFromExcelJobArgs args, List<ChartsOfAccountsExcellImportDto> accounts)
        {
            var invalidAccounts = new List<ChartsOfAccountsExcellImportDto>();
            var validRecords = new List<ChartsOfAccountsExcellImportDto>();

            foreach (var account in accounts)
            {
                if (account.CanBeImported())
                {

                    account.isValid = true;


                    try
                    {
                        var result = CheckErrors(account);
                        var data = CheckReconciliationTypeErrors(result);
                        if (data.isValid)
                        {
                            var finalResult = CheckAssignee(data);
                            if (finalResult.isValid)
                            {
                                validRecords.Add(data);
                            }
                            else
                            {
                                invalidAccounts.Add(data);
                            }

                          
                        }
                        else
                        {
                            invalidAccounts.Add(result);
                        }

                    }
                    catch (UserFriendlyException exception)
                    {
                        
                    }
                   
                }              
            }
            List<ChartsOfAccountsExcellImportDto> ValidRows = accounts.Except(invalidAccounts).ToList();
            SuccessRecordsCount = validRecords.Count;

            #region|Log intial info|
            ImportPathDto pathDto = new ImportPathDto();
            pathDto.FilePath = "";
            pathDto.Type = FileTypes.ChartOfAccounts.ToString();
            pathDto.TenantId = (int)TenantId;
            pathDto.CreatorId = UserId;
            pathDto.FailedRecordsCount = 0;
            pathDto.SuccessRecordsCount = 0;
          loggedFileId =  _importPathsAppService.SaveFilePath(pathDto);
            #endregion

            foreach (var item in validRecords)
            {
                AsyncHelper.RunSync(() => CreateChartsOfAccountAsync(item));
            }
            AsyncHelper.RunSync(() => ProcessImportAccountsResultAsync(args, invalidAccounts));
        }

        private async Task CreateChartsOfAccountAsync(ChartsOfAccountsExcellImportDto input)
        {
            var tenantId = CurrentUnitOfWork.GetTenantId();
            var allData = await _chartsOfAccountsrepository.GetAllListAsync();
            var result = allData.Where(a => a.AccountNumber.ToLower() == input.AccountNumber.ToLower()).FirstOrDefault();
            if (result != null)
            {
                result.TenantId = (int)tenantId;
                result.AccountName = input.AccountName;
                result.CreationTime = DateTime.Now.ToUniversalTime();
                result.Status = (Status)2;
                result.AssigneeId = await GetUserIdByEmail(input.AssignedUser);
                result.CreatorUserId = UserId;

                result.AccountType = (AccountType)GetAccountTypeValue(input.AccountType);
                result.AccountSubTypeId = await _accountSubTypeAppService.GetAccountSubTypeIdByTitle(input.AccountSubType, UserId, TenantId);
                result.Reconciled = (Reconciled)GetReconciledValue(input.ReconciliationAs);
                result.ReconciliationType = (ReconciliationType)GetReconcilationTypeValue(input.ReconciliationType);

                await _chartsOfAccountsrepository.UpdateAsync(result);
            }
            else
            {
                ChartsofAccount account = new ChartsofAccount();
                account.TenantId = (int)tenantId;
                account.AccountName = input.AccountName;
                account.AccountNumber = input.AccountNumber;
                account.CreationTime = DateTime.Now.ToUniversalTime();
                account.Status = (Status)2;
                account.AssigneeId = await GetUserIdByEmail(input.AssignedUser);
                account.CreatorUserId = UserId;

                account.AccountType = (AccountType)GetAccountTypeValue(input.AccountType);
                account.AccountSubTypeId = await _accountSubTypeAppService.GetAccountSubTypeIdByTitle(input.AccountSubType, UserId, TenantId);
                account.ReconciliationType = (ReconciliationType)GetReconcilationTypeValue(input.ReconciliationType);
                int type = GetReconcilationTypeValue(input.ReconciliationType);
                account.Reconciled = (Reconciled)GetReconciledValue(input.ReconciliationAs);

                await _chartsOfAccountsrepository.InsertAsync(account);

            }
        }

        private async Task ProcessImportAccountsResultAsync(ImportChartsOfAccountFromExcelJobArgs args, List<ChartsOfAccountsExcellImportDto> invalidAccounts)
        {
            //////await _appNotifier.SendMessageAsync(
            //////       args.User,
            //////       _localizationSource.GetString("AllAccountsSuccessfullyImportedFromExcel"),
            //////       Abp.Notifications.NotificationSeverity.Success);


            if (invalidAccounts.Any())
            {

                #region|Update log|
                var url = _invalidAccountsExporter.ExportToFile(invalidAccounts);
                ImportPathDto pathDto = new ImportPathDto();
                pathDto.Id = loggedFileId;
                pathDto.FilePath = url;
                pathDto.FailedRecordsCount = invalidAccounts.Count;
                pathDto.SuccessRecordsCount = SuccessRecordsCount;
                await _importPathsAppService.UpdateFilePath(pathDto);
                #endregion

                await _appNotifier.SendMessageAsync(
                       args.User,
                       _localizationSource.GetString("SomeAccountsSuccessfullyImportedFromExcel"),
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
                    _localizationSource.GetString("AllAccountsSuccessfullyImportedFromExcel"),
                    Abp.Notifications.NotificationSeverity.Success);
            }
        }

        private void SendInvalidExcelNotification(ImportChartsOfAccountFromExcelJobArgs args)
        {
            AsyncHelper.RunSync(() => _appNotifier.SendMessageAsync(
                args.User,
                _localizationSource.GetString("FileCantBeConvertedToAccountsList"),
                Abp.Notifications.NotificationSeverity.Warn));
        }
        #region|Helpers|
        public async Task<long> GetUserIdByEmail(string emailAddress)
        {
            var user = await userManager.FindByEmailAsync(emailAddress);

            return user.Id;
        }

        public int GetReconciliationTypeValue(string name)
        {
            if (name.Trim().ToLower() == "itemized")
            {
                return 1;
            }
            else
            {
                return 2;
            }
        }
        public int GetAccountTypeValue(string name)
        {
            if (name.Trim().ToLower() == "fixed")
            {
                return 1;
            }
            else if (name.Trim().ToLower() == "assets")
            {
                return 2;
            }
            else
            {
                return 3;
            }
        }

        public int GetReconcilationTypeValue(string name)
        {
            if (name.Trim().ToLower() == "itemized")
            {
                return 1;
            }
            else
            {
                return 2;
            }
        }

        public int GetReconciledValue(string name)
        {
            if (name.Trim().ToLower() == "netamount")
            {
                return 1;
            }
            else if (name.Trim().ToLower() == "beginningamount")
            {
                return 2;
            }
            else if (name.Trim().ToLower() == "accruedAmount")
            {
                return 3;
            }
            else
            {
                return 0;
            }
        }

        public ChartsOfAccountsExcellImportDto CheckReconciliationTypeErrors(ChartsOfAccountsExcellImportDto input)
        {
            bool result = false;
            string[] strReconciledArray = { "netamount", "beginningamount", "accruedamount" };
            if (!string.IsNullOrEmpty(input.ReconciliationType))
            {
                if (input.ReconciliationType.Trim().ToLower() == "amortization")
                {
                    result = !Array.Exists(strReconciledArray, E => E == input.ReconciliationAs.ToLower());
                }
            }
            if (result)
            {
                input.isValid = false;
                input.Exception += _localizationSource.GetString("ReconcilationError");
                return input;
            }
            else
            {
                return input;
            }
        }
        public ChartsOfAccountsExcellImportDto CheckAssignee(ChartsOfAccountsExcellImportDto input)
        {
            bool isEmail = Regex.IsMatch(input.AssignedUser, @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z", RegexOptions.IgnoreCase);
            if (isEmail)
            {
                var result = userManager.FindByEmailAsync(input.AssignedUser);
                if (result.Result != null)
                {

                    return input;
                }
                else
                {
                    input.isValid = false;
                    input.Exception += _localizationSource.GetString("AssigneeDoesNotExist");
                    return input;
                }

            }
            else
            {
                input.isValid = false;
                return input;
            }
        }
        #endregion
        public ChartsOfAccountsExcellImportDto CheckErrors(ChartsOfAccountsExcellImportDto input)
        {

            bool isAccountName = false;
            bool isAccountNumber = false;
            bool isAccountType = false;
            bool isAccountSubType = false;
            bool isReconciliationType = false;
            string errorMessage = string.Empty;

            if (string.IsNullOrEmpty(input.AccountName))
            {
                isAccountName = true;
                errorMessage += "AccountName,";
            }
            if (string.IsNullOrEmpty(input.AccountNumber))
            {
                isAccountNumber = true;
                errorMessage += "AccountNumber,";
            }
            if (string.IsNullOrEmpty(input.AccountType))
            {
                isAccountType = true;
                errorMessage += "AccountType,";
            }
            if (string.IsNullOrEmpty(input.AccountSubType))
            {
                isAccountSubType = true;
                errorMessage += "AccountSubType,";
            }
            if (string.IsNullOrEmpty(input.ReconciliationType))
            {
                isReconciliationType = true;
                errorMessage += "ReconciliationType,";
            }

            if (isAccountName == true || isAccountNumber == true
                || isAccountSubType == true || isReconciliationType == true || isAccountType == true)
            {
                
                input.isValid = false;
                input.Exception = errorMessage + _localizationSource.GetString("EmptyValuesError");
                return input;
            }
            else
            {
                return input;

            }
        }

    }

}