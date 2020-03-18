using Abp.Localization;
using Abp.Localization.Sources;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Zinlo.ChartsofAccount.Dtos;
using Zinlo.DataExporting.Excel.EpPlus;

namespace Zinlo.ChartsofAccount.Importing
{

    public class ChartsOfAccountListExcelDataReader : EpPlusExcelImporterBase<ChartsOfAccountsExcellImportDto>, IChartsOfAccontListExcelDataReader
    {
        private readonly ILocalizationSource _localizationSource;
        public ChartsOfAccountListExcelDataReader(ILocalizationManager localizationManager)
        {
            _localizationSource = localizationManager.GetSource(ZinloConsts.LocalizationSourceName);
        }
        public List<ChartsOfAccountsExcellImportDto> GetAccountsFromExcel(byte[] fileBytes)
        {
            return ProcessExcelFile(fileBytes, ProcessExcelRow);
        }
        private ChartsOfAccountsExcellImportDto ProcessExcelRow(ExcelWorksheet worksheet, int row)
        {
            if (IsRowEmpty(worksheet, row))
            {
                return null;
            }
            var exceptionMessage = new StringBuilder();
            var chartsofaccount = new ChartsOfAccountsExcellImportDto();
            try
            {
                chartsofaccount.AccountName = GetRequiredValueFromRowOrNull(worksheet, row, 1, nameof(chartsofaccount.AccountName), exceptionMessage);
                chartsofaccount.AccountNumber = GetRequiredValueFromRowOrNull(worksheet, row, 2, nameof(chartsofaccount.AccountNumber), exceptionMessage);
                chartsofaccount.AccountType = GetRequiredValueFromRowOrNull(worksheet, row, 3, nameof(chartsofaccount.AccountType), exceptionMessage);
                chartsofaccount.AccountSubType = GetRequiredValueFromRowOrNull(worksheet, row, 4, nameof(chartsofaccount.AccountSubType), exceptionMessage);
                chartsofaccount.AssignedUser = GetRequiredValueFromRowOrNull(worksheet, row, 5, nameof(chartsofaccount.AssignedUser), exceptionMessage);
                chartsofaccount.ReconciliationType = GetRequiredValueFromRowOrNull(worksheet, row, 6, nameof(chartsofaccount.ReconciliationType), exceptionMessage);
            }
            catch (System.Exception exception)
            {
                chartsofaccount.Exception = exception.Message;
            }
            return chartsofaccount;
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
