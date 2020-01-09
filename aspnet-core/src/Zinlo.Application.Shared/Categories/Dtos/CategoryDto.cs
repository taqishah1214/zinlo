
using System;
using Abp.Application.Services.Dto;

namespace Zinlo.Categories.Dtos
{
    public class CategoryDto : EntityDto
    {
		public string Title { get; set; }

		public string Description { get; set; }



    }
}