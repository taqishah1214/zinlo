using Abp.Dependency;
using System;
using System.Collections.Generic;
using System.Text;
using Zinlo.ChartsofAccount.Dtos;

namespace Zinlo.ChartsofAccount.Importing
{
    public interface IChartsOfAccontListExcelDataReader : ITransientDependency
    {
        List<ChartsOfAccountsExcellImportDto> GetAccountsFromExcel(byte[] fileBytes);
    }
}
