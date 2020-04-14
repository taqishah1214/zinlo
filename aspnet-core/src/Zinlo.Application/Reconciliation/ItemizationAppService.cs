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
using Zinlo.Comment.Dtos;
using Zinlo.Comment;
using Abp.Domain.Uow;

namespace Zinlo.Reconciliation
{
    public class ItemizationAppService : ZinloAppServiceBase, IItemizationAppService
    {
        private readonly IRepository<Itemization, long> _itemizationRepository;
        private readonly IAttachmentAppService _attachmentAppService;
        private readonly IChartsofAccountAppService _chartsofAccountAppService;
        private readonly ICommentAppService _commentAppService;
        private readonly IUnitOfWorkManager _unitOfWorkManager;


        #region|#Constructor|
        public ItemizationAppService(ICommentAppService commentAppService, IChartsofAccountAppService chartsofAccountAppService,IRepository<Itemization,long> itemizationRepository, 
            IAttachmentAppService attachmentAppService,
            IUnitOfWorkManager unitOfWorkManager)
        {
            _itemizationRepository = itemizationRepository;
            _attachmentAppService = attachmentAppService;
            _chartsofAccountAppService = chartsofAccountAppService;
            _commentAppService = commentAppService;
            _unitOfWorkManager = unitOfWorkManager;
        }
        #endregion
        #region|Get All|
        public async Task<PagedResultDto<ItemizedListDto>> GetAll(GetAllItemizationInput input)
        {
            using (_unitOfWorkManager.Current.DisableFilter(AbpDataFilters.SoftDelete))
            {
                var query = _itemizationRepository.GetAll().Where(e => e.ClosingMonth.Month == input.MonthFilter.Month && e.ClosingMonth.Year == input.MonthFilter.Year)
              .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false || e.Description.Contains(input.Filter))
              .WhereIf((input.ChartofAccountId != 0), e => false || e.ChartsofAccountId == input.ChartofAccountId)
              .WhereIf(!string.IsNullOrWhiteSpace(input.AccountNumer), e => false || e.ChartsofAccount.AccountNumber == input.AccountNumer)
              .WhereIf(input.AllOrActive != true, e => (e.IsDeleted == input.AllOrActive));

                var pagedAndFilteredItems = query.OrderBy(input.Sorting ?? "id asc").PageBy(input);
                var totalCount = query.Count();
                var ItemizedList = (from o in pagedAndFilteredItems.ToList()

                                    select new ItemizedListForViewDto()
                                    {
                                        Id = o.Id,
                                        Amount = o.Amount,
                                        Date = o.Date,
                                        Description = o.Description,
                                        IsDeleted = o.IsDeleted,
                                        Attachments = _attachmentAppService.GetAttachmentsPath(o.Id, 3).Result
                                    }).ToList();

                List<ItemizedListDto> result = new List<ItemizedListDto>();
                ItemizedListDto itemizedListDto = new ItemizedListDto
                {
                    itemizedListForViewDto = ItemizedList,
                    TotalAmount = ItemizedList.Sum(item => item.Amount),
                    TotalTrialBalance = await _chartsofAccountAppService.GetTrialBalanceofAccount(input.ChartofAccountId),
                };
                itemizedListDto.Variance = itemizedListDto.TotalAmount - itemizedListDto.TotalTrialBalance;
                itemizedListDto.Comments = await _commentAppService.GetComments((int)CommentType.ItemizedList, input.ChartofAccountId);
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
            if (!String.IsNullOrWhiteSpace(input.CommentBody))
            {
                await PostComment(input.CommentBody, itemAddedId, CommentTypeDto.ItemizedItem);
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
            if (!String.IsNullOrWhiteSpace(input.CommentBody))
            {
                await PostComment(input.CommentBody, input.Id, CommentTypeDto.ItemizedItem);
            }
        }

        public async Task<CreateOrEditItemizationDto> GetEdit(long Id)
        {
            using (_unitOfWorkManager.Current.DisableFilter(AbpDataFilters.SoftDelete))
            {
                var item = await _itemizationRepository.FirstOrDefaultAsync(x => x.Id == Id);
                var output = ObjectMapper.Map<CreateOrEditItemizationDto>(item);
                output.Attachments = await _attachmentAppService.GetAttachmentsPath(Id, 3);
                output.Comments = await _commentAppService.GetComments((int)CommentType.ItemizedItem, Id);
                return output;
            }
        }

        public async Task Delete(long Id)
        {
            await _itemizationRepository.DeleteAsync(Id);
        }
        public async Task RestoreItemizedItem(long id)
        {
            using (_unitOfWorkManager.Current.DisableFilter(AbpDataFilters.SoftDelete))
            {
                var item = await _itemizationRepository.FirstOrDefaultAsync(id);
                item.IsDeleted = false;
                await _itemizationRepository.UpdateAsync(item);
            }

        }
        #endregion
    }
}
