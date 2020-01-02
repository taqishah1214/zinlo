using Abp.Modules;
using Abp.Reflection.Extensions;

namespace Zinlo
{
    public class ZinloClientModule : AbpModule
    {
        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(ZinloClientModule).GetAssembly());
        }
    }
}
