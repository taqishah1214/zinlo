using Abp.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Zinlo.Reconciliation.Dtos;

namespace Zinlo.Reconciliation
{
    public class ItemizationAppService : ZinloAppServiceBase, IItemizationAppService
    {
        private readonly IRepository<Itemization, long> _itemizationRepository;
        #region|#Constructor|
        public ItemizationAppService(IRepository<Itemization,long> itemizationRepository)
        {
            _itemizationRepository = itemizationRepository;
        }
        #endregion
        #region|Create Edit|
        public async Task CreateOrEdit(CreateOrEditItemizationDto input)
        {
            if(input.Id == 0)
            {
                await Create(input);
            }
            else
            {
                await Update(input);
            }
        }
        protected virtual async Task Create(CreateOrEditItemizationDto input)
        {
            var item = ObjectMapper.Map<Itemization>(input);
           await _itemizationRepository.InsertAsync(item);

        }
        protected virtual async Task Update(CreateOrEditItemizationDto input)
        {
            var item = await _itemizationRepository.FirstOrDefaultAsync(input.Id);
            var data = ObjectMapper.Map(input, item);
          await  _itemizationRepository.UpdateAsync(data);
        }
        #endregion
    }
}
