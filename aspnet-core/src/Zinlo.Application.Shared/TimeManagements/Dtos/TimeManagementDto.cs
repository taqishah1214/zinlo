
using System;
using Abp.Application.Services.Dto;

namespace Zinlo.TimeManagements.Dtos
{
    public class TimeManagementDto : EntityDto<long>
    {
		public DateTime OpenDate { get; set; }

		public DateTime CloseDate { get; set; }

		public DateTime Month { get; set; }

		public bool Status { get; set; }



    }
}