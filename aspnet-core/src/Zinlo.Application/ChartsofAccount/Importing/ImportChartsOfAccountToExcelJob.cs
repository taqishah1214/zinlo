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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zinlo.AccountSubType;
using Zinlo.Authorization.Roles;
using Zinlo.Authorization.Users;
using Zinlo.ChartsofAccount.Dtos;
using Zinlo.ChartsofAccount.Importing;
using Zinlo.Notifications;
using Zinlo.Storage;
namespace Zinlo.ChartsofAccount
{
   
    public class ImportChartsOfAccountToExcelJob : BackgroundJob<ImportChartsOfAccountFromExcelJobArgs>, ITransientDependency
    {
              
       private readonly IChartsOfAccontListExcelDataReader _chartsOfAccontListExcelDataReader;
        private readonly IAccountSubTypeAppService _accountSubTypeAppService;
        private readonly IInvalidAccountsExcellExporter _invalidAccountsExporter;
        private readonly IRepository<ChartsofAccount, long>  _chartsOfAccountsrepository;
        private readonly IAppNotifier _appNotifier;
        private readonly IBinaryObjectManager _binaryObjectManager;
        private readonly ILocalizationSource _localizationSource;
        private readonly IObjectMapper _objectMapper;

        public UserManager userManager { get; set; }

        public ImportChartsOfAccountToExcelJob(

        IChartsOfAccontListExcelDataReader chartsOfAccontListExcelDataReader,
            IAccountSubTypeAppService accountSubTypeAppService,
            IRepository<ChartsofAccount, long> chartsOfAccountsrepository,
            IInvalidAccountsExcellExporter invalidAccountsExporter,
             IAppNotifier appNotifier,
            IBinaryObjectManager binaryObjectManager,
            ILocalizationManager localizationManager,
            IObjectMapper objectMapper
            
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
        }

        [UnitOfWork]
        public override void Execute(ImportChartsOfAccountFromExcelJobArgs args)
        {
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

              foreach (var account in accounts)
              {
                  if (account.CanBeImported())
                  {
                      try
                      {
                          AsyncHelper.RunSync(() => CreateChartsOfAccountAsync(account));
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
                AsyncHelper.RunSync(() => CreateChartsOfAccountAsync(item));
            }           
            AsyncHelper.RunSync(() => ProcessImportAccountsResultAsync(args, invalidAccounts));
        }

        private async Task CreateChartsOfAccountAsync(ChartsOfAccountsExcellImportDto input)
        {
            var tenantId = CurrentUnitOfWork.GetTenantId();        
            ChartsofAccount account = new ChartsofAccount();
            account.TenantId = (int)tenantId;
            account.AccountName = input.AccountName;
            account.AccountNumber = input.AccountNumber;
            account.CreationTime = DateTime.Now.ToUniversalTime();
            account.Status = (Status)2;
            account.AssigneeId = await GetUserIdByEmail(input.AssignedUser);
            account.CreatorUserId = 2;

            account.AccountType =  (AccountType)GetAccountTypeValue(input.AccountType);
            account.AccountSubTypeId = await _accountSubTypeAppService.GetAccountSubTypeIdByTitle(input.AccountSubType);
            account.Reconciled = (Reconciled)1;   //GetReconciledValue(input.);
            account.ReconciliationType = (ReconciliationType)1;
            await _chartsOfAccountsrepository.InsertAsync(account);                    
        }

        private async Task ProcessImportAccountsResultAsync(ImportChartsOfAccountFromExcelJobArgs args, List<ChartsOfAccountsExcellImportDto> invalidAccounts)
        {
            await _appNotifier.SendMessageAsync(
                   args.User,
                   _localizationSource.GetString("AllAccountsSuccessfullyImportedFromExcel"),
                   Abp.Notifications.NotificationSeverity.Success);
            

            if (invalidAccounts.Any())
            {
                 var file = _invalidAccountsExporter.ExportToFile(invalidAccounts);
                await _appNotifier.SomeUsersCouldntBeImported(args.User, file.FileToken, file.FileType, file.FileName);
            }
            else
            {
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
            if(user != null)
            {
                return user.Id;
            }
            else
            {
                return 2;
            }
           
        }

        public int GetReconciliationTypeValue(string name)
        {
            if(name.Trim().ToLower() == "itemized")
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
            else
            {
                return 3;
            }
        }
        #endregion
    }

}
