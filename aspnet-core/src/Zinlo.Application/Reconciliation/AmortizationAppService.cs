using Abp.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Zinlo.Reconciliation.Dtos;

namespace Zinlo.Reconciliation
{
   public class AmortizationAppService : ZinloAppServiceBase, IAmortizationAppService
    {
        private readonly IRepository<Amortization, long> _amortizationRepository;
        #region|#Constructor|
        public AmortizationAppService(IRepository<Amortization, long> amortizationRepository)
        {
            _amortizationRepository = amortizationRepository;
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
        protected virtual async Task Create(CreateOrEditAmortizationDto input)
        {
            var item = ObjectMapper.Map<Amortization>(input);
            await _amortizationRepository.InsertAsync(item);

        }
        protected virtual async Task Update(CreateOrEditAmortizationDto input)
        {
            var item = await _amortizationRepository.FirstOrDefaultAsync(input.Id);
            var data = ObjectMapper.Map(input, item);
            await _amortizationRepository.UpdateAsync(data);
        }
        #endregion

    }
}
