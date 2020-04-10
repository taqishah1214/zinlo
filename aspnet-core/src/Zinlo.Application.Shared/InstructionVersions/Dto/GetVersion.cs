using Abp.Application.Services.Dto;

namespace Zinlo.InstructionVersions.Dto
{
    public class GetVersion : EntityDto<long>
    {
        public string Body { get; set; }
    }
}