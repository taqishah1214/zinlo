using Abp.Modules;
using Abp.Reflection.Extensions;

namespace Zinlo
{
    [DependsOn(typeof(ZinloXamarinSharedModule))]
    public class ZinloXamarinAndroidModule : AbpModule
    {
        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(ZinloXamarinAndroidModule).GetAssembly());
        }
    }
}