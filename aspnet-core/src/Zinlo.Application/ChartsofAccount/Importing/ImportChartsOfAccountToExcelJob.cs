using Abp.BackgroundJobs;
using Abp.Dependency;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Localization;
using Abp.Localization.Sources;
using Abp.ObjectMapping;
using Abp.Threading;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zinlo.Authorization.Roles;
using Zinlo.ChartsofAccount.Dtos;
using Zinlo.ChartsofAccount.Importing;
using Zinlo.Notifications;
using Zinlo.Storage;

namespace Zinlo.ChartsofAccount
{
   
    public class ImportChartsOfAccountToExcelJob : BackgroundJob<ImportChartsOfAccountFromExcelJobArgs>, ITransientDependency
    {
        private readonly RoleManager _roleManager;
       private readonly IChartsOfAccontListExcelDataReader _chartsOfAccontListExcelDataReader;
        // private readonly IInvalidUserExporter _invalidUserExporter;
        // private readonly IUserPolicy _userPolicy;
        // private readonly IEnumerable<IPasswordValidator<User>> _passwordValidators;
        // private readonly IPasswordHasher<User> _passwordHasher;
        private readonly IRepository<ChartsofAccount, long>  _chartsOfAccountsrepository;
        private readonly IAppNotifier _appNotifier;
        private readonly IBinaryObjectManager _binaryObjectManager;
        private readonly ILocalizationSource _localizationSource;
        private readonly IObjectMapper _objectMapper;

      //  public UserManager UserManager { get; set; }

        public ImportChartsOfAccountToExcelJob(
            //RoleManager roleManager,
            IChartsOfAccontListExcelDataReader chartsOfAccontListExcelDataReader,
            IRepository<ChartsofAccount, long> chartsOfAccountsrepository,
            //IInvalidUserExporter invalidUserExporter,
            //IUserPolicy userPolicy,
            //IEnumerable<IPasswordValidator<User>> passwordValidators,
            //IPasswordHasher<User> passwordHasher,
             IAppNotifier appNotifier,
            IBinaryObjectManager binaryObjectManager,
            ILocalizationManager localizationManager,
            IObjectMapper objectMapper)
        {
            //_roleManager = roleManager;
            _chartsOfAccontListExcelDataReader = chartsOfAccontListExcelDataReader;
            _chartsOfAccountsrepository = chartsOfAccountsrepository;
            //_invalidUserExporter = invalidUserExporter;
            //_userPolicy = userPolicy;
            //_passwordValidators = passwordValidators;
            //_passwordHasher = passwordHasher;
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
                var chartsofaccount = GetUserListFromExcelOrNull(args);
                if (chartsofaccount == null || !chartsofaccount.Any())
                {
                    //SendInvalidExcelNotification(args);
                    return;
                }

                CreateUsers(args, chartsofaccount);
            }
        }

        private List<CreateOrEditChartsofAccountDto> GetUserListFromExcelOrNull(ImportChartsOfAccountFromExcelJobArgs args)
        {
            try
            {
                var file = AsyncHelper.RunSync(() => _binaryObjectManager.GetOrNullAsync(args.BinaryObjectId));
                return _chartsOfAccontListExcelDataReader.GetUsersFromExcel(file.Bytes);
            }
            catch (Exception)
            {
                return null;
            }
        }

        private void CreateUsers(ImportChartsOfAccountFromExcelJobArgs args, List<CreateOrEditChartsofAccountDto> users)
        {
            var invalidUsers = new List<CreateOrEditChartsofAccountDto>();

            /*  foreach (var user in users)
              {
                  if (user.CanBeImported())
                  {
                      try
                      {
                          AsyncHelper.RunSync(() => CreateUserAsync(user));
                      }
                      catch (UserFriendlyException exception)
                      {
                          user.Exception = exception.Message;
                          invalidUsers.Add(user);
                      }
                      catch (Exception exception)
                      {
                          user.Exception = exception.ToString();
                          invalidUsers.Add(user);
                      }
                  }
                  else
                  {
                      invalidUsers.Add(user);
                  }
              }*/
            foreach (var item in users)
            {
                AsyncHelper.RunSync(() => CreateUserAsync(item));
            }
            

            
            AsyncHelper.RunSync(() => ProcessImportUsersResultAsync(args, invalidUsers));
        }

        private async Task CreateUserAsync(CreateOrEditChartsofAccountDto input)
        {
            var tenantId = CurrentUnitOfWork.GetTenantId();

            //if (tenantId.HasValue)
            //{
            //    await _userPolicy.CheckMaxUserCountAsync(tenantId.Value);
            //}

            var account = _objectMapper.Map<ChartsofAccount>(input); //Passwords is not mapped (see mapping configuration)
            account.TenantId = (int)tenantId;
            await _chartsOfAccountsrepository.InsertAsync(account);
            //user.Password = input.Password;
            //user.TenantId = tenantId;

            //if (!input.Password.IsNullOrEmpty())
            //{
            //    await UserManager.InitializeOptionsAsync(tenantId);
            //    foreach (var validator in _passwordValidators)
            //    {
            //        (await validator.ValidateAsync(UserManager, user, input.Password)).CheckErrors();
            //    }

            //    user.Password = _passwordHasher.HashPassword(user, input.Password);
            //}

            //user.Roles = new List<UserRole>();
            //var roleList = _roleManager.Roles.ToList();

            //foreach (var roleName in input.AssignedRoleNames)
            //{
            //    var correspondingRoleName = GetRoleNameFromDisplayName(roleName, roleList);
            //    var role = await _roleManager.GetRoleByNameAsync(correspondingRoleName);
            //    user.Roles.Add(new UserRole(tenantId, user.Id, role.Id));
            //}

            //(await UserManager.CreateAsync(user)).CheckErrors();
        }

        private async Task ProcessImportUsersResultAsync(ImportChartsOfAccountFromExcelJobArgs args, List<CreateOrEditChartsofAccountDto> invalidUsers)
        {
            //await _appNotifier.SendMessageAsync(
            //       args.Account,
            //       _localizationSource.GetString("AllUsersSuccessfullyImportedFromExcel"),
            //       Abp.Notifications.NotificationSeverity.Success);


            //if (invalidUsers.Any())
            //{
            //   // var file = _invalidUserExporter.ExportToFile(invalidUsers);
            //    await _appNotifier.SomeUsersCouldntBeImported(args.Account, file.FileToken, file.FileType, file.FileName);
            //}
            //else
            //{
            //    await _appNotifier.SendMessageAsync(
            //        args.Account,
            //        _localizationSource.GetString("AllUsersSuccessfullyImportedFromExcel"),
            //        Abp.Notifications.NotificationSeverity.Success);
            //}
        }

        //private void SendInvalidExcelNotification(ImportChartsOfAccountFromExcelJobArgs args)
        //{
        //    AsyncHelper.RunSync(() => _appNotifier.SendMessageAsync(
        //        args.User,
        //        _localizationSource.GetString("FileCantBeConvertedToUserList"),
        //        Abp.Notifications.NotificationSeverity.Warn));
        //}

        //private string GetRoleNameFromDisplayName(string displayName, List<Role> roleList)
        //{
        //    return roleList.FirstOrDefault(
        //                r => r.DisplayName?.ToLowerInvariant() == displayName?.ToLowerInvariant()
        //            )?.Name;
        //}
    }

}
