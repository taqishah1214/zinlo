using Abp.Application.Services.Dto;

namespace Zinlo.InstructionVersions.Dto
{
    public class CreateOrEditInstructionVersion : EntityDto<long>
    {
        public string Body { get; set; }

    }
}
