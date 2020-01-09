using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace Zinlo.Categories.Dtos
{
    public class GetCategoryForEditOutput
    {
		public CreateOrEditCategoryDto Category { get; set; }


    }
}