using System;
using System.Collections.Generic;
using System.Text;
using Zinlo.ChartsofAccount.Dtos;
using Zinlo.Dto;

namespace Zinlo.ChartsofAccount
{
  public  interface IChartsOfAccountsTrialBalanceExcelExporter
    {
        FileDto ExportToExcell(List<ChartsOfAccountsTrialBalanceExcellImportDto> accountsTrialBalanceDtos);
    }
}
