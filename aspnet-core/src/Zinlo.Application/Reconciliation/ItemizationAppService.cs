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
        #region|Get All|
        public async Task<PagedResultDto<ItemizedListForViewDto>> GetAll(GetAllItemizationInput input)
        {
           
            var query = _itemizationRepository.GetAll()
                 .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false || e.Description.Contains(input.Filter));            
            var pagedAndFilteredItems = query.OrderBy(input.Sorting ?? "id asc").PageBy(input);
            var totalCount = query.Count();
            var ItemizedList = from o in pagedAndFilteredItems

                               select new ItemizedListForViewDto()
                               {
                                   Id = o.Id,
                                   Amount = o.Amount,
                                   Date = o.Date,
                                   Description = o.Description                                  
                               };

            return new PagedResultDto<ItemizedListForViewDto>(
               totalCount,
               ItemizedList.ToList()
           );

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

        public async Task<CreateOrEditItemizationDto> GetEdit(long Id)
        {
            var item = await _itemizationRepository.FirstOrDefaultAsync(x => x.Id == Id);
            var output = ObjectMapper.Map<CreateOrEditItemizationDto>(item);
            return output;
        }

        #endregion
    }
}
