using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Zinlo.Configuration;
using Zinlo.Web;

namespace Zinlo.EntityFrameworkCore
{
    /* This class is needed to run "dotnet ef ..." commands from command line on development. Not used anywhere else */
    public class ZinloDbContextFactory : IDesignTimeDbContextFactory<ZinloDbContext>
    {
        public ZinloDbContext CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<ZinloDbContext>();
            var configuration = AppConfigurations.Get(WebContentDirectoryFinder.CalculateContentRootFolder(), addUserSecrets: true);

            ZinloDbContextConfigurer.Configure(builder, configuration.GetConnectionString(ZinloConsts.ConnectionStringName));

            return new ZinloDbContext(builder.Options);
        }
    }
}