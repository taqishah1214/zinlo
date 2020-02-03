using Abp.Application.Services.Dto;
using System;
using Zinlo.Dto;

namespace Zinlo.Categories.Dtos
{
    public class GetAllCategoriesInput : PagedAndSortedResultRequestDto  
    {
		public string Filter { get; set; }

		public string TitleFilter { get; set; }

		public string DescriptionFilter { get; set; }



    }
}