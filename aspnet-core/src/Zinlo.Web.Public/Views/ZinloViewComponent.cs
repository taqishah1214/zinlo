using Abp.AspNetCore.Mvc.ViewComponents;

namespace Zinlo.Web.Public.Views
{
    public abstract class ZinloViewComponent : AbpViewComponent
    {
        protected ZinloViewComponent()
        {
            LocalizationSourceName = ZinloConsts.LocalizationSourceName;
        }
    }
}