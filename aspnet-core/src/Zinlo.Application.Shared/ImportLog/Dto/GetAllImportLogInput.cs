using Abp.Application.Services.Dto;

namespace Zinlo.ErrorLog.Dto
{
    public class GetAllImportLogInput : PagedAndSortedResultRequestDto
    {
        public string Filter { get; set; }
    }
}
