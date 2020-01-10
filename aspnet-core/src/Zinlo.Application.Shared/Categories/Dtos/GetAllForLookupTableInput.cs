using Abp.Application.Services.Dto;

namespace Zinlo.Categories.Dtos
{
    public class GetAllForLookupTableInput : PagedAndSortedResultRequestDto
    {
		public string Filter { get; set; }
    }
}