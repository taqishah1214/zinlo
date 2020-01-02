using Abp.Modules;
using Abp.Reflection.Extensions;

namespace Zinlo
{
    public class ZinloCoreSharedModule : AbpModule
    {
        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(ZinloCoreSharedModule).GetAssembly());
        }
    }
}