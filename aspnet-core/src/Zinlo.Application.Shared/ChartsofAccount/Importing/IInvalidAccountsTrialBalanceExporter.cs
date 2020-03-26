using System;
using System.Collections.Generic;
using System.Text;
using Zinlo.ChartsofAccount.Dtos;
using Zinlo.Dto;

namespace Zinlo.ChartsofAccount.Importing
{
  public  interface IInvalidAccountsTrialBalanceExporter
    {
        string ExportToFile(List<ChartsOfAccountsTrialBalanceExcellImportDto> inputs);
    }
}
