using System;
using System.Collections.Generic;
using System.Text;
using Zinlo.ChartsofAccount.Dtos;
using Zinlo.Dto;

namespace Zinlo.ChartsofAccount
{
  public  interface IInvalidAccountsExcellExporter
    {
      //  string ExportInvalidAccountsUrl(List<ChartsOfAccountsExcellImportDto> accountsListDtos);
        string ExportToFile(List<ChartsOfAccountsExcellImportDto> accountsListDtos);
    }
}
