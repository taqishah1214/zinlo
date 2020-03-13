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
using Zinlo.Attachments.Dtos;
using Zinlo.Attachments;
using Zinlo.ChartsofAccount;

namespace Zinlo.Reconciliation
{
    public class ItemizationAppService : ZinloAppServiceBase, IItemizationAppService
    {
        private readonly IRepository<Itemization, long> _itemizationRepository;
        private readonly IAttachmentAppService _attachmentAppService;
        private readonly IChartsofAccountAppService _chartsofAccountAppService;


        #region|#Constructor|
        public ItemizationAppService(IChartsofAccountAppService chartsofAccountAppService,IRepository<Itemization,long> itemizationRepository, IAttachmentAppService attachmentAppService)
        {
            _itemizationRepository = itemizationRepository;
            _attachmentAppService = attachmentAppService;
            _chartsofAccountAppService = chartsofAccountAppService;


    }
        #endregion
        #region|Get All|
        public async Task<PagedResultDto<ItemizedListDto>> GetAll(GetAllItemizationInput input)
        {     
            var query = _itemizationRepository.GetAll()
                 .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false || e.Description.Contains(input.Filter))  
                 .WhereIf((input.ChartofAccountId != 0), e => false || e.ChartsofAccountId == input.ChartofAccountId);

            var pagedAndFilteredItems = query.OrderBy(input.Sorting ?? "id asc").PageBy(input);
            var totalCount = query.Count();
            var ItemizedList = (from o in pagedAndFilteredItems.ToList()

                               select new ItemizedListForViewDto()
                               {
                                   Id = o.Id,
                                   Amount = o.Amount,
                                   Date = o.Date,
                                   Description = o.Description,
                                   Attachments = _attachmentAppService.GetAttachmentsPath(o.Id, 3).Result
                               }).ToList();

            List<ItemizedListDto> result = new List<ItemizedListDto>();
            ItemizedListDto itemizedListDto = new ItemizedListDto
            {
                itemizedListForViewDto = ItemizedList,
                TotalAmount = ItemizedList.Sum(item => item.Amount),
              
            };

            if (input.ChartofAccountId != 0)
            {
                await _chartsofAccountAppService.AddandUpdateBalance(itemizedListDto.TotalAmount, input.ChartofAccountId);
            }
            result.Add(itemizedListDto);
            return new PagedResultDto<ItemizedListDto>(
               totalCount,
               result
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


        public async Task PostAttachments(List<string> AttachmentsPath,long TypeId,int Type)
        {
            PostAttachmentsPathDto postAttachmentsPathDto = new PostAttachmentsPathDto();
            postAttachmentsPathDto.FilePath = AttachmentsPath;
            postAttachmentsPathDto.TypeId = TypeId;
            postAttachmentsPathDto.Type = Type;
            await _attachmentAppService.PostAttachmentsPath(postAttachmentsPathDto);
  
        }


        protected virtual async Task Create(CreateOrEditItemizationDto input)
        {
            var item = ObjectMapper.Map<Itemization>(input);
            if (AbpSession.TenantId != null)
            {
                item.TenantId = (int)AbpSession.TenantId;
            }
            long itemAddedId = await _itemizationRepository.InsertAndGetIdAsync(item);

            if (input.AttachmentsPath != null)
            {
                await PostAttachments(input.AttachmentsPath, itemAddedId, 3);
            }

        }
        protected virtual async Task Update(CreateOrEditItemizationDto input)
        {
            var item = await _itemizationRepository.FirstOrDefaultAsync(input.Id);
            var data = ObjectMapper.Map(input, item);
            await  _itemizationRepository.UpdateAsync(data);

            if (input.AttachmentsPath != null)
            {
                await PostAttachments(input.AttachmentsPath, input.Id, 3);
            }
        }

        public async Task<CreateOrEditItemizationDto> GetEdit(long Id)
        {
            var item = await _itemizationRepository.FirstOrDefaultAsync(x => x.Id == Id);
            var output = ObjectMapper.Map<CreateOrEditItemizationDto>(item);
            output.Attachments = await _attachmentAppService.GetAttachmentsPath(Id, 3);
            return output;
        }

        public async Task Delete(long Id)
        {
            await _itemizationRepository.DeleteAsync(Id);
        }

        #endregion
    }
}
