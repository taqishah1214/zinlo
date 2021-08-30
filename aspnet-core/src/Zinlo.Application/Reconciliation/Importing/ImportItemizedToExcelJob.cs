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
    public class ImportItemizedToExcelJob : BackgroundJob<ImportItemizedFromExcelJobArgs>, ITransientDependency
    {
        private readonly IItemizedListExcelDataReader _itemizedListExcelDataReader;
        private readonly IAppNotifier _appNotifier;
        private readonly IInvalidItemizedExporter _invalidItemizedExporter;
        private readonly IItemizationAppService _itemizationAppService;        
        private readonly IBinaryObjectManager _binaryObjectManager;
        private readonly ILocalizationSource _localizationSource;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly IImportPathsAppService _importPathsAppService;

        public UserManager userManager { get; set; }
        public long TenantId = 0;
        public long UserId = 0;
        public int SuccessRecordsCount = 0;
        public long loggedFileId = 0;
                
        public ImportItemizedToExcelJob(
        IItemizationAppService itemizationAppService,
        IItemizedListExcelDataReader itemizedListExcelDataReader,
        IInvalidItemizedExporter invalidItemizedExcelExporter,        
        IAppNotifier appNotifier,
        IBinaryObjectManager binaryObjectManager,
        ILocalizationManager localizationManager,
        IImportPathsAppService importPathsAppService,
        IUnitOfWorkManager unitOfWorkManager

        )
        {
            _itemizedListExcelDataReader = itemizedListExcelDataReader;
            _invalidItemizedExporter = invalidItemizedExcelExporter;            
            _itemizationAppService = itemizationAppService;
            _appNotifier = appNotifier;
            _binaryObjectManager = binaryObjectManager;
            _localizationSource = localizationManager.GetSource(ZinloConsts.LocalizationSourceName);
            _importPathsAppService = importPathsAppService;
            _unitOfWorkManager = unitOfWorkManager;
        }

        [UnitOfWork]
        public override void Execute(ImportItemizedFromExcelJobArgs args)
        {
            TenantId = (int)args.TenantId;
            UserId = args.User.UserId;

            using (CurrentUnitOfWork.SetTenantId(args.TenantId))
            {
                var accountsList = GetAccountsListFromExcelOrNull(args);
                var fileUrl = _invalidItemizedExporter.ExportToFile(accountsList);
                ImportPathDto pathDto = new ImportPathDto();
                pathDto.FilePath = "";
                pathDto.Type = "Itemized";
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
                AddItemized(args, accountsList);
            }
        }

        private List<ItemizedExcelImportDto> GetAccountsListFromExcelOrNull(ImportItemizedFromExcelJobArgs args)
        {
            try
            {
                var file = AsyncHelper.RunSync(() => _binaryObjectManager.GetOrNullAsync(args.BinaryObjectId));
                return _itemizedListExcelDataReader.GetAccountsFromExcel(file.Bytes);
            }
            catch (Exception)
            {
                return null;
            }
        }
                
        private void AddItemized(ImportItemizedFromExcelJobArgs args, List<ItemizedExcelImportDto> accounts)
        {            
            var invalidRecords = new List<ItemizedExcelImportDto>();
            var validRecords = new List<ItemizedExcelImportDto>();

            #region |bell notification|
            AsyncHelper.RunSync(()=>_appNotifier.SendMessageAsync(
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
                        var result = CheckErrors(account);                        
                        if (result.isValid)
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
            pathDtoUpdate.Type = "Itemized";
            pathDtoUpdate.TenantId = (int)TenantId;
            pathDtoUpdate.UploadedFilePath = args.Url;//
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
                    AsyncHelper.RunSync(() => CreateOrUpdateItemized(item, args));
                }
                unitOfWork.Complete();
            }
            AsyncHelper.RunSync(() => ProcessImportItemizedAccountsResultAsync(args, invalidRecords));
        }

        private async Task CreateOrUpdateItemized(ItemizedExcelImportDto input, ImportItemizedFromExcelJobArgs args)
        {
            var tenantId = CurrentUnitOfWork.GetTenantId();
            CreateOrEditItemizationDto createOrEditItemizationDto = new CreateOrEditItemizationDto();
            createOrEditItemizationDto.InoviceNo = input.InoviceNo;
            createOrEditItemizationDto.JournalEntryNo = input.JournalEntryNo;
            createOrEditItemizationDto.Date = Convert.ToDateTime(input.Date);
            createOrEditItemizationDto.Description = input.Description;
            createOrEditItemizationDto.Amount = Convert.ToDouble(input.Amount);
            createOrEditItemizationDto.ChartsofAccountId = args.ChartsofAccountsId;
            await _itemizationAppService.CreateOrEdit(createOrEditItemizationDto);            
        }

        private async Task ProcessImportItemizedAccountsResultAsync(ImportItemizedFromExcelJobArgs args, List<ItemizedExcelImportDto> invalidAccounts)
        {
            if (invalidAccounts.Any())
            {

                #region|Update log|
                var url = _invalidItemizedExporter.ExportToFile(invalidAccounts);
                ImportPathDto pathDto = new ImportPathDto();
                pathDto.Id = loggedFileId;
                pathDto.FilePath = url;
                pathDto.UploadedFilePath = args.Url;//
                pathDto.FailedRecordsCount = invalidAccounts.Count;
                pathDto.SuccessRecordsCount = SuccessRecordsCount;
                pathDto.FileStatus = ImportPaths.Dto.Status.Completed;
                pathDto.UploadMonth = args.SelectedMonth;//
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
                pathDto.UploadMonth = args.SelectedMonth;//
                pathDto.UploadedFilePath = args.Url;//
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

        private void SendInvalidExcelNotification(ImportItemizedFromExcelJobArgs args)
        {
            AsyncHelper.RunSync(() => _appNotifier.SendMessageAsync(
                args.User,
                _localizationSource.GetString("FileCantBeConvertedToAccountsList"),
                Abp.Notifications.NotificationSeverity.Warn));
        }
                
        public ItemizedExcelImportDto CheckErrors(ItemizedExcelImportDto input)
        {            
            bool isDate = true;
            bool isAmount = true;
            bool isDescription = true;
            string errorMessage = string.Empty;
                        
            if (string.IsNullOrEmpty(input.Date))
            {
                isDate = false;
                errorMessage += "Date,";
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

            if (isAmount == false || isDate == false || isDescription == false)
            {
                input.isValid = false;
                input.Exception = errorMessage + _localizationSource.GetString("EmptyValuesError");
                return input;
            }
            else
            {
                input.isValid = true;
                return input;
            }
        }

    }

}