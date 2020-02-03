
using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace Zinlo.TimeManagements.Dtos
{
    public class CreateOrEditTimeManagementDto : EntityDto<long?>
    {

		public DateTime OpenDate { get; set; }
		
		
		public DateTime CloseDate { get; set; }
		
		
		public DateTime Month { get; set; }
		
		
		public bool Status { get; set; }
		
		

    }
}