using Abp.Auditing;
using Zinlo.Configuration.Dto;

namespace Zinlo.Configuration.Tenants.Dto
{
    public class TenantEmailSettingsEditDto : EmailSettingsEditDto
    {
        public bool UseHostDefaultEmailSettings { get; set; }
    }
}