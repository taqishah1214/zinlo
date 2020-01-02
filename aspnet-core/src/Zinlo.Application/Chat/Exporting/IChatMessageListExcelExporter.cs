using System.Collections.Generic;
using Zinlo.Chat.Dto;
using Zinlo.Dto;

namespace Zinlo.Chat.Exporting
{
    public interface IChatMessageListExcelExporter
    {
        FileDto ExportToFile(List<ChatMessageExportDto> messages);
    }
}
