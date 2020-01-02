using Abp.AutoMapper;
using Abp.Modules;
using Abp.Reflection.Extensions;

namespace Zinlo.Startup
{
    [DependsOn(typeof(ZinloCoreModule))]
    public class ZinloGraphQLModule : AbpModule
    {
        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(ZinloGraphQLModule).GetAssembly());
        }

        public override void PreInitialize()
        {
            base.PreInitialize();

            //Adding custom AutoMapper configuration
            Configuration.Modules.AbpAutoMapper().Configurators.Add(CustomDtoMapper.CreateMappings);
        }
    }
}