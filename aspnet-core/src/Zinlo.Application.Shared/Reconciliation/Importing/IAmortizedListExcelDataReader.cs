using Abp.Dependency;
using System;
using System.Collections.Generic;
using System.Text;
using Zinlo.Reconciliation.Dtos;

namespace Zinlo.Reconciliation.Importing
{
    public interface IAmortizedListExcelDataReader:ITransientDependency
    {
        List<AmortizedExcelImportDto> GetAccountsFromExcel(byte[] fileBytes);
    }
}
