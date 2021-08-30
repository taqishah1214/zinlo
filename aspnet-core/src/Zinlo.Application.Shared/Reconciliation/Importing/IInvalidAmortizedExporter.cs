using System.Collections.Generic;
using Zinlo.Reconciliation.Dtos;

namespace Zinlo.Reconciliation.Importing
{
    public interface IInvalidAmortizedExporter
    {
        string ExportToFile(List<AmortizedExcelImportDto> inputs);
    }
}
