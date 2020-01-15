using System.Linq;
using System.Linq.Dynamic.Core;
using Abp.Linq.Extensions;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Zinlo.Categories.Exporting;
using Zinlo.Categories.Dtos;
using Zinlo.Dto;
using Abp.Application.Services.Dto;
using Zinlo.Authorization;
using Abp.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using Zinlo.Authorization.Users;

namespace Zinlo.Categories
{
	[AbpAuthorize(AppPermissions.Pages_Categories)]
    public class CategoriesAppService : ZinloAppServiceBase, ICategoriesAppService
    {
		 private readonly IRepository<Category,long> _categoryRepository;
        private readonly UserManager _userManager;

        private readonly ICategoriesExcelExporter _categoriesExcelExporter;
		 

		  public CategoriesAppService(IRepository<Category,long> categoryRepository, ICategoriesExcelExporter categoriesExcelExporter, UserManager userManager ) 
		  {
			_categoryRepository = categoryRepository;
			_categoriesExcelExporter = categoriesExcelExporter;
            _userManager = userManager;
        }
       private string getUserNameById(long UserId)
        {
            string userName = string.Empty;
            userName = _userManager.GetUserById(UserId).FullName;
            return userName;
        }

        public async Task<PagedResultDto<GetCategoryForViewDto>> GetAll(GetAllCategoriesInput input)
        {

            var filteredCategories = _categoryRepository.GetAll()
                        .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false || e.Title.Contains(input.Filter) || e.Description.Contains(input.Filter))
                        .WhereIf(!string.IsNullOrWhiteSpace(input.TitleFilter), e => e.Title == input.TitleFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.DescriptionFilter), e => e.Description == input.DescriptionFilter);
            
            var pagedAndFilteredCategories = filteredCategories
                .OrderBy(input.Sorting ?? "id asc")
                .PageBy(input);

            var categories =  from o in  pagedAndFilteredCategories                           
                              select new GetCategoryForViewDto()
                             {
                                 Category = new CategoryDto
                                 {   Id = o.Id,
                                    // CreatedBy = getUserNameById(o.Id),
                                      CreationDate = o.CreationTime,
                                      Title = o.Title,
                                      Description = o.Description,
                                   
                                 }
                             };

            var totalCount = await filteredCategories.CountAsync();

            return new PagedResultDto<GetCategoryForViewDto>(
                totalCount,
                await categories.ToListAsync()
            );
        }

        public async Task<GetCategoryForViewDto> GetCategoryForView(int id)
         {
            var category = await _categoryRepository.GetAsync(id);

            var output = new GetCategoryForViewDto { Category = ObjectMapper.Map<CategoryDto>(category) };
			
            return output;
         }
		 
		 [AbpAuthorize(AppPermissions.Pages_Categories_Edit)]
		 public async Task<GetCategoryForEditOutput> GetCategoryForEdit(EntityDto input)
         {
            var category = await _categoryRepository.FirstOrDefaultAsync(input.Id);
           
		    var output = new GetCategoryForEditOutput {Category = ObjectMapper.Map<CreateOrEditCategoryDto>(category)};
			
            return output;
         }

		 public async Task CreateOrEdit(CreateOrEditCategoryDto input)
         {
            if(input.Id == null){
				await Create(input);
			}
			else{
				await Update(input);
			}
         }

		 [AbpAuthorize(AppPermissions.Pages_Categories_Create)]
		 protected virtual async Task Create(CreateOrEditCategoryDto input)
         {
            var category = ObjectMapper.Map<Category>(input);

			
			if (AbpSession.TenantId != null)
			{
				category.TenantId = (int)AbpSession.TenantId;
			}
		

            await _categoryRepository.InsertAsync(category);
         }

		 [AbpAuthorize(AppPermissions.Pages_Categories_Edit)]
		 protected virtual async Task Update(CreateOrEditCategoryDto input)
         {
            var category = await _categoryRepository.FirstOrDefaultAsync((int)input.Id);
             ObjectMapper.Map(input, category);
         }

		 [AbpAuthorize(AppPermissions.Pages_Categories_Delete)]
         public async Task Delete(EntityDto input)
         {
            await _categoryRepository.DeleteAsync(input.Id);
         }

        //public Task<PagedResultDto<GetCategoryForViewDto>> GetAll(GetAllCategoriesInput input)
        //{
           
        //}

        public Task<FileDto> GetCategoriesToExcel(GetAllCategoriesForExcelInput input)
        {
            throw new System.NotImplementedException();
        }

        public Task<List<NameValueDto<int>>> CategoryDropDown()
        {
            throw new System.NotImplementedException();
        }

        /*		public async Task<FileDto> GetCategoriesToExcel(GetAllCategoriesForExcelInput input)
                 {

                    var filteredCategories = _categoryRepository.GetAll()
                                .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false  || e.Title.Contains(input.Filter) || e.Description.Contains(input.Filter))
                                .WhereIf(!string.IsNullOrWhiteSpace(input.TitleFilter),  e => e.Title == input.TitleFilter)
                                .WhereIf(!string.IsNullOrWhiteSpace(input.DescriptionFilter),  e => e.Description == input.DescriptionFilter);

                    var query = (from o in filteredCategories
                                 select new GetCategoryForViewDto() { 
                                    Category = new CategoryDto
                                    {
                                        Title = o.Title,
                                        Description = o.Description,
                                        Id = o.Id
                                    }
                                 });


                    var categoryListDtos = await query.ToListAsync();

                    return _categoriesExcelExporter.ExportToFile(categoryListDtos);
                 }*/

        /*      public async Task<List<NameValueDto<int>>> CategoryDropDown()
              {

                      var categories = _categoryRepository.GetAll();


                  var query = (from o in categoriesE:\Zinlo\zinlo\aspnet-core\src\Zinlo.Application.Shared\Editions\

                               select new NameValueDto<int>()
                               {
                                   Name = o.Title,
                                   Value = o.Id
                               });

                  var assets = await query.ToListAsync();
                  return assets;
              }
      */
    }
}