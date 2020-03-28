using Abp.Dependency;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Threading.BackgroundWorkers;
using Abp.Threading.Timers;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Zinlo.ChartsofAccount.Dtos;
using Zinlo.Reconciliation;
using Zinlo.Reconciliation.Dtos;

namespace Zinlo.ChartsofAccount
{
   public class ChartsOfAccountMonthly :PeriodicBackgroundWorkerBase, ISingletonDependency
    {
        private readonly IRepository<ChartsofAccount, long> _chartsofAccountRepository;
        private readonly IChartsofAccountAppService _chartsofAccountAppService;
        private readonly IItemizationAppService _itemizationAppService;
        private readonly IRepository<Itemization, long> _itemizationRepository;
        private readonly IRepository<Amortization, long> _amortizationRepository;
        private readonly IAmortizationAppService _amortizationAppService;



        public ChartsOfAccountMonthly(IAmortizationAppService amortizationAppService, IRepository<Amortization, long> amortizationRepository,IItemizationAppService itemizationAppService, IRepository<Itemization, long> itemizationRepository, IChartsofAccountAppService chartsofAccountAppService, AbpTimer timer, IRepository<ChartsofAccount, long> chartsofAccountRepository)
        : base(timer)
        {
            _itemizationRepository = itemizationRepository;
            Timer.Period = 3600; //Every 1 Hour 
            _chartsofAccountRepository = chartsofAccountRepository;
            _chartsofAccountAppService = chartsofAccountAppService;
            _itemizationAppService = itemizationAppService;
            _amortizationRepository = amortizationRepository;
            _amortizationAppService = amortizationAppService;
        }

        public int GetLastDateofMonth()
        {
            DateTime now = DateTime.Now;
            var lastDate = DateTime.DaysInMonth(now.Year, now.Month);
            return lastDate;
        }

        public bool CheckLastHourOfMonth()
        {
            DateTime now = DateTime.Now;
            var CurrentDate = new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Second);
            int LastDate = GetLastDateofMonth();
            if (LastDate == CurrentDate.Day)
            {
                if (CurrentDate.Hour == 22 || CurrentDate.Hour == 23)
                {
                    return true;
                }
                else
                {
                    return false;
                }

            }
            else
            {
                return false;
            }
        }

        public void ShiftAmortizedItems(double previousAccountId, double newAccountId)
        {
            var amortizedItemList = _amortizationRepository.GetAll().Where(e => e.ChartsofAccount.Id == previousAccountId).Include(e => e.ChartsofAccount);
            var itemList = (from o in amortizedItemList.ToList()

                            select new CreateOrEditAmortizationDto()
                            {
                                Id = 0,
                                InoviceNo = o.InoviceNo,
                                JournalEntryNo = o.JournalEntryNo,
                                StartDate = o.StartDate,
                                EndDate = o.EndDate,
                                AccomulateAmount = o.AccomulateAmount,
                                Amount = o.Amount,
                                Description = o.Description,
                                ChartsofAccountId = (long)newAccountId,
                                CreationTime = o.CreationTime.AddMonths(1),
                                Criteria = (Reconciliation.Dtos.Criteria)o.Criteria,

                            }).ToList();

            foreach (var item in itemList)
            {    
                _amortizationAppService.CreateOrEdit(item);
            }

        }

        

        public void ShiftItemizedItem (double previousAccountId , double newAccountId)
        {
            var itemizedItemList = _itemizationRepository.GetAll().Where(e => e.ChartsofAccount.Id == previousAccountId).Include(e => e.ChartsofAccount);
            var itemList = (from o in itemizedItemList.ToList()

                            select new CreateOrEditItemizationDto()
                            {
                                InoviceNo = o.InoviceNo,
                                JournalEntryNo = o.JournalEntryNo,
                                Date = o.Date,
                                Amount = o.Amount,
                                Description = o.Description,
                                ChartsofAccountId = (long)newAccountId,
                                CreationTime = o.CreationTime.AddMonths(1)
                            }).ToList();

            foreach(var item in itemList)
            {
                item.Id = 0;
                _itemizationAppService.CreateOrEdit(item);
            }

        }

        public bool checkBackGroundServiceAlreadyRun()
        {
            DateTime now = DateTime.Now;
            var CurrentDate = new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, 0);
            var checkNextMonthAccounts = _chartsofAccountRepository.GetAll().FirstOrDefault(e => e.CreationTime.Month == CurrentDate.Month + 1 && e.CreationTime.Year == CurrentDate.Year);
            if (checkNextMonthAccounts == null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        [UnitOfWork]
        protected override async void  DoWork()
        { 
            if (CheckLastHourOfMonth())
            {    
                if (checkBackGroundServiceAlreadyRun())
                { 
                using (CurrentUnitOfWork.DisableFilter(AbpDataFilters.MayHaveTenant))
                {
                        DateTime now = DateTime.Now;
                        var CurrentDate = new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, 0);
                        var accountsList = _chartsofAccountRepository.GetAll().Where(e => e.CreationTime.Month == CurrentDate.Month && e.CreationTime.Year == CurrentDate.Year).Include(p => p.AccountSubType).Include(p => p.Assignee); 
                        var itemList = (from o in accountsList.ToList()

                                            select new CreateOrEditChartsofAccountDto()
                                            {
                                                Id = (int)o.Id,
                                                CreatorUserId = o.CreatorUserId,
                                                AccountName = o.AccountName,
                                                AccountNumber = o.AccountNumber,
                                                AccountType = (Dtos.AccountType)o.AccountType,
                                                ReconciliationType = (Dtos.ReconciliationType)o.ReconciliationType,
                                                AccountSubTypeId = o.AccountSubType.Id,
                                                Reconciled = (Dtos.Reconciled)o.Reconciled,
                                                Balance = 0,
                                                CreationTime = o.CreationTime.AddMonths(1),
                                                AssigneeId = o.Assignee.Id

                                            }).ToList();

                    foreach(var item in itemList)
                    {
                        double PreviousAccountId = (double)item.Id;
                        item.Id = 0;
                        double newAccountId = await _chartsofAccountAppService.CreateOrEdit(item);
                        if ((int)item.ReconciliationType == 1)
                        {
                            ShiftItemizedItem(PreviousAccountId, newAccountId);
                        }
                        else
                        {
                            ShiftAmortizedItems(PreviousAccountId, newAccountId);
                        }

                    }
                    CurrentUnitOfWork.SaveChanges();
                }
                }

            }
        }
    }
}




