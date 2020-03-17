using System;
using System.Collections.Generic;
using System.Text;
using Zinlo.ChartsofAccount.Dtos;

namespace Zinlo.ChartsofAccount.Importing
{
  public  interface IChartsOfAccontTrialBalanceListExcelDataReader
    {
        List<ChartsOfAccountsTrialBalanceExcellImportDto> GetAccountsTrialBalanceFromExcel(byte[] fileBytes);
    }
}
