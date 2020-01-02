using Abp.AutoMapper;
using Abp.Modules;
using Abp.Reflection.Extensions;

namespace Zinlo
{
    [DependsOn(typeof(ZinloClientModule), typeof(AbpAutoMapperModule))]
    public class ZinloXamarinSharedModule : AbpModule
    {
        public override void PreInitialize()
        {
            Configuration.Localization.IsEnabled = false;
            Configuration.BackgroundJobs.IsJobExecutionEnabled = false;
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(ZinloXamarinSharedModule).GetAssembly());
        }
    }
}