﻿using Abp.Application.Services.Dto;
using System;

namespace Zinlo.Categories.Dtos
{
    public class GetAllCategoriesForExcelInput
    {
		public string Filter { get; set; }

		public string TitleFilter { get; set; }

		public string DescriptionFilter { get; set; }



    }
}