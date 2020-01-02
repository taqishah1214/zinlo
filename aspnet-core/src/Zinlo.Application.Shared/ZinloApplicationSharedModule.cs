using Abp.Modules;
using Abp.Reflection.Extensions;

namespace Zinlo
{
    [DependsOn(typeof(ZinloCoreSharedModule))]
    public class ZinloApplicationSharedModule : AbpModule
    {
        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(ZinloApplicationSharedModule).GetAssembly());
        }
    }
}