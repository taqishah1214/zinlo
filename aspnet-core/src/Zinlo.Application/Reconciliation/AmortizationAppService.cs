using Abp.Application.Services.Dto;
using Abp.Collections.Extensions;
using Abp.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zinlo.Reconciliation.Dtos;
using System.Linq.Dynamic.Core;
using Abp.Linq.Extensions;
using Microsoft.EntityFrameworkCore;
using Zinlo.Attachments.Dtos;
using Zinlo.Attachments;
using Zinlo.ChartsofAccount;

namespace Zinlo.Reconciliation
{
   public class AmortizationAppService : ZinloAppServiceBase, IAmortizationAppService
    {
        private readonly IRepository<Amortization, long> _amortizationRepository;
        private readonly IAttachmentAppService _attachmentAppService;
        private readonly IChartsofAccountAppService _chartsofAccountAppService;

        #region|#Constructor|
        public AmortizationAppService(IChartsofAccountAppService chartsofAccountAppService, IRepository<Amortization, long> amortizationRepository,IAttachmentAppService attachmentAppService)
        {
            _attachmentAppService = attachmentAppService;
            _amortizationRepository = amortizationRepository;
            _chartsofAccountAppService = chartsofAccountAppService;
        }
        #endregion
        #region|Create Edit|
        public async Task CreateOrEdit(CreateOrEditAmortizationDto input)
        {
            if (input.Id == 0)
            {
                await Create(input);
            }
            else
            {
                await Update(input);
            }
        }

        public async Task Delete(long Id)
        {
            await _amortizationRepository.DeleteAsync(Id);
        }

        public async Task<PagedResultDto<AmortizedListDto>> GetAll(GetAllAmortizationInput input)
        {
            var query = _amortizationRepository.GetAll().Where(e => e.ClosingMonth.Month == input.MonthFilter.Month && e.ClosingMonth.Year == input.MonthFilter.Year)
               .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false || e.Description.Contains(input.Filter))
               .WhereIf((input.ChartofAccountId != 0), e => false || e.ChartsofAccountId == input.ChartofAccountId)
               .WhereIf(!string.IsNullOrWhiteSpace(input.AccountNumer), e => false || e.ChartsofAccount.AccountNumber == input.AccountNumer);


            var pagedAndFilteredItems = query.OrderBy(input.Sorting ?? "id asc").PageBy(input);
            var totalCount = query.Count();
           var amortizedList = (from o in pagedAndFilteredItems.ToList()

                                select new AmortizedListForViewDto()
                                {
                                    Id = o.Id,
                                    StartDate = o.StartDate,
                                    EndDate = o.EndDate,
                                    Description = o.Description,
                                    AccuredAmortization = CalculateAccuredAmount((int)(o.Criteria), o.AccomulateAmount, o.Amount,o.EndDate, input.MonthFilter,o.StartDate),
                                    BeginningAmount = o.Amount,
                                    NetAmount = CalculateNetAmount((int)(o.Criteria) == 2 || (int)(o.Criteria) == 3 ? CalculateAccuredAmount((int)(o.Criteria), o.AccomulateAmount, o.Amount, o.EndDate, input.MonthFilter, o.StartDate): o.AccomulateAmount, o.Amount) ,
                                    Attachments = _attachmentAppService.GetAttachmentsPath(o.Id, 2).Result        
                               }).ToList();


            List<AmortizedListDto> result = new List <AmortizedListDto>();

            AmortizedListDto amortizedListDto = new AmortizedListDto
            {
                amortizedListForViewDtos = amortizedList,
                TotalBeginningAmount = amortizedList.Sum(item => item.BeginningAmount),
                TotalAccuredAmortization = amortizedList.Sum(item => item.AccuredAmortization),
                TotalNetAmount = amortizedList.Sum(item => item.NetAmount)
            };

            amortizedListDto.TotalTrialBalance = await _chartsofAccountAppService.GetTrialBalanceofAccount(input.ChartofAccountId);



            if (input.ChartofAccountId != 0 )
            {
                var reconcilledResult = await _chartsofAccountAppService.CheckReconcilled(input.ChartofAccountId);
                amortizedListDto.ReconciliedBase = reconcilledResult;
                switch (reconcilledResult)
                {
                    case 1:
                        await _chartsofAccountAppService.AddandUpdateBalance(amortizedListDto.TotalNetAmount, input.ChartofAccountId);
                        amortizedListDto.VarianceNetAmount = amortizedListDto.TotalNetAmount - amortizedListDto.TotalTrialBalance;
                        amortizedListDto.VarianceBeginningAmount = 0;
                        amortizedListDto.VarianceAccuredAmount = 0;
                        break;
                    case 2:
                        await _chartsofAccountAppService.AddandUpdateBalance(amortizedListDto.TotalBeginningAmount, input.ChartofAccountId);
                        amortizedListDto.VarianceBeginningAmount = amortizedListDto.TotalBeginningAmount - amortizedListDto.TotalTrialBalance;
                        amortizedListDto.VarianceAccuredAmount = amortizedListDto.TotalAccuredAmortization - amortizedListDto.TotalTrialBalance;
                        break;
                    case 3:
                        await _chartsofAccountAppService.AddandUpdateBalance(amortizedListDto.TotalAccuredAmortization, input.ChartofAccountId);
                        amortizedListDto.VarianceBeginningAmount = amortizedListDto.TotalBeginningAmount - amortizedListDto.TotalTrialBalance;
                        amortizedListDto.VarianceAccuredAmount = amortizedListDto.TotalAccuredAmortization - amortizedListDto.TotalTrialBalance;
                        break;

                    default:
                        break;
                }      
            }

        result.Add(amortizedListDto);
            return new PagedResultDto<AmortizedListDto>(
               totalCount,
               result
           );
        }

        protected virtual double CalculateNetAmount(double AccomulateAmount, double BeginningAmount)
        {
            double Result = 0;
            Result =  BeginningAmount - AccomulateAmount;
            return Result;
        }

        protected virtual double CalculateAccuredAmount( int CriteriaId , double AccomulateAmount, double BeginningAmount,DateTime EndDate, DateTime Current, DateTime StartDate)
        {
            double Result =1;
            switch (CriteriaId)
            {
                case 1:
                    Result = AccomulateAmount;
                    break;
                case 2:
                    Result = GetAccuredAmountMonthly(AccomulateAmount, BeginningAmount, EndDate, Current, StartDate);
                    break;
                case 3:
                    Result = GetAccuredAmountDaily(AccomulateAmount, BeginningAmount, EndDate, Current, StartDate);
                    break;

                default:
                    Result = 1;
                    break;
            }
            return Result;
        }

        protected virtual double  GetAccuredAmountMonthly(double AccomulateAmount, double BeginningAmount , DateTime EndDate, DateTime Current, DateTime StartDate)
        {
            DateTime MonthEnd =  GetValidDate(Current);
            if (MonthEnd >= EndDate)
            {
                return BeginningAmount;
            }
            else
            {
                // Year of MonthEnd - Year of StartDate) 12 + (Month of MonthEnd -Month of StartDate +1)  Original Amount
                double Year = MonthEnd.Year - StartDate.Year;
                double Month = MonthEnd.Month - StartDate.Month;
                double EndMonth = EndDate.Month - StartDate.Month;
                double EndYear = EndDate.Year - StartDate.Year;
                double Result1 = (Year * 12) + (Month + 1);
                double Result2 = (EndYear * 12) + (EndMonth + 1);
                double Result3 = (Result1/Result2)*BeginningAmount;
                return Result3;
            }
        }
        protected virtual double GetAccuredAmountDaily(double AccomulateAmount, double beginningAmount, DateTime EndDate, DateTime Current, DateTime StartDate)
        {

            DateTime MonthEnd = GetValidDate(Current);
            int DateResult = CompareDates(EndDate, MonthEnd);
            if (MonthEnd >= EndDate)
            {
                return beginningAmount;
            }
            else
            {
                //(MonthEnd.Date - StartDate.Date)
                //(MonthEnd - StartDate +1) / (EndDate - StartDate +1) * Original Amount
                double month1 = (MonthEnd.Date.Subtract(StartDate.Date).Days)+1;
                double month2 = (EndDate.Date.Subtract(StartDate.Date).Days)+1 ;
                double divideMonths = (month1 / month2);
                double result = divideMonths * beginningAmount;
                return result;
            }
           
        }

        public DateTime GetValidDate(DateTime dateTime)
        {
                var totalDays = DateTime.DaysInMonth(dateTime.Year, dateTime.Month);
                DateTime LastDaydateTime = new DateTime(dateTime.Year, dateTime.Month, totalDays, 0, 0, 0);
                return LastDaydateTime;
        }

        public int CompareDates(DateTime EndDate,DateTime MonthEnd)
        {
            DateTime date1 = new DateTime(EndDate.Year, EndDate.Month, EndDate.Day, 0, 0, 0);
            DateTime date2 = new DateTime(MonthEnd.Year, MonthEnd.Month, MonthEnd.Day, 0, 0, 0);
            int result = DateTime.Compare(date2, date1);
            return result;
        }

       


        protected virtual async Task Create(CreateOrEditAmortizationDto input)
        {            
            var item = ObjectMapper.Map<Amortization>(input);
            var itemAddedId = await _amortizationRepository.InsertAndGetIdAsync(item);
            if (input.AttachmentsPath != null)
            {
                PostAttachmentsPathDto postAttachmentsPathDto = new PostAttachmentsPathDto();
                postAttachmentsPathDto.FilePath = input.AttachmentsPath;
                postAttachmentsPathDto.TypeId = itemAddedId;
                postAttachmentsPathDto.Type = 2;
                await _attachmentAppService.PostAttachmentsPath(postAttachmentsPathDto);
            }
          

        }
        protected virtual async Task Update(CreateOrEditAmortizationDto input)
        {
            var item = await _amortizationRepository.FirstOrDefaultAsync(input.Id);
            var data = ObjectMapper.Map(input, item);
            await _amortizationRepository.UpdateAsync(data);
            if (input.AttachmentsPath != null)
            {
                PostAttachmentsPathDto postAttachmentsPathDto = new PostAttachmentsPathDto();
                postAttachmentsPathDto.FilePath = input.AttachmentsPath;
                postAttachmentsPathDto.TypeId = input.Id;
                postAttachmentsPathDto.Type = 2;
                await _attachmentAppService.PostAttachmentsPath(postAttachmentsPathDto);
            }
        }

        public async Task<CreateOrEditAmortizationDto> GetAmortizedItemDetails(long Id)
        {
            CreateOrEditAmortizationDto ItemData = new CreateOrEditAmortizationDto();
            var item = await _amortizationRepository.FirstOrDefaultAsync(x => x.Id == Id);
            ItemData.Attachments = await _attachmentAppService.GetAttachmentsPath(Id, 2);
            var data = ObjectMapper.Map(item, ItemData);
            return data;
        }

       



        #endregion

    }
}
