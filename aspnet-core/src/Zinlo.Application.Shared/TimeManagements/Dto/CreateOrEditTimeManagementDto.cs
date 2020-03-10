using System;
using Abp.Application.Services.Dto;

namespace Zinlo.TimeManagements.Dto
{
    public class CreateOrEditTimeManagementDto : EntityDto<long?>
    {

        public DateTime Month { get; set; }
		
		
		public bool Status { get; set; }
		
		

    }
}