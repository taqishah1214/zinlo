using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Zinlo.Categories.Dtos;
using Zinlo.Dto;

namespace Zinlo.Categories
{
    public interface ICategoriesAppService : IApplicationService 
    {
		//GetAllCategoriesInput input
		Task<PagedResultDto<GetCategoryForViewDto>> GetAll(GetAllCategoriesInput input);

        Task<GetCategoryForViewDto> GetCategoryForView(int id);

		Task<GetCategoryForEditOutput> GetCategoryForEdit(EntityDto input);

		Task CreateOrEdit(CreateOrEditCategoryDto input);

		Task Delete(EntityDto input);
        Task<List<NameValueDto<long>>> CategoryDropDown();



	}
}