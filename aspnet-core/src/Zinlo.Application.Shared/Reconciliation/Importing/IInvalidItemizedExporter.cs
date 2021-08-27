using System.Collections.Generic;
using Zinlo.Reconciliation.Dtos;

namespace Zinlo.Reconciliation.Importing
{
    public interface IInvalidItemizedExporter
    {
        string ExportToFile(List<ItemizedExcelImportDto> inputs);
    }
}

