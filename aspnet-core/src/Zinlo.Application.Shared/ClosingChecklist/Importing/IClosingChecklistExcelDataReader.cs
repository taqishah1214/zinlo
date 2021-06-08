using Abp.Dependency;
using System;
using System.Collections.Generic;
using System.Text;
using Zinlo.ClosingChecklist.Dtos;

namespace Zinlo.ClosingChecklist.Importing
{
    public interface IClosingChecklistExcelDataReader : ITransientDependency
    {
        List<ClosingChecklistExcellImportDto> GetClosingChecklistFromExcel(byte[] fileBytes);
    }
}
