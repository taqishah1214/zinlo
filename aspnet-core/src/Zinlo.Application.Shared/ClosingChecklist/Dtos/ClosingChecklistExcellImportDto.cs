using System;

namespace Zinlo.ClosingChecklist.Dtos
{
    public class ClosingChecklistExcellImportDto
    {
        public string CategoryName { get; set; }
        public string TaskName { get; set; }
        public string AssigneeEmail { get; set; }
        public string Frequency { get; set; }
        public int NoOfMonths { get; set; }
        public int DueOn { get; set; }
        public bool EndOfMonth { get; set; }
        public string Exception { get; set; }
        public DateTime selectedMonth { get; set; }
        public string DayBeforeAfter { get; set; }
        public string Status { get; set; }
        public string Instruction { get; set; }
        public bool CanBeImported()
        {
            return string.IsNullOrEmpty(Exception);
        }
    }

    public enum FileTypes
    {
        ClosingChecklist
    }
}
