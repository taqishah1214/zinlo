using Abp.Domain.Entities;

namespace Zinlo.InstructionVersions
{
    public class InstructionVersion : Entity<long>
    {
        public string Body { get; set; }
    }
}
