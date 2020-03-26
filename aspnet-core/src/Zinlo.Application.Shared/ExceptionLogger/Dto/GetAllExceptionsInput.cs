using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace Zinlo.ExceptionLogger.Dto
{
    public class GetAllExceptionsInput : PagedAndSortedResultRequestDto
    {
        public string Filter { get; set; }
    }
}
