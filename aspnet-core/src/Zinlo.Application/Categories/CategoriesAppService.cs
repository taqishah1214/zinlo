using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using Abp.Linq.Extensions;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Zinlo.Categories.Dtos;
using Zinlo.Dto;
using Abp.Application.Services.Dto;
using Zinlo.Authorization;
using Abp.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using Zinlo.Authorization.Users;
using Zinlo.Authorization.Users.Profile;

namespace Zinlo.Categories
{
    [AbpAuthorize(AppPermissions.Pages_Categories)]
    public class CategoriesAppService : ZinloAppServiceBase, ICategoriesAppService
    {
        private readonly IRepository<Category, long> _categoryRepository;
        private readonly IProfileAppService _profileAppService;
        public CategoriesAppService(IRepository<Category, long> categoryRepository, UserManager userManager, IRepository<User, long> userRepository, IProfileAppService profileAppService)
        {
            _categoryRepository = categoryRepository;
            _profileAppService = profileAppService;
        }
        public async Task<PagedResultDto<GetCategoryForViewDto>> GetAll(GetAllCategoriesInput input)
        {
            var filteredCategories = _categoryRepository.GetAll()
                        .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false || e.Title.Contains(input.Filter) || e.Description.Contains(input.Filter))
                        .WhereIf(!string.IsNullOrWhiteSpace(input.TitleFilter), e => e.Title == input.TitleFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.DescriptionFilter), e => e.Description == input.DescriptionFilter);

            var pagedAndFilteredCategories = filteredCategories.OrderBy(input.Sorting ?? "id asc").PageBy(input).ToList();

            var totalCount = await filteredCategories.CountAsync();
            var mappedData = ObjectMapper.Map<List<GetCategoryForViewDto>>(pagedAndFilteredCategories);
            foreach (var data in mappedData)
            {
                if (data.UserId != null)
                {
                    var userDetail = UserManager.GetUserById((long)data.UserId);
                    data.CreatedBy = userDetail.FullName;
                    data.ProfilePicture = userDetail.ProfilePictureId.HasValue ? "data:image/jpeg;base64," + _profileAppService.GetProfilePictureById((Guid)userDetail.ProfilePictureId).Result.ProfilePicture : "";
                }
            }
            return new PagedResultDto<GetCategoryForViewDto>(
                totalCount,
                 mappedData
            );
        }

        public async Task<GetCategoryForViewDto> GetCategoryForView(int id)
        {
            var category = await _categoryRepository.GetAsync(id);

            var output = ObjectMapper.Map<GetCategoryForViewDto>(category);

            return output;
        }

        [AbpAuthorize(AppPermissions.Pages_Categories_Edit)]
        public async Task<GetCategoryForEditOutput> GetCategoryForEdit(EntityDto input)
        {
            var category = await _categoryRepository.FirstOrDefaultAsync(input.Id);
            var output = new GetCategoryForEditOutput { Category = ObjectMapper.Map<CreateOrEditCategoryDto>(category) };
            return output;
        }

        public async Task<long>CreateOrEdit(CreateOrEditCategoryDto input)
        {
            if (input.Id == null)
            {
                long id =  await Create(input);
                return id;
            }
            else
            {
                long id = await Update(input);
                return id;
            }
        }

        [AbpAuthorize(AppPermissions.Pages_Categories_Create)]
        protected virtual async Task<long> Create(CreateOrEditCategoryDto input)
        {
            var category = ObjectMapper.Map<Category>(input);


            if (AbpSession.TenantId != null)
            {
                category.TenantId = (int)AbpSession.TenantId;
            }
            var id  =  await _categoryRepository.InsertAndGetIdAsync(category);
            return id;
        }

        [AbpAuthorize(AppPermissions.Pages_Categories_Edit)]
        protected virtual async Task<long> Update(CreateOrEditCategoryDto input)
        {
            if (input.Id != null)
            {
                var category = await _categoryRepository.FirstOrDefaultAsync((int)input.Id);
                ObjectMapper.Map(input, category);
            }

            return (long)input.Id;
        }

        [AbpAuthorize(AppPermissions.Pages_Categories_Delete)]
        public async Task Delete(EntityDto input)
        {
            await _categoryRepository.DeleteAsync(input.Id);
        }

        public async Task<List<NameValueDto<long>>> CategoryDropDown()
        {
            var categories = _categoryRepository.GetAll();
            var query = (from o in categories

                         select new NameValueDto<long>()
                         {
                             Name = o.Title,
                             Value = o.Id
                         });

            var assets = await query.ToListAsync();
            return assets;
        }

        public bool IsCategoryExist(string input)
        {
            if(input != null)
            {
                bool isExist = _categoryRepository.GetAll().Any(x => x.Title.Trim().ToLower() == input.Trim().ToLower());
                return isExist;
            }
            else
            {
                return false;
            }
           
        }
    }
}