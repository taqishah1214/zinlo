using Abp.AspNetCore.Mvc.Views;
using Abp.Runtime.Session;
using Microsoft.AspNetCore.Mvc.Razor.Internal;

namespace Zinlo.Web.Public.Views
{
    public abstract class ZinloRazorPage<TModel> : AbpRazorPage<TModel>
    {
        [RazorInject]
        public IAbpSession AbpSession { get; set; }

        protected ZinloRazorPage()
        {
            LocalizationSourceName = ZinloConsts.LocalizationSourceName;
        }
    }
}
