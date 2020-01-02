using Abp.AspNetCore.Mvc.Views;

namespace Zinlo.Web.Views
{
    public abstract class ZinloRazorPage<TModel> : AbpRazorPage<TModel>
    {
        protected ZinloRazorPage()
        {
            LocalizationSourceName = ZinloConsts.LocalizationSourceName;
        }
    }
}
