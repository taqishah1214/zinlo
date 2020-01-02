using System.Collections.Generic;
using Zinlo.Auditing.Dto;
using Zinlo.Dto;

namespace Zinlo.Auditing.Exporting
{
    public interface IAuditLogListExcelExporter
    {
        FileDto ExportToFile(List<AuditLogListDto> auditLogListDtos);

        FileDto ExportToFile(List<EntityChangeListDto> entityChangeListDtos);
    }
}
