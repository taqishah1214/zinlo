using Abp.Application.Services.Dto;

namespace Zinlo.InstructionVersions.Dto
{
    public class GetInstruction : EntityDto<long>
    {
        public string Body { get; set; }
    }
}