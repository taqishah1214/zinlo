
using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace Zinlo.Categories.Dtos
{
    public class CreateOrEditCategoryDto : EntityDto<int?>
    {

		[Required]
		public string Title { get; set; }
		
		
		[Required]
		public string Description { get; set; }
		
		

    }
}