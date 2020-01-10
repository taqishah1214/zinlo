using System.Collections.Generic;
using Abp.Runtime.Session;
using Abp.Timing.Timezone;
using Zinlo.DataExporting.Excel.EpPlus;
using Zinlo.Categories.Dtos;
using Zinlo.Dto;
using Zinlo.Storage;

namespace Zinlo.Categories.Exporting
{
    public class CategoriesExcelExporter : EpPlusExcelExporterBase, ICategoriesExcelExporter
    {

        private readonly ITimeZoneConverter _timeZoneConverter;
        private readonly IAbpSession _abpSession;

        public CategoriesExcelExporter(
            ITimeZoneConverter timeZoneConverter,
            IAbpSession abpSession,
			ITempFileCacheManager tempFileCacheManager) :  
	base(tempFileCacheManager)
        {
            _timeZoneConverter = timeZoneConverter;
            _abpSession = abpSession;
        }

        public FileDto ExportToFile(List<GetCategoryForViewDto> categories)
        {
            return CreateExcelPackage(
                "Categories.xlsx",
                excelPackage =>
                {
                    var sheet = excelPackage.Workbook.Worksheets.Add(L("Categories"));
                    sheet.OutLineApplyStyle = true;

                    AddHeader(
                        sheet,
                        L("Title"),
                        L("Description")
                        );

                    AddObjects(
                        sheet, 2, categories,
                        _ => _.Category.Title,
                        _ => _.Category.Description
                        );

					

                });
        }
    }
}
