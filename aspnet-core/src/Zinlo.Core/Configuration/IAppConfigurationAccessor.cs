using Microsoft.Extensions.Configuration;

namespace Zinlo.Configuration
{
    public interface IAppConfigurationAccessor
    {
        IConfigurationRoot Configuration { get; }
    }
}
