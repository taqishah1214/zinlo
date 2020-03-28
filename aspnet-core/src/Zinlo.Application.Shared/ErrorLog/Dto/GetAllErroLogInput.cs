using Abp.Application.Services.Dto;

namespace Zinlo.ErrorLog.Dto
{
    public class GetAllErroLogInput : PagedAndSortedResultRequestDto
    {
        public string Filter { get; set; }
    }
}
