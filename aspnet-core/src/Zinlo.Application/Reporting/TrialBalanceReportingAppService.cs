using Abp.Application.Services.Dto;
using Abp.Collections.Extensions;
using Abp.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Zinlo.ErrorLog.Dto;
using Zinlo.ImportLog.Dto;
using Zinlo.Reporting.Dtos;
using System.Linq;
using System.Linq.Dynamic.Core;
using Abp.Linq.Extensions;
using System.Net;
using Abp.IO.Extensions;
using Abp.UI;
using Abp.Web.Models;
using Zinlo.ChartsofAccount.Importing;
using Zinlo.ChartsofAccount.Dtos;
using Zinlo.Dto;
using Zinlo.Reporting.Importing;

namespace Zinlo.Reporting
{
    public class TrialBalanceReportingAppService : ZinloAppServiceBase, ITrialBalanceReportingAppService
    {
        private readonly IRepository<ImportsPaths.ImportsPath, long> _importPathsRepository;
        private readonly IChartsOfAccontTrialBalanceListExcelDataReader _chartsOfAccontTrialBalanceListExcelDataReader;
        private readonly ITrialBalanceExporter _trialBalanceExporter;

        public TrialBalanceReportingAppService(ITrialBalanceExporter trialBalanceExporter, IRepository<ImportsPaths.ImportsPath, long> importPathsRepository, IChartsOfAccontTrialBalanceListExcelDataReader chartsOfAccontTrialBalanceListExcelDataReader)
        {
            _importPathsRepository = importPathsRepository;
            _chartsOfAccontTrialBalanceListExcelDataReader = chartsOfAccontTrialBalanceListExcelDataReader;
            _trialBalanceExporter = trialBalanceExporter;
        }

        public async Task<PagedResultDto<ImportLogForViewDto>> GetAll(GetAllImportLogInput input)
        {
            var query = _importPathsRepository.GetAll().Where(p => p.Type == "TrialBalance").Include(p => p.User)
                 .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false || e.FilePath.Contains(input.Filter));

            var pagedAndFilteredAccounts = query.OrderBy(input.Sorting ?? "CreationTime desc").PageBy(input);
            var totalCount = query.Count();
            var accountsList = from o in pagedAndFilteredAccounts.ToList()

                               select new ImportLogForViewDto()
                               {
                                   Id = o.Id,
                                   Type = o.Type,
                                   FilePath = o.UploadedFilePath,
                                   CreationTime = o.CreationTime,
                                   Records = o.SuccessRecordsCount + "/" + (o.FailedRecordsCount + o.SuccessRecordsCount),
                                   CreatedById = o.User.Id,
                                   IsRollBacked = o.IsRollBacked,
                                   SuccessFilePath = o.SuccessFilePath != "" ? o.SuccessFilePath : ""
                               };

            return new PagedResultDto<ImportLogForViewDto>(
               totalCount,
               accountsList.ToList()
           );

        }


        public async Task<List<GetTrialBalanceofSpecficMonth>> GetTrialBalancesofSpecficMonth(DateTime SelectedMonth)
        {
            var query = _importPathsRepository.GetAll().Where(p => p.Type == "TrialBalance" && SelectedMonth.Month == p.UploadMonth.Month && SelectedMonth.Year == p.UploadMonth.Year).ToList();

            var trialBalanceOfMonth = from o in query.ToList()
                                      select new GetTrialBalanceofSpecficMonth()
                                      {
                                          id = o.Id,
                                          CreationTime = o.CreationTime,
                                          Name = o.UploadedFilePath,
                                      };

            return trialBalanceOfMonth.ToList();

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

        public async Task<PagedResultDto<CompareTrialBalanceViewDto>> GetCompareTrialBalances(ComparingTrialBalanceInputDto input)
        {

            var FirstMonthFile = _importPathsRepository.FirstOrDefault(p => p.Id == input.FirstMonthId);
            var SecondMonthFile = _importPathsRepository.FirstOrDefault(p => p.Id == input.SecondMonthId);

            byte[] firstFileBytes = RequestToGetTheFile(FirstMonthFile.UploadedFilePath);
            var firstFileList = readDateFromBytesArray(firstFileBytes);

            byte[] secondFileBytes = RequestToGetTheFile(SecondMonthFile.UploadedFilePath);
            var secondFileList = readDateFromBytesArray(secondFileBytes);

           

                var ComparisonResult = from o in firstFileList.ToList()
                                          select new CompareTrialBalanceViewDto()
                                          {
                                              AccountName = o.AccountName,
                                              AccountNumber = o.AccountNumber,
                                              FirstMonthBalance = o.Balance,
                                              SecondMonthBalance = secondFileList.FirstOrDefault(p => p.AccountNumber == o.AccountNumber).Balance

                                          };

            var totalCount = ComparisonResult.Count();



            return new PagedResultDto<CompareTrialBalanceViewDto>(
           totalCount,
           ComparisonResult.ToList()
       );



        }

        public async Task<FileDto> GetInToExcel(List<CompareTrialBalanceViewDto> input, DateTime FirstMonth, DateTime SecondMonth)
        {
            string firstMonth = FirstMonth.ToString("MMM") + " " + FirstMonth.Year.ToString();
            string secondMonth = SecondMonth.ToString("MMM") + " " + SecondMonth.Year.ToString();
            return _trialBalanceExporter.ExportToFile(input, firstMonth, secondMonth);
        }
    }
}
 