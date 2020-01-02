using Abp.AutoMapper;
using Abp.Modules;
using Abp.Reflection.Extensions;
using Zinlo.Authorization;

namespace Zinlo
{
    /// <summary>
    /// Application layer module of the application.
    /// </summary>
    [DependsOn(
        typeof(ZinloApplicationSharedModule),
        typeof(ZinloCoreModule)
        )]
    public class ZinloApplicationModule : AbpModule
    {
        public override void PreInitialize()
        {
            //Adding authorization providers
            Configuration.Authorization.Providers.Add<AppAuthorizationProvider>();

            //Adding custom AutoMapper configuration
            Configuration.Modules.AbpAutoMapper().Configurators.Add(CustomDtoMapper.CreateMappings);
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(ZinloApplicationModule).GetAssembly());
        }
    }
}