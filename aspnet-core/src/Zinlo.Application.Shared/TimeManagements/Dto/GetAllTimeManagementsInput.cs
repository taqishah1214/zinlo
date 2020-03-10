using Abp.Application.Services.Dto;
using System;

namespace Zinlo.TimeManagements.Dto
{
    public class GetAllTimeManagementsInput : PagedAndSortedResultRequestDto
    {
		public string Filter { get; set; }



    }
}