using Abp.Modules;
using Abp.Reflection.Extensions;
using Castle.Windsor.MsDependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Zinlo.Configure;
using Zinlo.Startup;
using Zinlo.Test.Base;

namespace Zinlo.GraphQL.Tests
{
    [DependsOn(
        typeof(ZinloGraphQLModule),
        typeof(ZinloTestBaseModule))]
    public class ZinloGraphQLTestModule : AbpModule
    {
        public override void PreInitialize()
        {
            IServiceCollection services = new ServiceCollection();
            
            services.AddAndConfigureGraphQL();

            WindsorRegistrationHelper.CreateServiceProvider(IocManager.IocContainer, services);
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(ZinloGraphQLTestModule).GetAssembly());
        }
    }
}