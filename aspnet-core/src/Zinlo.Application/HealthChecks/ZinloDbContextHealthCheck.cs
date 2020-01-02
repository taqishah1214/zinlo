using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Zinlo.EntityFrameworkCore;

namespace Zinlo.HealthChecks
{
    public class ZinloDbContextHealthCheck : IHealthCheck
    {
        private readonly DatabaseCheckHelper _checkHelper;

        public ZinloDbContextHealthCheck(DatabaseCheckHelper checkHelper)
        {
            _checkHelper = checkHelper;
        }

        public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = new CancellationToken())
        {
            if (_checkHelper.Exist("db"))
            {
                return Task.FromResult(HealthCheckResult.Healthy("ZinloDbContext connected to database."));
            }

            return Task.FromResult(HealthCheckResult.Unhealthy("ZinloDbContext could not connect to database"));
        }
    }
}
