using System.Collections.Generic;
using Zinlo.Categories.Dtos;
using Zinlo.Dto;

namespace Zinlo.Categories.Exporting
{
    public interface ICategoriesExcelExporter
    {
        FileDto ExportToFile(List<GetCategoryForViewDto> categories);
    }
}