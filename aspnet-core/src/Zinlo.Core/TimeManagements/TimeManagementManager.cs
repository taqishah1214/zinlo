using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Dependency;
using Abp.Domain.Entities;
using Abp.Domain.Repositories;
using Abp.Runtime.Session;
using Abp.UI;
using AutoMapper;
using Stripe;
using Zinlo.Authorization;
using Zinlo.ClosingChecklist;

namespace Zinlo.TimeManagements
{
    public class TimeManagementManager : ITransientDependency
    {
        public IAbpSession AbpSession { get; set; }
        private readonly IRepository<TimeManagement, long> _timeManagementRepository;

        public TimeManagementManager(IRepository<TimeManagement, long> timeManagementRepository)
        {
            AbpSession = NullAbpSession.Instance;
            _timeManagementRepository = timeManagementRepository;
        }

        public IQueryable<TimeManagement> GetAll()
        {

            return _timeManagementRepository.GetAll();

        }


        public async Task<TimeManagement> GetById(long id)
        {
            return await _timeManagementRepository.FirstOrDefaultAsync(id);


        }

        public async Task<TimeManagement> GetByDate(DateTime dateTime)
        {
            return await _timeManagementRepository.FirstOrDefaultAsync(p =>
                p.Month.Month.Equals(dateTime.Month) && p.Month.Year.Equals(dateTime.Year));


        }
        public virtual async Task<bool> CheckMonth(DateTime month)
        {
            var geTimeManagement = await GetMonthByDate(month);
            return geTimeManagement != null;
        }
        public virtual async Task<bool> CheckMonthStatus(DateTime month)
        {
            var geTimeManagement = await GetMonthByDate(month);
            return geTimeManagement != null && geTimeManagement.Status;
        }

        private async Task<TimeManagement> GetMonthByDate(DateTime dateTime)
        {
            return await _timeManagementRepository.FirstOrDefaultAsync(p => p.Month.Year == dateTime.Year && p.Month.Month == dateTime.Month);
        }
        public virtual async Task CreateAsync(TimeManagement input)
        { 
            await _timeManagementRepository.InsertAsync(input);
        }


        public virtual async Task UpdateAsync(TimeManagement input)
        {
            var timeManagement = await GetManagement((long)input.Id);
            await _timeManagementRepository.UpdateAsync(input);
        }

       
        public async Task DeleteAsync(long id)
        {
            await _timeManagementRepository.DeleteAsync(id);
        }
        
        public async Task<bool> GetMonthStatus(DateTime dateTime)
        {
            var management = await GetMonthByDate(dateTime);
            if (management == null)
            {
                if (dateTime.Year.Equals(DateTime.Now.Year) && dateTime.Month.Equals(DateTime.Now.Month))
                {
                    var createManagement = new TimeManagement()
                    {
                        Month = dateTime,
                        Status = false,
                    };
                    await CreateAsync(createManagement);
                }

                return false;
            }
            return management.Status;
        }

        public IEnumerable<TimeManagement> GetOpenManagement()
        {
            return  _timeManagementRepository.GetAllList().Where(p => !p.IsClosed);
            

        }

        public async Task<bool> TaskCreationValidation(DateTime input)
        {
            var month = await GetMonthByDate(input);
            if (month==null)
            {
                var timeManagement = new TimeManagement()
                {
                    Month = input,
                    Status = false,
                    IsClosed = false,
                };
                await CreateAsync(timeManagement);
                return false;
            }

            return month.Status;
        }

        public async Task<bool> CheckManagementExist(DateTime dateTime)
        {
            var checkManagement = await GetMonthByDate(dateTime);
            var result = checkManagement != null ? true : false;
            return result;
        }

        public virtual async Task<TimeManagement> GetManagement(long id)
        {
            return await _timeManagementRepository.FirstOrDefaultAsync(id);

        }

        public virtual async Task<bool> IsClosed(DateTime input)
        {
            var month= await GetMonthByDate(input);
            if (month == null)
            {
                var timeManagement = new TimeManagement()
                {
                    Month = input,
                    Status = false,
                    IsClosed = false,
                };
                await CreateAsync(timeManagement);
                return false;
            }
            return month.IsClosed;
        }
    }
}
