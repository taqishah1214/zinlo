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
using Zinlo.Comment;
using Zinlo.Comment.Dtos;
using Abp.Domain.Uow;
using Zinlo.TimeManagements;

namespace Zinlo.Reconciliation
{
   public class AmortizationAppService : ZinloAppServiceBase, IAmortizationAppService
    {
        private readonly IRepository<Amortization, long> _amortizationRepository;
        private readonly IAttachmentAppService _attachmentAppService;
        private readonly IChartsofAccountAppService _chartsofAccountAppService;
        private readonly ICommentAppService _commentAppService;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly ITimeManagementsAppService _timeManagementsAppService;
        private readonly IRepository<ReconciliationAmounts, long> _reconciliationAmountsRepository;


        #region|#Constructor|
        public AmortizationAppService(ICommentAppService commentAppService,IChartsofAccountAppService chartsofAccountAppService, IRepository<Amortization, long> amortizationRepository,
            IAttachmentAppService attachmentAppService,
            IUnitOfWorkManager unitOfWorkManager,
            ITimeManagementsAppService timeManagementsAppService,
            IRepository<ReconciliationAmounts, long> reconciliationAmountsRepository)
        {
            _attachmentAppService = attachmentAppService;
            _amortizationRepository = amortizationRepository;
            _chartsofAccountAppService = chartsofAccountAppService;
            _commentAppService = commentAppService;
            _unitOfWorkManager = unitOfWorkManager;
            _timeManagementsAppService = timeManagementsAppService;
            _reconciliationAmountsRepository = reconciliationAmountsRepository;


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
            using (_unitOfWorkManager.Current.DisableFilter(AbpDataFilters.SoftDelete))
            {
                var query = _amortizationRepository.GetAll()
               .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false || e.Description.Contains(input.Filter))
               .WhereIf((input.ChartofAccountId != 0), e => false || e.ChartsofAccountId == input.ChartofAccountId);
               //.WhereIf(input.AllOrActive != true, e => (e.IsDeleted == input.AllOrActive));

                query = query.Where((e => (e.CreationTime.Year < input.SelectedMonth.Year) || (e.CreationTime.Year == input.SelectedMonth.Year && e.CreationTime.Month <= input.SelectedMonth.Month)));


                var monthStatus = await _timeManagementsAppService.GetMonthStatus(input.SelectedMonth);
                var isDeletedAccountExist = query.Where((e => e.IsDeleted)).ToList();
                var AllItems = isDeletedAccountExist;
                if (input.AllOrActive == false)
                {
                    AllItems = query.Where((e => !e.IsDeleted)).ToList();
                    if (isDeletedAccountExist.Count > 0)
                    {
                        var deletedAccount = query.Where((e => (e.IsDeleted && ((e.DeletionTime.Value.Year > input.SelectedMonth.Year) || (e.DeletionTime.Value.Year == input.SelectedMonth.Year && e.DeletionTime.Value.Month > input.SelectedMonth.Month))))).ToList();
                        AllItems.AddRange(deletedAccount);
                      
                    }
                   
                }
                else
                {
                    AllItems = AllItems.Where((e => e.DeletionTime.Value.Month == input.SelectedMonth.Month && e.DeletionTime.Value.Year == input.SelectedMonth.Year)).ToList();
                }

                var itemsAmounts = _reconciliationAmountsRepository.GetAll().Where(p => p.ChartsofAccountId == input.ChartofAccountId && p.AmountType == AmountType.Amortized).ToList();
                var pagedAndFilteredItems = AllItems/*.OrderBy(input.Sorting ?? "id asc").PageBy(input)*/;
                var totalCount = query.Count();
                var amortizedList = (from o in pagedAndFilteredItems.ToList()

                                     select new AmortizedListForViewDto()
                                     {
                                         Id = o.Id,
                                         StartDate = o.StartDate,
                                         EndDate = o.EndDate,
                                         Description = o.Description,
                                         AccuredAmortization = CalculateAccuredAmount(o.Id, itemsAmounts, (int)(o.Criteria), o.Amount, o.EndDate, input.SelectedMonth, o.StartDate),
                                         BeginningAmount = o.Amount,
                                         IsDeleted = o.IsDeleted,
                                         NetAmount = CalculateNetAmount(o.Id, itemsAmounts, (int)(o.Criteria), o.Amount, o.EndDate, input.SelectedMonth, o.StartDate),
                                         Attachments = _attachmentAppService.GetAttachmentsPath(o.Id, 2).Result
                                     }).ToList();


                List<AmortizedListDto> result = new List<AmortizedListDto>();

                AmortizedListDto amortizedListDto = new AmortizedListDto
                {
                    amortizedListForViewDtos = amortizedList,
                    TotalBeginningAmount = amortizedList.Sum(item => item.BeginningAmount),
                    TotalAccuredAmortization = amortizedList.Sum(item => item.AccuredAmortization),
                    TotalNetAmount = amortizedList.Sum(item => item.NetAmount)
                };

                amortizedListDto.TotalTrialBalance = await _chartsofAccountAppService.GetTrialBalanceofAccount(input.ChartofAccountId,input.SelectedMonth);
                amortizedListDto.Comments = await _commentAppService.GetComments((int)CommentType.AmortizedList, input.ChartofAccountId);
                amortizedListDto.MonthStatus = monthStatus;


                if (input.ChartofAccountId != 0)
                {
                    var reconcilledResult = await _chartsofAccountAppService.CheckReconcilled(input.ChartofAccountId);
                    amortizedListDto.ReconciliedBase = reconcilledResult;
                    switch (reconcilledResult)
                    {
                        case 1:
                            await _chartsofAccountAppService.AddandUpdateBalance(amortizedListDto.TotalNetAmount, input.ChartofAccountId,input.SelectedMonth);
                            amortizedListDto.VarianceNetAmount = amortizedListDto.TotalNetAmount - amortizedListDto.TotalTrialBalance;
                            amortizedListDto.VarianceBeginningAmount = 0;
                            amortizedListDto.VarianceAccuredAmount = 0;
                            break;
                        case 2:
                            await _chartsofAccountAppService.AddandUpdateBalance(amortizedListDto.TotalBeginningAmount, input.ChartofAccountId,input.SelectedMonth);
                            amortizedListDto.VarianceBeginningAmount = amortizedListDto.TotalBeginningAmount - amortizedListDto.TotalTrialBalance;
                            amortizedListDto.VarianceAccuredAmount = amortizedListDto.TotalAccuredAmortization - amortizedListDto.TotalTrialBalance;
                            break;
                        case 3:
                            await _chartsofAccountAppService.AddandUpdateBalance(amortizedListDto.TotalAccuredAmortization, input.ChartofAccountId,input.SelectedMonth);
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
        }

        protected virtual double CalculateNetAmount(long itemId, List<ReconciliationAmounts> amountsList, int CriteriaId, double BeginningAmount, DateTime EndDate, DateTime Current, DateTime StartDate)
        {
            if (CriteriaId == 1)
            {
                double AccomulateAmount = CalculateAmount(itemId, amountsList, Current);
                double Result = BeginningAmount - AccomulateAmount;
                return Result;
            }
            else
            {
                double AccomulateAmount = CalculateAccuredAmount(itemId, amountsList, CriteriaId, BeginningAmount, EndDate, Current, StartDate);
                double Result = BeginningAmount - AccomulateAmount;
                return Result;
            }
           
            
        }

        public double CalculateAmount(long itemId, List<ReconciliationAmounts> amountsList, DateTime SelectedMonth)
        {
            var getAllAmountsOfRespectiveId = amountsList.Where(p => p.itemId == itemId).ToList();
            var CheckIsAmountChanged = getAllAmountsOfRespectiveId.Where(p => p.isChanged).ToList();
            if (CheckIsAmountChanged.Count > 0)
            {
                foreach (var e in getAllAmountsOfRespectiveId)
                {
                    if (((e.CreationTime.Year < SelectedMonth.Year) || (e.CreationTime.Year == SelectedMonth.Year && e.CreationTime.Month <= SelectedMonth.Month)) &&
                       ((e.ChangeDateTime.Year > SelectedMonth.Year) || (e.ChangeDateTime.Year == SelectedMonth.Year && e.ChangeDateTime.Month >= SelectedMonth.Month)))
                    {
                        return e.Amount;
                    }
                }

                var item = getAllAmountsOfRespectiveId.FirstOrDefault(p => p.ChangeDateTime.Year == 0001);
                return item.Amount;
            }
            else
            {
                return getAllAmountsOfRespectiveId[0].Amount;
            }
        }

        protected virtual double CalculateAccuredAmount(long itemId, List<ReconciliationAmounts> amountsList, int CriteriaId , double BeginningAmount,DateTime EndDate, DateTime Current, DateTime StartDate)
        {
            double Result =1;
            switch (CriteriaId)
            {
                case 1:
                    Result = CalculateAmount(itemId, amountsList, Current);
                    break;
                case 2:
                    Result = GetAccuredAmountMonthly(0, BeginningAmount, EndDate, Current, StartDate);
                    break;
                case 3:
                    Result = GetAccuredAmountDaily(0, BeginningAmount, EndDate, Current, StartDate);
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
            if ((int)input.Criteria == (int)Criteria.Manual)
            {

                ReconciliationAmounts amounts = new ReconciliationAmounts();
                amounts.Amount = input.AccomulateAmount;
                amounts.AmountType = AmountType.Amortized;
                amounts.itemId = itemAddedId;
                amounts.ChartsofAccountId = input.ChartsofAccountId;
                await _reconciliationAmountsRepository.InsertAsync(amounts);
            }
            else
            {
                ReconciliationAmounts amounts = new ReconciliationAmounts();
                amounts.Amount = input.Amount;
                amounts.AmountType = AmountType.Amortized;
                amounts.itemId = itemAddedId;
                amounts.ChartsofAccountId = input.ChartsofAccountId;
                await _reconciliationAmountsRepository.InsertAsync(amounts);
            }
               
          
            if (input.AttachmentsPath != null)
            {
                PostAttachmentsPathDto postAttachmentsPathDto = new PostAttachmentsPathDto();
                postAttachmentsPathDto.FilePath = input.AttachmentsPath;
                postAttachmentsPathDto.TypeId = itemAddedId;
                postAttachmentsPathDto.Type = 2;
                await _attachmentAppService.PostAttachmentsPath(postAttachmentsPathDto);
            }
            if (!String.IsNullOrWhiteSpace(input.CommentBody))
            {
                await PostComment(input.CommentBody, itemAddedId, CommentTypeDto.AmortizedItem);
            }


        }
        protected virtual async Task Update(CreateOrEditAmortizationDto input)
        {
            if ((int)input.Criteria == (int)Criteria.Manual)
            {
                var item = await _amortizationRepository.FirstOrDefaultAsync(input.Id);
                var previousItemAmount = await _reconciliationAmountsRepository.FirstOrDefaultAsync(p => p.itemId == input.Id && p.AmountType == AmountType.Amortized);
                previousItemAmount.isChanged = true;
                previousItemAmount.ChangeDateTime = DateTime.Now;
                await _reconciliationAmountsRepository.UpdateAsync(previousItemAmount);
                ReconciliationAmounts amounts = new ReconciliationAmounts();
                amounts.Amount = input.AccomulateAmount;
                amounts.AmountType = AmountType.Amortized;
                amounts.itemId = input.Id;
                amounts.isChanged = true;
                amounts.ChartsofAccountId = input.ChartsofAccountId;
                await _reconciliationAmountsRepository.InsertAsync(amounts);
            }
          
            if (input.AttachmentsPath != null)
            {
                PostAttachmentsPathDto postAttachmentsPathDto = new PostAttachmentsPathDto();
                postAttachmentsPathDto.FilePath = input.AttachmentsPath;
                postAttachmentsPathDto.TypeId = input.Id;
                postAttachmentsPathDto.Type = 2;
                await _attachmentAppService.PostAttachmentsPath(postAttachmentsPathDto);
            }
            if (!String.IsNullOrWhiteSpace(input.CommentBody))
            {
                await PostComment(input.CommentBody, input.Id, CommentTypeDto.AmortizedItem);
            }
        }

        public async Task<CreateOrEditAmortizationDto> GetAmortizedItemDetails(long Id)
        {
            using (_unitOfWorkManager.Current.DisableFilter(AbpDataFilters.SoftDelete))
            {
                CreateOrEditAmortizationDto ItemData = new CreateOrEditAmortizationDto();
                var item = await _amortizationRepository.FirstOrDefaultAsync(x => x.Id == Id);
                ItemData.Attachments = await _attachmentAppService.GetAttachmentsPath(Id, 2);
                ItemData.Comments = await _commentAppService.GetComments((int)CommentType.AmortizedItem, Id);
                var data = ObjectMapper.Map(item, ItemData);
                return data;
            }
        }

        public async Task PostComment(string comment, long TypeId, CommentTypeDto CommentType)
        {
            if (!String.IsNullOrWhiteSpace(comment))
            {
                var commentDto = new CreateOrEditCommentDto()
                {
                    Body = comment,
                    Type = CommentType,
                    TypeId = TypeId
                };
                await _commentAppService.Create(commentDto);
            }


        }
        public async Task RestoreAmortizedItem(long id)
        {
            using (_unitOfWorkManager.Current.DisableFilter(AbpDataFilters.SoftDelete))
            {
                var item = await _amortizationRepository.FirstOrDefaultAsync(id);
                item.IsDeleted = false;
                item.DeletionTime = DateTime.MinValue;
                await _amortizationRepository.UpdateAsync(item);
            }

        }


        #endregion

    }
}
