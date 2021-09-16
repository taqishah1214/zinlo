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
using System.Linq;
using System.Threading.Tasks;
using Zinlo.Authorization.Users;
using Zinlo.ImportPaths;
using Zinlo.ImportPaths.Dto;
using Zinlo.Notifications;
using Zinlo.Reconciliation.Dtos;
using Zinlo.Storage;

namespace Zinlo.Reconciliation.Importing
{
    public class ImportAmortizedToExcelJob : BackgroundJob<ImportAmortizedFromExcelJobArgs>, ITransientDependency
    {
        private readonly IAmortizedListExcelDataReader _amortizedListExcelDataReader;
        private readonly IAppNotifier _appNotifier;
        private readonly IInvalidAmortizedExporter _invalidAmortizedExporter;
        private readonly IAmortizationAppService _amortizationAppService;
        private readonly IBinaryObjectManager _binaryObjectManager;
        private readonly ILocalizationSource _localizationSource;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly IImportPathsAppService _importPathsAppService;

        public UserManager userManager { get; set; }
        public long TenantId = 0;
        public long UserId = 0;
        public int SuccessRecordsCount = 0;
        public long loggedFileId = 0;

        public ImportAmortizedToExcelJob(            
            IAmortizationAppService amortizationAppService,
            IAmortizedListExcelDataReader amortizedListExcelDataReader,
            IInvalidAmortizedExporter invalidAmortizedExcelExporter,
            IAppNotifier appNotifier,
            IBinaryObjectManager binaryObjectManager,
            ILocalizationManager localizationManager,
            IImportPathsAppService importPathsAppService,
            IUnitOfWorkManager unitOfWorkManager
            )
        {
            _amortizedListExcelDataReader = amortizedListExcelDataReader;
            _invalidAmortizedExporter = invalidAmortizedExcelExporter;
            _amortizationAppService = amortizationAppService;
            _appNotifier = appNotifier;
            _binaryObjectManager = binaryObjectManager;
            _localizationSource = localizationManager.GetSource(ZinloConsts.LocalizationSourceName);
            _importPathsAppService = importPathsAppService;
            _unitOfWorkManager = unitOfWorkManager;
        }

        [UnitOfWork]
        public override void Execute(ImportAmortizedFromExcelJobArgs args)
        {
            TenantId = (int)args.TenantId;
            UserId = args.User.UserId;

            using (CurrentUnitOfWork.SetTenantId(args.TenantId))
            {
                var accountsList = GetAccountsListFromExcelOrNull(args);
                var fileUrl = _invalidAmortizedExporter.ExportToFile(accountsList);
                ImportPathDto pathDto = new ImportPathDto();
                pathDto.FilePath = "";
                pathDto.Type = "Amortized";
                pathDto.TenantId = (int)TenantId;
                pathDto.UploadedFilePath = args.Url;
                pathDto.CreatorId = UserId;
                pathDto.FailedRecordsCount = 0;
                pathDto.SuccessRecordsCount = 0;
                pathDto.SuccessFilePath = fileUrl;
                pathDto.UploadMonth = args.SelectedMonth;
                pathDto.FileStatus = ImportPaths.Dto.Status.InProcess;
                loggedFileId = _importPathsAppService.SaveFilePath(pathDto);
                if (accountsList == null || !accountsList.Any())
                {
                    SendInvalidExcelNotification(args);
                    return;
                }
                AddAmortized(args, accountsList);
            }
        }

        private List<AmortizedExcelImportDto> GetAccountsListFromExcelOrNull(ImportAmortizedFromExcelJobArgs args)
        {
            try
            {
                var file = AsyncHelper.RunSync(() => _binaryObjectManager.GetOrNullAsync(args.BinaryObjectId));
                return _amortizedListExcelDataReader.GetAccountsFromExcel(file.Bytes);
            }
            catch (Exception)
            {
                return null;
            }
        }

        private void AddAmortized(ImportAmortizedFromExcelJobArgs args, List<AmortizedExcelImportDto> accounts)
        {
            var invalidRecords = new List<AmortizedExcelImportDto>();
            var validRecords = new List<AmortizedExcelImportDto>();

            #region |bell notification|
            AsyncHelper.RunSync(() => _appNotifier.SendMessageAsync(
                    args.User,
                    "file logged",
                    Abp.Notifications.NotificationSeverity.Success));
            #endregion

            foreach (var account in accounts)
            {
                if (account.CanBeImported())
                {
                    account.IsValid = true;
                    try
                    {
                        var result = CheckErrors(account);
                        if (result.IsValid)
                        {
                            validRecords.Add(result);
                        }
                        else
                        {
                            account.Exception = result.Exception;
                            invalidRecords.Add(result);
                        }
                    }
                    catch (UserFriendlyException exception)
                    {
                        
                    }
                }
                else
                {
                    invalidRecords.Add(account);
                }
            }
            SuccessRecordsCount = validRecords.Count;

            #region|Log intial info|
            ImportPathDto pathDtoUpdate = new ImportPathDto();
            pathDtoUpdate.Id = loggedFileId;
            pathDtoUpdate.FilePath = "";
            pathDtoUpdate.Type = "Amortized";
            pathDtoUpdate.TenantId = (int)TenantId;
            pathDtoUpdate.UploadedFilePath = args.Url;
            pathDtoUpdate.CreatorId = UserId;
            pathDtoUpdate.FailedRecordsCount = 0;
            pathDtoUpdate.SuccessRecordsCount = 0;
            pathDtoUpdate.FileStatus = ImportPaths.Dto.Status.InProcess;
            _importPathsAppService.UpdateFilePath(pathDtoUpdate);
            #endregion
            #region|mgs to  be deleted|
            AsyncHelper.RunSync(() => _appNotifier.SendMessageAsync(
                  args.User,
                  "Insertion started",
                  Abp.Notifications.NotificationSeverity.Success));
            #endregion
            using (var unitOfWork = _unitOfWorkManager.Begin())
            {
                foreach (var item in validRecords)
                {
                    AsyncHelper.RunSync(() => CreateOrUpdateAmortized(item, args));
                }
                unitOfWork.Complete();
            }
            AsyncHelper.RunSync(() => ProcessImportAmortizedAccountsResultAsync(args, invalidRecords));
        }
    

    private async Task CreateOrUpdateAmortized(AmortizedExcelImportDto input, ImportAmortizedFromExcelJobArgs args)
    {
        var tenantId = CurrentUnitOfWork.GetTenantId();
        CreateOrEditAmortizationDto createOrEditAmortizationDto = new CreateOrEditAmortizationDto();
        createOrEditAmortizationDto.InoviceNo = input.InvoiceNo;
        createOrEditAmortizationDto.JournalEntryNo = input.JournalEntryNo;
        createOrEditAmortizationDto.StartDate = Convert.ToDateTime(input.StartDate);
        createOrEditAmortizationDto.EndDate = Convert.ToDateTime(input.EndDate);
        createOrEditAmortizationDto.Amount = Convert.ToDouble(input.Amount);
        createOrEditAmortizationDto.Description = input.Description;
        string criteria = (input.Criteria).ToLower();
        switch (criteria)
        {
            case "manual":
                createOrEditAmortizationDto.Criteria = Dtos.Criteria.Manual;
                break;
            case "monthly":
                createOrEditAmortizationDto.Criteria = Dtos.Criteria.Monthly;
                break;
            case "daily":
                createOrEditAmortizationDto.Criteria = Dtos.Criteria.Daily;
                break;                
        }        
        createOrEditAmortizationDto.ChartsofAccountId = args.ChartsofAccountsId;     
        await _amortizationAppService.CreateOrEdit(createOrEditAmortizationDto);
    }

    private async Task ProcessImportAmortizedAccountsResultAsync(ImportAmortizedFromExcelJobArgs args, List<AmortizedExcelImportDto> invalidAccounts)
    {
        if (invalidAccounts.Any())
        {

            #region|Update log|
            var url = _invalidAmortizedExporter.ExportToFile(invalidAccounts);
            ImportPathDto pathDto = new ImportPathDto();
            pathDto.Id = loggedFileId;
            pathDto.FilePath = url;
            pathDto.UploadedFilePath = args.Url;
            pathDto.FailedRecordsCount = invalidAccounts.Count;
            pathDto.SuccessRecordsCount = SuccessRecordsCount;
            pathDto.FileStatus = ImportPaths.Dto.Status.Completed;
            pathDto.UploadMonth = args.SelectedMonth;
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
            pathDto.UploadMonth = args.SelectedMonth;
            pathDto.UploadedFilePath = args.Url;
            pathDto.FailedRecordsCount = invalidAccounts.Count;
            pathDto.SuccessRecordsCount = SuccessRecordsCount;
            pathDto.FileStatus = ImportPaths.Dto.Status.Completed;
            await _importPathsAppService.UpdateFilePath(pathDto);
            #endregion

            await _appNotifier.SendMessageAsync(
                args.User,
                _localizationSource.GetString("AllAccountsSuccessfullyImportedFromExcel"),
                Abp.Notifications.NotificationSeverity.Success);
        }
    }

        private void SendInvalidExcelNotification(ImportAmortizedFromExcelJobArgs args)
        {
            AsyncHelper.RunSync(() => _appNotifier.SendMessageAsync(
                args.User,
                _localizationSource.GetString("FileCantBeConvertedToAccountsList"),
                Abp.Notifications.NotificationSeverity.Warn));
        }

        public AmortizedExcelImportDto CheckErrors(AmortizedExcelImportDto input)
        {
            bool isStartDate = true;
            bool isEndDate = true;
            bool isAmount = true;
            bool isDescription = true;
            bool isCriteria = true;
            bool isCriteriaCorrect = true;
            string errorMessage = string.Empty;
                        
            if (string.IsNullOrEmpty(input.StartDate))
            {
                isStartDate = false;
                errorMessage += "StartDate,";
            }
            if (string.IsNullOrEmpty(input.EndDate))
            {
                isEndDate = false;
                errorMessage += "EndDate,";
            }
            if (string.IsNullOrEmpty(input.Amount))
            {
                isAmount = false;
                errorMessage += "Amount,";
            }
            if (string.IsNullOrEmpty(input.Description))
            {
                isDescription = false;
                errorMessage += "Description,";
            }
            if (string.IsNullOrEmpty(input.Criteria))
            {
                isCriteria = false;
                errorMessage += "Criteria,";
            }
            if (!(input.Criteria.ToLower() == "daily" || input.Criteria.ToLower() == "manual" || input.Criteria.ToLower() == "monthly"))
            {
                isCriteriaCorrect = false;
                errorMessage += "Incorrect Criteria,";
            }

            if (isStartDate == false || isEndDate == false || isAmount == false || isDescription == false || isCriteria == false || isCriteriaCorrect == false)
            {
                input.IsValid = false;
                input.Exception = errorMessage + _localizationSource.GetString("EmptyValuesError");
                return input;
            }
            else
            {
                input.IsValid = true;
                return input;
            }
        }
    }
}
