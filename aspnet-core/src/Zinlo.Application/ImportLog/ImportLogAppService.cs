using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Net;
using System.Threading.Tasks;
using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using Abp.IO.Extensions;
using Abp.Linq.Extensions;
using Abp.UI;
using Microsoft.EntityFrameworkCore;
using Zinlo.ChartsofAccount.Dtos;
using Zinlo.ChartsofAccount.Importing;
using Zinlo.ErrorLog.Dto;
using Zinlo.ImportLog.Dto;
using Zinlo.ImportsPaths;

namespace Zinlo.ImportLog
{
    public class ImportLogAppService : ZinloAppServiceBase, IImportLogAppService
    {
        private readonly IRepository<ImportsPath, long> _importsPathRepository;
        private readonly IRepository<ChartofAccounts.ChartofAccounts, long> _chartOfAccountRepository;
        private readonly IChartsOfAccontTrialBalanceListExcelDataReader _chartsOfAccontTrialBalanceListExcelDataReader;
        private readonly IRepository<ChartofAccounts.AccountBalance, long> _accountBalanceRepositry;

        public ImportLogAppService(IRepository<ImportsPath, long> importsPathRepository,
            IRepository<ChartofAccounts.ChartofAccounts, long> chartOfAccountRepository,
            IChartsOfAccontTrialBalanceListExcelDataReader chartsOfAccontTrialBalanceListExcelDataReader,
            IRepository<ChartofAccounts.AccountBalance, long> accountBalanceRepositry)
        {
            _importsPathRepository = importsPathRepository;
            _chartOfAccountRepository = chartOfAccountRepository;
            _chartsOfAccontTrialBalanceListExcelDataReader = chartsOfAccontTrialBalanceListExcelDataReader;
            _accountBalanceRepositry = accountBalanceRepositry;
        }

        public async Task<PagedResultDto<ImportLogForViewDto>> GetAll(GetAllImportLogInput input)
        {
            var query = _importsPathRepository.GetAll().Include(p => p.User)
                 .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false || e.FilePath.Contains(input.Filter));

            var pagedAndFilteredAccounts = query.OrderBy(input.Sorting ?? "CreationTime desc").PageBy(input);
            var totalCount = query.Count();
            var accountsList = from o in pagedAndFilteredAccounts.ToList()

                               select new ImportLogForViewDto()
                               {
                                   Id = o.Id,
                                   Type = o.Type,
                                   FilePath = o.FilePath != "" ? o.FilePath : "",
                                   CreationTime = o.CreationTime,
                                   Records = o.SuccessRecordsCount + "/" + (o.FailedRecordsCount + o.SuccessRecordsCount),
                                   CreatedBy = o.User.EmailAddress,
                                   IsRollBacked = o.IsRollBacked,
                                   SuccessFilePath = o.SuccessFilePath != ""? o.SuccessFilePath: ""
                               };

            return new PagedResultDto<ImportLogForViewDto>(
               totalCount,
               accountsList.ToList()
           );

        }

        public async Task RollBackTrialBalance(long id)
        {
            var result = _importsPathRepository.FirstOrDefault(p => p.Id == id);
            byte[] FileBytes = RequestToGetTheFile(result.UploadedFilePath);
            var FileList = readDateFromBytesArray(FileBytes);
            var RollBackFileList = FileList.ToList();
            var accounts =  _chartOfAccountRepository.GetAll();
            var accountBalanceInformation = _accountBalanceRepositry.GetAll();
            foreach (var item in RollBackFileList)
            {
                var itemAccount = accounts.Where(p => p.AccountNumber == item.AccountNumber).ToList();
                var itemAccountBalanceInfo = accountBalanceInformation.FirstOrDefault(p => p.AccountId == itemAccount[0].Id && result.UploadMonth.Month == p.Month.Month && result.UploadMonth.Year == p.Month.Year);
                itemAccountBalanceInfo.TrialBalance = long.Parse(item.Balance);
                _accountBalanceRepositry.Update(itemAccountBalanceInfo);
            }
            result.IsRollBacked = true;

            _importsPathRepository.Update(result);

            var remainingFile =  _importsPathRepository.GetAll().Where(p => p.Id != id && p.UploadMonth.Month == result.UploadMonth.Month && p.UploadMonth.Year == result.UploadMonth.Year).ToList();
           if (remainingFile.Count > 0)
            {
                foreach (var item in remainingFile)
                {
                    item.IsRollBacked = false;
                    _importsPathRepository.Update(item);
                }
            }
            

        }

        protected virtual byte[] RequestToGetTheFile(string url)
        {
            try
            {
                WebRequest request = WebRequest.Create(url);
                byte[] fileBytes;
                using (var response = request.GetResponse())
                using (var stream = response.GetResponseStream())
                {
                    fileBytes = stream.GetAllBytes();
                }

                return fileBytes;
            }
            catch (UserFriendlyException ex)
            {
                throw new UserFriendlyException(L(ex.ToString()));
            }

        }

        protected virtual List<ChartsOfAccountsTrialBalanceExcellImportDto> readDateFromBytesArray(byte[] file)
        {
            try
            {
                var result = _chartsOfAccontTrialBalanceListExcelDataReader.GetAccountsTrialBalanceFromExcel(file);
                return result;
            }
            catch (UserFriendlyException ex)
            {
                throw new UserFriendlyException(L(ex.ToString()));
            }

        }
    }
}
