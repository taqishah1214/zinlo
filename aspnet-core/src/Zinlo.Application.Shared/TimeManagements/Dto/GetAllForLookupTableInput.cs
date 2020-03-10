using Abp.Application.Services.Dto;

namespace Zinlo.TimeManagements.Dto
{
    public class GetAllForLookupTableInput : PagedAndSortedResultRequestDto
    {
		public string Filter { get; set; }
    }
}