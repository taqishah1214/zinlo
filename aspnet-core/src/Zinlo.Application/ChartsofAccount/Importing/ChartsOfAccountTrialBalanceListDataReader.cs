using Abp.Localization;
using Abp.Localization.Sources;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Text;
using Zinlo.ChartsofAccount.Dtos;
using Zinlo.DataExporting.Excel.EpPlus;

namespace Zinlo.ChartsofAccount.Importing
{
    
 
    public class ChartsOfAccountTrialBalanceListDataReader : EpPlusExcelImporterBase<ChartsOfAccountsTrialBalanceExcellImportDto>, IChartsOfAccontTrialBalanceListExcelDataReader
    {
        private readonly ILocalizationSource _localizationSource;
        public ChartsOfAccountTrialBalanceListDataReader(ILocalizationManager localizationManager)
        {
            _localizationSource = localizationManager.GetSource(ZinloConsts.LocalizationSourceName);
        }
        public List<ChartsOfAccountsTrialBalanceExcellImportDto> GetAccountsTrialBalanceFromExcel(byte[] fileBytes)
        {
            return ProcessExcelFile(fileBytes, ProcessExcelRow);
        }
        private ChartsOfAccountsTrialBalanceExcellImportDto ProcessExcelRow(ExcelWorksheet worksheet, int row)
        {
            if (IsRowEmpty(worksheet, row))
            {
                return null;
            }
            var exceptionMessage = new StringBuilder();
            var chartsofaccount = new ChartsOfAccountsTrialBalanceExcellImportDto();
            try
            {
                chartsofaccount.AccountNumber = GetRequiredValueFromRowOrNull(worksheet, row, 1, nameof(chartsofaccount.AccountNumber), exceptionMessage);
                chartsofaccount.AccountName = GetRequiredValueFromRowOrNull(worksheet, row, 2, nameof(chartsofaccount.AccountName), exceptionMessage);
                chartsofaccount.Balance = GetRequiredValueFromRowOrNull(worksheet, row, 3, nameof(chartsofaccount.Balance), exceptionMessage);             
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
