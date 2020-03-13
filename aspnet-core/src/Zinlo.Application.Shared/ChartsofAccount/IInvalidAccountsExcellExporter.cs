using System;
using System.Collections.Generic;
using System.Text;
using Zinlo.ChartsofAccount.Dtos;
using Zinlo.Dto;

namespace Zinlo.ChartsofAccount
{
  public  interface IInvalidAccountsExcellExporter
    {
        FileDto ExportToFile(List<ChartsOfAccountsExcellImportDto> accountsListDtos);
    }
}
