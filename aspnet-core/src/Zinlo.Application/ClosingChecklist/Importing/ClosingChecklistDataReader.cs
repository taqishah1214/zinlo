using Abp.Localization;
using Abp.Localization.Sources;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Text;
using Zinlo.ClosingChecklist.Dtos;
using Zinlo.DataExporting.Excel.EpPlus;

namespace Zinlo.ClosingChecklist.Importing
{
    public class ClosingChecklistDataReader : EpPlusExcelImporterBase<ClosingChecklistExcellImportDto>, IClosingChecklistExcelDataReader
    {
        private readonly ILocalizationSource _localizationSource;
        public ClosingChecklistDataReader(ILocalizationManager localizationManager)
        {
            _localizationSource = localizationManager.GetSource(ZinloConsts.LocalizationSourceName);
        }
        public List<ClosingChecklistExcellImportDto> GetClosingChecklistFromExcel(byte[] fileBytes)
        {
            return ProcessExcelFile(fileBytes, ProcessExcelRow);
        }
        private ClosingChecklistExcellImportDto ProcessExcelRow(ExcelWorksheet worksheet, int row)
        {
            if (IsRowEmpty(worksheet, row))
            {
                return null;
            }
            var exceptionMessage = new StringBuilder();
            var closingchecklist = new ClosingChecklistExcellImportDto();
            try
            {
                var bol = GetRequiredValueFromRowOrNull(worksheet, row, 7, nameof(closingchecklist.EndOfMonth), exceptionMessage);

                closingchecklist.CategoryName = GetRequiredValueFromRowOrNull(worksheet, row, 1, nameof(closingchecklist.CategoryName), exceptionMessage);
                closingchecklist.TaskName = GetRequiredValueFromRowOrNull(worksheet, row, 2, nameof(closingchecklist.TaskName), exceptionMessage);
                closingchecklist.AssigneeEmail = GetRequiredValueFromRowOrNull(worksheet, row, 3, nameof(closingchecklist.AssigneeEmail), exceptionMessage);
                closingchecklist.Frequency = GetRequiredValueFromRowOrNull(worksheet, row, 4, nameof(closingchecklist.Frequency), exceptionMessage);
                closingchecklist.NoOfMonths = Convert.ToInt32(GetRequiredValueFromRowOrNull(worksheet, row, 5, nameof(closingchecklist.NoOfMonths), exceptionMessage));
                closingchecklist.DueOn = Convert.ToInt32(GetRequiredValueFromRowOrNull(worksheet, row, 6, nameof(closingchecklist.DueOn), exceptionMessage));
                closingchecklist.EndOfMonth = (GetRequiredValueFromRowOrNull(worksheet, row, 7, nameof(closingchecklist.EndOfMonth), exceptionMessage)) == "True" ? true : false;
                closingchecklist.DayBeforeAfter = GetRequiredValueFromRowOrNull(worksheet, row, 8, nameof(closingchecklist.DayBeforeAfter), exceptionMessage);
                closingchecklist.Status = GetRequiredValueFromRowOrNull(worksheet, row, 9, nameof(closingchecklist.Status), exceptionMessage);
                closingchecklist.Instruction = GetRequiredValueFromRowOrNull(worksheet, row, 10, nameof(closingchecklist.Instruction), exceptionMessage);
            }
            catch (System.Exception exception)
            {
                closingchecklist.Exception = exception.Message;
            }
            return closingchecklist;
        }

        private string GetRequiredValueFromRowOrNull(ExcelWorksheet worksheet, int row, int column, string columnName, StringBuilder exceptionMessage)
        {
            var cellValue = worksheet.Cells[row, column].Value;

            if (cellValue != null && !string.IsNullOrWhiteSpace(cellValue.ToString()))
            {
                return cellValue.ToString();
            }
            exceptionMessage.Append(GetLocalizedExceptionMessagePart(columnName));
            return null;
        }

        private string GetLocalizedExceptionMessagePart(string parameter)
        {
            return _localizationSource.GetString("{0}IsInvalid", _localizationSource.GetString(parameter)) + "; ";
        }

        private bool IsRowEmpty(ExcelWorksheet worksheet, int row)
        {
            return worksheet.Cells[row, 1].Value == null || string.IsNullOrWhiteSpace(worksheet.Cells[row, 1].Value.ToString());
        }
    }
}
