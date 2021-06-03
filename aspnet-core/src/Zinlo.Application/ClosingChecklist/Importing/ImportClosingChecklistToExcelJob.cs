using Abp.BackgroundJobs;
using Abp.Dependency;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Localization;
using Abp.Localization.Sources;
using Abp.Runtime.Session;
using Abp.Threading;
using Abp.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using Zinlo.Authorization.Users;
using Zinlo.Categories;
using Zinlo.ClosingChecklist.Dtos;
using Zinlo.ImportPaths;
using Zinlo.ImportPaths.Dto;
using Zinlo.ImportsPaths;
using Zinlo.Notifications;
using Zinlo.Storage;

namespace Zinlo.ClosingChecklist.Importing
{
    public class ImportClosingChecklistToExcelJob : BackgroundJob<ImportClosingChecklistFromExcelJobArgs>, ITransientDependency
    {
        private readonly IClosingChecklistExcelDataReader _closingChecklistExcelDataReader;
        private readonly IAppNotifier _appNotifier;
        private readonly IRepository<ImportsPath, long> _importPathsRepository;
        private readonly IBinaryObjectManager _binaryObjectManager;
        private readonly ILocalizationSource _localizationSource;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly IRepository<ClosingChecklist, long> _closingChecklistRepository;
        private readonly IRepository<User, long> _userRepository;
        private readonly IAbpSession _abpSession;
        private readonly ClosingChecklistAppService _closingChecklistAppService;
        //private readonly CategoriesAppService _categoriesAppService;
        private readonly UserAppService _userAppService;
        private readonly IRepository<Category, long> _categoryRepository;
        public UserManager userManager { get; set; }
        public long UserId = 0;
        private readonly IInvalidClosingChecklistExporter _invalidClosingChecklistExporter;
        private readonly IImportPathsAppService _importPathsAppService;
        public long TenantId = 0;
        public int SuccessRecordsCount = 0;
        public long loggedFileId = 0;
        public ImportClosingChecklistToExcelJob(

        IClosingChecklistExcelDataReader closingChecklistExcelDataReader,
        IInvalidClosingChecklistExporter invalidClosingChecklistExporter,
        IAppNotifier appNotifier,
            IBinaryObjectManager binaryObjectManager,
            ILocalizationManager localizationManager,
            IImportPathsAppService importPathsAppService,
            IUnitOfWorkManager unitOfWorkManager,
            IAbpSession abpSession,
        IRepository<ClosingChecklist, long> closingChecklistRepository, IRepository<ImportsPath, long> importPathsRepository,
        IRepository<Category, long> categoryRepository,
        ClosingChecklistAppService closingChecklistAppService,
        CategoriesAppService categoriesAppService,
        UserAppService userAppService)
        {
            _closingChecklistExcelDataReader = closingChecklistExcelDataReader;
            _appNotifier = appNotifier;
            _binaryObjectManager = binaryObjectManager;
            _localizationSource = localizationManager.GetSource(ZinloConsts.LocalizationSourceName);
            _invalidClosingChecklistExporter = invalidClosingChecklistExporter;
            _importPathsAppService = importPathsAppService;
            _unitOfWorkManager = unitOfWorkManager;
            _closingChecklistRepository = closingChecklistRepository;
            _importPathsRepository = importPathsRepository;
            _abpSession = abpSession;
            _closingChecklistAppService = closingChecklistAppService;
            _userAppService = userAppService;
            _categoryRepository = categoryRepository;
        }

        [UnitOfWork]
        public override void Execute(ImportClosingChecklistFromExcelJobArgs args)
        {
            TenantId = (int)args.TenantId;
            UserId = args.User.UserId;
            using (CurrentUnitOfWork.SetTenantId(args.TenantId))
            {
                var closingChecklist = GetClosingChecklisttFromExcelOrNull(args);

                var fileUrl = _invalidClosingChecklistExporter.ExportToFile(closingChecklist);
                ImportPathDto pathDto = new ImportPathDto();
                pathDto.FilePath = "";
                pathDto.Type = FileTypes.ClosingChecklist.ToString();
                pathDto.TenantId = (int)TenantId;
                pathDto.UploadedFilePath = args.url;
                pathDto.CreatorId = UserId;
                pathDto.FailedRecordsCount = 0;
                pathDto.SuccessRecordsCount = 0;
                pathDto.SuccessFilePath = fileUrl;
                pathDto.UploadMonth = args.selectedMonth;
                pathDto.FileStatus = ImportPaths.Dto.Status.InProcess;
                loggedFileId = _importPathsAppService.SaveFilePath(pathDto);

                if (closingChecklist == null || !closingChecklist.Any())
                {
                    SendInvalidExcelNotification(args);
                    return;
                }
                AddClosingChecklist(args, closingChecklist);
            }

        }

        private List<ClosingChecklistExcellImportDto> GetClosingChecklisttFromExcelOrNull(ImportClosingChecklistFromExcelJobArgs args)
        {
            try
            {
                var file = AsyncHelper.RunSync(() => _binaryObjectManager.GetOrNullAsync(args.BinaryObjectId));

                var result = _closingChecklistExcelDataReader.GetClosingChecklistFromExcel(file.Bytes);
                return result;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        private void AddClosingChecklist(ImportClosingChecklistFromExcelJobArgs args, List<ClosingChecklistExcellImportDto> checklists)
        {
            var invalidChecklists = new List<ClosingChecklistExcellImportDto>();
            var list = new List<ClosingChecklistExcellImportDto>();

            #region|mgs to  be deleted|
            AsyncHelper.RunSync(() => _appNotifier.SendMessageAsync(
                  args.User,
                  "file logged",
                  Abp.Notifications.NotificationSeverity.Success));
            #endregion
            foreach (var checklist in checklists)
            {
                if (checklist.CanBeImported())
                {
                    try
                    {
                        var result = CheckNullValues(checklist);
                        if (result.isTrue)
                        {
                            checklist.Exception = result.Exception;
                            invalidChecklists.Add(checklist);
                        }
                        else
                        {
                            list.Add(checklist);
                        }

                    }

                    catch (UserFriendlyException exception)
                    {
                        checklist.Exception = exception.Message;
                        invalidChecklists.Add(checklist);
                    }

                }
                else
                {
                    invalidChecklists.Add(checklist);
                }
            }

            SuccessRecordsCount = list.Count;
            #region|Log intial info|

            var pathDtoUpdate = new ImportPathDto
            {
                Id = loggedFileId,
                FilePath = "",
                Type = FileTypes.ClosingChecklist.ToString(),
                TenantId = (int)TenantId,
                UploadedFilePath = args.url,
                CreatorId = UserId,
                FailedRecordsCount = 0,
                SuccessRecordsCount = 0,
                FileStatus = ImportPaths.Dto.Status.InProcess,
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
                CreateClosingChecklistAsync(item, args.selectedMonth);

            }
            //unitOfWork.Complete();
            //}
            ProcessImportClosingChecklistResultAsync(args, invalidChecklists);
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
        private void AddClosingChecklist(ClosingChecklistExcellImportDto input)
        {
            var frequency = new FrequencyDto();
            if (input.Frequency == "Monthly")
                frequency = FrequencyDto.Monthly;
            else if (input.Frequency == "Quarterly")
                frequency = FrequencyDto.Quarterly;
            else if (input.Frequency == "Annually")
                frequency = FrequencyDto.Annually;
            else if (input.Frequency == "XNumberOfMonths")
                frequency = FrequencyDto.XNumberOfMonths;
            else if (input.Frequency == "None")
                frequency = FrequencyDto.None;

            var dayBeforeAfter = new DaysBeforeAfterDto();
            if (input.DayBeforeAfter == "None")
                dayBeforeAfter = DaysBeforeAfterDto.None;
            else if (input.DayBeforeAfter == "DaysBefore")
                dayBeforeAfter = DaysBeforeAfterDto.DaysBefore;
            else if (input.DayBeforeAfter == "DaysAfter")
                dayBeforeAfter = DaysBeforeAfterDto.DaysAfter;

            var status = new StatusDto();
            if (input.Status == "NotStarted")
                status = StatusDto.NotStarted;
            else if (input.Status == "InProcess")
                status = StatusDto.InProcess;
            else if (input.Status == "OnHold")
                status = StatusDto.OnHold;
            else if (input.Status == "Completed")
                status = StatusDto.Completed;


            var closingchecklist = new CreateOrEditClosingChecklistDto
            {
                CategoryId = _categoryRepository.FirstOrDefault(x => x.Title == input.CategoryName).Id,
                TaskName = input.TaskName,
                AssigneeId = _userAppService.GetUserIdByUserEmail(input.AssigneeEmail),
                Frequency = frequency,
                NoOfMonths = input.NoOfMonths,
                DueOn = input.DueOn,
                EndOfMonth = input.EndOfMonth,
                ClosingMonth = input.selectedMonth,
                DayBeforeAfter = dayBeforeAfter,
                Status = status,
                Instruction = input.Instruction,
                TenantId = Convert.ToInt32(TenantId)
                
            };

            AsyncHelper.RunSync(() => _closingChecklistAppService.CreateOrEdit(closingchecklist));
        }

        private void CreateClosingChecklistAsync(ClosingChecklistExcellImportDto input, DateTime selectedMonth)
        {
            var output = new ClosingChecklistExcellImportDto
            {
                CategoryName = input.CategoryName,
                TaskName = input.TaskName,
                AssigneeEmail = input.AssigneeEmail,
                Frequency = input.Frequency,
                NoOfMonths = input.NoOfMonths,
                DueOn = input.DueOn,
                DayBeforeAfter = input.DayBeforeAfter,
                Status = input.Status,
                EndOfMonth = input.EndOfMonth,
                Instruction = input.Instruction,
                selectedMonth = selectedMonth
            };
            AddClosingChecklist(output);
        }



        private void ProcessImportClosingChecklistResultAsync(ImportClosingChecklistFromExcelJobArgs args, List<ClosingChecklistExcellImportDto> invalidChecklists)
        {
            if (invalidChecklists.Any())
            {
                #region|Update log|tefile
                var url = _invalidClosingChecklistExporter.ExportToFile(invalidChecklists);
                ImportPathDto pathDto = new ImportPathDto();
                pathDto.Id = loggedFileId;
                pathDto.FilePath = url;
                pathDto.UploadedFilePath = args.url;
                pathDto.FailedRecordsCount = invalidChecklists.Count;
                pathDto.SuccessRecordsCount = SuccessRecordsCount;
                pathDto.UploadMonth = args.selectedMonth;
                pathDto.FileStatus = ImportPaths.Dto.Status.Completed;
                UpdateFilePath(pathDto);
                #endregion
                AsyncHelper.RunSync(() => _appNotifier.SendMessageAsync(
                      args.User,
                      _localizationSource.GetString("SomeClosingChecklistsSuccessfullyImportedFromExcel"),
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
                pathDto.FailedRecordsCount = invalidChecklists.Count;
                pathDto.SuccessRecordsCount = SuccessRecordsCount;
                pathDto.FileStatus = ImportPaths.Dto.Status.Completed;
                UpdateFilePath(pathDto);
                #endregion
                AsyncHelper.RunSync(() => _appNotifier.SendMessageAsync(
                        args.User,
                        _localizationSource.GetString("AllClosingChecklistSuccessfullyImportedFromExcel"),
                        Abp.Notifications.NotificationSeverity.Success));
            }
        }


        private void SendInvalidExcelNotification(ImportClosingChecklistFromExcelJobArgs args)
        {
                #region ||
                AsyncHelper.RunSync(() => _appNotifier.SendMessageAsync(
                       args.User,
                      _localizationSource.GetString("FileCantBeConvertedToClosingChecklist"),
                       Abp.Notifications.NotificationSeverity.Success));
                #endregion
        }

        #region|Helpers|      
        private ClosingChecklistValidationDto CheckNullValues(ClosingChecklistExcellImportDto input)
         {
            string errorMessage = string.Empty;
            bool isCategoryIsNull = false;
            bool isTaskNameIsNull = false;
            bool isAssigneeIsNull = false;
            bool isFrequencyIsNull = false;
            bool isNoOfMonthIsNull = false;
            bool isDueOnIsNull = false;
            bool isEndOfMonthIsNull = false;
            bool IsDayBeforeAfterIsNull = false;
            var result = new ClosingChecklistValidationDto();
            if (input.CategoryName == null)
            {
                isCategoryIsNull = true;
                errorMessage += "Category,";
                // return true;
            }
            else if (string.IsNullOrEmpty(input.TaskName))
            {
                isTaskNameIsNull = true;
                errorMessage += "TaskName,";
                // return true;
            }
            else if (input.AssigneeEmail == null)
            {
                isAssigneeIsNull = true;
                errorMessage += "Assignee";
                // return true;
            }
            else if (input.Frequency == null)
            {
                isFrequencyIsNull = true;
                errorMessage += "Frequency,";
                // return true;
            }
            else if (input.EndOfMonth == false)
            {
                if(input.DueOn == 0)
                {
                    isDueOnIsNull = true;
                    errorMessage += "DueOn,";
                }
                if(input.DayBeforeAfter == null)
                {
                    IsDayBeforeAfterIsNull = true;
                    errorMessage += "DayBeforeAfter,";
                }
                
                // return true;
            }
            else if(input.DueOn == 0)
            {
                if(input.EndOfMonth == false)
                {
                    isEndOfMonthIsNull = true;
                    errorMessage += "EndOfMonth,";
                }
            }
            else if(input.Frequency != "XNumberOfMonths")
            {
                if(input.NoOfMonths == 0)
                {
                    isNoOfMonthIsNull = true;
                    errorMessage += "NoOfMonths";
                }
            }

            if (isCategoryIsNull == true || isTaskNameIsNull == true || isAssigneeIsNull == true 
                || isFrequencyIsNull == true || isNoOfMonthIsNull == true || isDueOnIsNull == true 
                || isEndOfMonthIsNull == true || IsDayBeforeAfterIsNull)
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
