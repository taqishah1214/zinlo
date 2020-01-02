using Abp.Modules;
using Abp.Reflection.Extensions;

namespace Zinlo
{
    [DependsOn(typeof(ZinloXamarinSharedModule))]
    public class ZinloXamarinIosModule : AbpModule
    {
        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(ZinloXamarinIosModule).GetAssembly());
        }
    }
}