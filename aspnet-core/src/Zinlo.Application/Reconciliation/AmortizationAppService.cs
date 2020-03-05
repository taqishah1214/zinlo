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

namespace Zinlo.Reconciliation
{
   public class AmortizationAppService : ZinloAppServiceBase, IAmortizationAppService
    {
        private readonly IRepository<Amortization, long> _amortizationRepository;
        private readonly IAttachmentAppService _attachmentAppService;

        #region|#Constructor|
        public AmortizationAppService(IRepository<Amortization, long> amortizationRepository,IAttachmentAppService attachmentAppService)
        {
            _attachmentAppService = attachmentAppService;
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

        public async Task Delete(long Id)
        {
            await _amortizationRepository.DeleteAsync(Id);
        }

        public async Task<PagedResultDto<AmortizedListForViewDto>> GetAll(GetAllAmortizationInput input)
        {
            var query = _amortizationRepository.GetAll()
                .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false || e.Description.Contains(input.Filter))
                .WhereIf((input.ChartofAccountId != 0), e => false || e.ChartsofAccountId == input.ChartofAccountId);

            var pagedAndFilteredItems = query.OrderBy(input.Sorting ?? "id asc").PageBy(input);
            var totalCount = query.Count();
            var AmortizedList = from o in pagedAndFilteredItems

                               select new AmortizedListForViewDto()
                               {
                                   Id = o.Id,
                                   StartDate =o.StartDate,
                                   EndDate =o.EndDate,
                                   Description =o.Description,
                                   BeginningAmount =o.Amount,
                                   AccuredAmortization = 0,
                                   NetAmount = 0,
                               };

            return new PagedResultDto<AmortizedListForViewDto>(
               totalCount,
               AmortizedList.ToList()
           );
        }

        protected virtual async Task Create(CreateOrEditAmortizationDto input)
        {
            var item = ObjectMapper.Map<Amortization>(input);
            if (input.AttachmentsPath != null)
            {
                PostAttachmentsPathDto postAttachmentsPathDto = new PostAttachmentsPathDto();
                postAttachmentsPathDto.FilePath = input.AttachmentsPath;
                postAttachmentsPathDto.TypeId = 2;
                postAttachmentsPathDto.Type = 1;
                await _attachmentAppService.PostAttachmentsPath(postAttachmentsPathDto);
            }
            await _amortizationRepository.InsertAsync(item);

        }
        protected virtual async Task Update(CreateOrEditAmortizationDto input)
        {
            var item = await _amortizationRepository.FirstOrDefaultAsync(input.Id);
            var data = ObjectMapper.Map(input, item);
            await _amortizationRepository.UpdateAsync(data);
        }

        public async Task<CreateOrEditAmortizationDto> GetAmortizedItemDetails(long Id)
        {
            CreateOrEditAmortizationDto ItemData = new CreateOrEditAmortizationDto();
            var item = await _amortizationRepository.FirstOrDefaultAsync(x => x.Id == Id);
            var data = ObjectMapper.Map(item, ItemData);
            return data;
        }



        #endregion

    }
}
