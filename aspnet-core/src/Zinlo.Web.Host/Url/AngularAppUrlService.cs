using Abp.MultiTenancy;
using Zinlo.Url;

namespace Zinlo.Web.Url
{
    public class AngularAppUrlService : AppUrlServiceBase
    {
        public override string EmailActivationRoute => "account/confirm-email";
        public override string InViteUserRoute => "account/confirm-email";

        public override string PasswordResetRoute => "account/reset-password";
        public override string BuyCustomPlaRoute => "account/buy";

        public AngularAppUrlService(
                IWebUrlService webUrlService,
                ITenantCache tenantCache
            ) : base(
                webUrlService,
                tenantCache
            )
        {

        }
    }
}