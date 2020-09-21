using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using Abp.Runtime.Validation;
using Abp.Timing;

namespace Zinlo.ClosingChecklist.Dtos
{
    [DisableDateTimeNormalization]
    public class GetAllClosingCheckListInput : PagedAndSortedResultRequestDto,IGetTaskInput
    {
        public string Filter { get; set; }
        public int CategoryFilter { get; set; }
        public int StatusFilter { get; set; }
        public DateTime DateFilter { get; set; }
        public long AssigneeId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool? AllOrActive { get; set; }

    }

    [DisableDateTimeNormalization]
    public class GetTaskReportInput : PagedAndSortedResultRequestDto, IGetTaskInput
    {
        public string Filter { get; set; }
        public int CategoryFilter { get; set; }
        public int StatusFilter { get; set; }
        public long AssigneeId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
    [DisableDateTimeNormalization]
    public class GetTaskToExcelInput : IGetTaskInput
    {
        public string Filter { get; set; }
        public int CategoryFilter { get; set; }
        public int StatusFilter { get; set; }
        public long AssigneeId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Sorting { get; set; }
    }

    public interface IGetTaskInput : ISortedResultRequest
    {
        string Filter { get; set; }
        int CategoryFilter { get; set; }
        int StatusFilter { get; set; }
        long AssigneeId { get; set; }
        DateTime? StartDate { get; set; }
        DateTime? EndDate { get; set; }

    }
}
