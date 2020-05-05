using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using Abp.IO.Extensions;
using Abp.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Zinlo.ChartofAccounts;
using Zinlo.ChartsofAccount.Dtos;
using Zinlo.ChartsofAccount.Importing;
using Zinlo.Dto;
using Zinlo.ErrorLog.Dto;
using Zinlo.ImportLog.Dto;
using Zinlo.Reporting.Dtos;
using Zinlo.Reporting.Importing;

namespace Zinlo.Reporting
{
    public class CompareVarianceAppService : ZinloAppServiceBase, ICompareVarianceAppService
    {
        private readonly IRepository<ImportsPaths.ImportsPath, long> _importPathsRepository;
        private readonly IChartsOfAccontTrialBalanceListExcelDataReader _chartsOfAccontTrialBalanceListExcelDataReader;
        private readonly ICompareVarianceExporter _compareVarianceExporter;
        private readonly IRepository<ChartofAccounts.ChartofAccounts, long> _chartsofAccountRepository;
        private readonly IRepository<AccountBalance, long> _accountBalanceRepository;



        public CompareVarianceAppService(ICompareVarianceExporter compareVarianceExporter, IRepository<ImportsPaths.ImportsPath, long> importPathsRepository, IChartsOfAccontTrialBalanceListExcelDataReader chartsOfAccontTrialBalanceListExcelDataReader, IRepository<ChartofAccounts.ChartofAccounts, long> chartsofAccountRepository, IRepository<AccountBalance, long> accountBalanceRepository)
        {
            _importPathsRepository = importPathsRepository;
            _chartsOfAccontTrialBalanceListExcelDataReader = chartsOfAccontTrialBalanceListExcelDataReader;
            _compareVarianceExporter = compareVarianceExporter;
            _chartsofAccountRepository = chartsofAccountRepository;
            _accountBalanceRepository = accountBalanceRepository;

        }

        public async Task<PagedResultDto<CompareVarianceViewDto>> GetCompareTrialBalances(CompareVarianceInputDto input)
        {

            var FirstMonthFile = _importPathsRepository.FirstOrDefault(p => p.Id == input.FirstMonthId);
            var SecondMonthFile = _importPathsRepository.FirstOrDefault(p => p.Id == input.SecondMonthId);

            byte[] firstFileBytes = RequestToGetTheFile(FirstMonthFile.UploadedFilePath);
            var firstFileList = readDateFromBytesArray(firstFileBytes);

            byte[] secondFileBytes = RequestToGetTheFile(SecondMonthFile.UploadedFilePath);
            var secondFileList = readDateFromBytesArray(secondFileBytes);

            var AllAccounts = _chartsofAccountRepository.GetAll();
            List<SingleMonthVariance> firstMonthVariance = new List<SingleMonthVariance>();

            foreach (var item in firstFileList)
            {
                SingleMonthVariance obj = new SingleMonthVariance();
                var account = AllAccounts.Where(p => item.AccountNumber == p.AccountNumber).ToList();
                var accountBalanceOnSpecficMonth = _accountBalanceRepository.FirstOrDefault(p=> account[0].Id == p.AccountId && FirstMonthFile.UploadMonth.Month == p.Month.Month && FirstMonthFile.UploadMonth.Year == p.Month.Year).Balance;
                var variance = accountBalanceOnSpecficMonth - Convert.ToInt64(item.Balance);
                obj.AccountNumber = account[0].AccountNumber;
                obj.Variance = variance;
                firstMonthVariance.Add(obj);
            }

            List<SingleMonthVariance> secondMonthVariance = new List<SingleMonthVariance>();
            foreach (var item in secondFileList)
            {
                SingleMonthVariance obj = new SingleMonthVariance();
                var account = AllAccounts.Where(p => item.AccountNumber == p.AccountNumber).ToList();
                var accountBalanceOnSpecficMonth = _accountBalanceRepository.FirstOrDefault(p => account[0].Id == p.AccountId && SecondMonthFile.UploadMonth.Month == p.Month.Month && SecondMonthFile.UploadMonth.Year == p.Month.Year).Balance;
                var variance = accountBalanceOnSpecficMonth - Convert.ToInt64(item.Balance);
                obj.AccountNumber = account[0].AccountNumber;
                obj.Variance = variance;
                secondMonthVariance.Add(obj);
            }

            var ComparisonResult = from o in firstMonthVariance.ToList()
                                   select new CompareVarianceViewDto()
                                   {
                                       AccountNumber = o.AccountNumber,
                                       AccountName = AllAccounts.FirstOrDefault(p => o.AccountNumber == p.AccountNumber).AccountName,
                                       FirstMonthVariance = o.Variance,
                                       SecondMonthVariance = secondMonthVariance.FirstOrDefault(p => p.AccountNumber == o.AccountNumber).Variance
                  
                                   };

            var totalCount = ComparisonResult.Count();

            return new PagedResultDto<CompareVarianceViewDto>(
                totalCount,
                ComparisonResult.ToList()
                );

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
        public async Task<FileDto> GetInToExcel(List<CompareVarianceViewDto> input, DateTime FirstMonth, DateTime SecondMonth)
        {
            string firstMonth = FirstMonth.ToString("MMM") +" "+FirstMonth.Year.ToString();
            string secondMonth = SecondMonth.ToString("MMM") + " "+SecondMonth.Year.ToString();
            return _compareVarianceExporter.ExportToFile(input, firstMonth, secondMonth);
        }
    }
}
