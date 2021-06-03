using System.Collections.Generic;
using Zinlo.ClosingChecklist.Dtos;

namespace Zinlo.ClosingChecklist.Importing
{
    public interface IInvalidClosingChecklistExporter
    {
        string ExportToFile(List<ClosingChecklistExcellImportDto> inputs);
    }
}
