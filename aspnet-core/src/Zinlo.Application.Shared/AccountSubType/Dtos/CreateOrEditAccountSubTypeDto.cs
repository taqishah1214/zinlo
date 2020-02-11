using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Zinlo.AccountSubType.Dtos
{	public class CreateOrEditAccountSubTypeDto : EntityDto<int?>
	{
		[Required]
		public string Title { get; set; }
		[Required]
		public string Description { get; set; }

	}
}
