﻿using Abp.Localization;
using Abp.Localization.Sources;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Text;
using Zinlo.Reconciliation.Dtos;
using Zinlo.DataExporting.Excel.EpPlus;

namespace Zinlo.Reconciliation.Importing
{
    public class ItemizedListExcelDataReader : EpPlusExcelImporterBase<ItemizedExcelImportDto>, IItemizedListExcelDataReader
    {
        private readonly ILocalizationSource _localizationSource;
        public ItemizedListExcelDataReader(ILocalizationManager localizationManager)
        {
            _localizationSource = localizationManager.GetSource(ZinloConsts.LocalizationSourceName);
        }

        public List<ItemizedExcelImportDto> GetAccountsFromExcel(byte[] fileBytes)
        {
            return ProcessExcelFile(fileBytes, ProcessExcelRow);
        }

        private ItemizedExcelImportDto ProcessExcelRow(ExcelWorksheet worksheet, int row)
        {
            if (IsRowEmpty(worksheet,row))
            {
                return null;
            }
            var exceptionMessage = new StringBuilder();
            var itemized = new ItemizedExcelImportDto();
            try
            {
                itemized.InoviceNo = GetOptionalValueFromRowOrEmpty(worksheet, row, 1, nameof(itemized.InoviceNo), exceptionMessage);
                itemized.JournalEntryNo = GetOptionalValueFromRowOrEmpty(worksheet, row, 2, nameof(itemized.JournalEntryNo), exceptionMessage);
                itemized.Date = GetRequiredValueFromRowOrNull(worksheet, row, 3, nameof(itemized.Date), exceptionMessage);
                itemized.Amount = GetRequiredValueFromRowOrNull(worksheet, row, 4, nameof(itemized.Amount), exceptionMessage);
                itemized.Description = GetRequiredValueFromRowOrNull(worksheet, row, 5, nameof(itemized.Description), exceptionMessage);
                
            }
            catch (System.Exception exception)
            {
                itemized.Exception = exception.Message;
            }
            return itemized;
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
            bool isDateCellEmpty = (worksheet.Cells[row, 3].Value == null || string.IsNullOrWhiteSpace(worksheet.Cells[row, 3].Value.ToString()));
            bool isAmountCellEmpty = (worksheet.Cells[row, 4].Value == null || string.IsNullOrWhiteSpace(worksheet.Cells[row, 4].Value.ToString()));
            bool isDescriptionCellEmpty = (worksheet.Cells[row, 5].Value == null || string.IsNullOrWhiteSpace(worksheet.Cells[row, 5].Value.ToString()));

            if (isDateCellEmpty == true && isAmountCellEmpty == true && isDescriptionCellEmpty == true)
                return true;
            else
                return false;
        }

    }
}
