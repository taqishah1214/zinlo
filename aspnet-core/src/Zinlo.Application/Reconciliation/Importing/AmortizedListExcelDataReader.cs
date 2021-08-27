using Abp.Localization;
using Abp.Localization.Sources;
using OfficeOpenXml;
using System.Collections.Generic;
using System.Text;
using Zinlo.Reconciliation.Dtos;
using Zinlo.DataExporting.Excel.EpPlus;

namespace Zinlo.Reconciliation.Importing
{
    public class AmortizedListExcelDataReader : EpPlusExcelImporterBase<AmortizedExcelImportDto>, IAmortizedListExcelDataReader
    {
        private readonly ILocalizationSource _localizationSource;
        public AmortizedListExcelDataReader(ILocalizationManager localizationManager)
        {
            _localizationSource = localizationManager.GetSource(ZinloConsts.LocalizationSourceName);
        }

        public List<AmortizedExcelImportDto> GetAccountsFromExcel(byte[] fileBytes)
        {
            return ProcessExcelFile(fileBytes, ProcessExcelRow);
        }

        private AmortizedExcelImportDto ProcessExcelRow(ExcelWorksheet worksheet, int row)
        {
            var exceptionMessage = new StringBuilder();
            var amortized = new AmortizedExcelImportDto();
            try
            {
                amortized.InvoiceNo = GetOptionalValueFromRowOrEmpty(worksheet, row, 1, nameof(amortized.InvoiceNo), exceptionMessage);
                amortized.JournalEntryNo = GetOptionalValueFromRowOrEmpty(worksheet, row, 2, nameof(amortized.JournalEntryNo), exceptionMessage);
                amortized.StartDate = GetRequiredValueFromRowOrNull(worksheet, row, 3, nameof(amortized.StartDate), exceptionMessage);
                amortized.EndDate = GetRequiredValueFromRowOrNull(worksheet, row, 4, nameof(amortized.EndDate), exceptionMessage);
                amortized.Amount = GetRequiredValueFromRowOrNull(worksheet, row, 5, nameof(amortized.Amount), exceptionMessage);
                amortized.Description = GetRequiredValueFromRowOrNull(worksheet, row, 6, nameof(amortized.Description), exceptionMessage);
                amortized.Criteria = GetRequiredValueFromRowOrNull(worksheet, row, 7, nameof(amortized.Criteria), exceptionMessage);
            }
            catch (System.Exception exception)
            {
                amortized.Exception = exception.Message;
            }
            return amortized;
        }

        private string GetRequiredValueFromRowOrNull(ExcelWorksheet worksheet, int row, int column, string columnName, StringBuilder exceptionMessage)
        {
            var cellValue = worksheet.Cells[row, column].Value;

            if (cellValue != null && !string.IsNullOrWhiteSpace(cellValue.ToString()))
            {
                return cellValue.ToString();
            }
            exceptionMessage.Append(GetLocalizedExceptionMessagePart(columnName));
            return "";
        }

        private string GetOptionalValueFromRowOrEmpty(ExcelWorksheet worksheet, int row, int column, string columnName, StringBuilder exceptionMessage)
        {
            var cellValue = worksheet.Cells[row, column].Value;

            if (cellValue != null && !string.IsNullOrWhiteSpace(cellValue.ToString()))
            {
                return cellValue.ToString();
            }
            else
                return "";
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
