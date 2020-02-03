using Abp.Application.Services.Dto;

namespace Zinlo.TimeManagements.Dtos
{
    public class GetAllForLookupTableInput : PagedAndSortedResultRequestDto
    {
		public string Filter { get; set; }
    }
}