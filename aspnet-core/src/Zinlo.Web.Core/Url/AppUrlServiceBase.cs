using Abp.Dependency;
using Abp.Extensions;
using Abp.MultiTenancy;
using Zinlo.Url;

namespace Zinlo.Web.Url
{
    public abstract class AppUrlServiceBase : IAppUrlService, ITransientDependency
    {
        public abstract string EmailActivationRoute { get; }

        public abstract string PasswordResetRoute { get; }
        public abstract string BuyCustomPlaRoute { get; }
        public abstract string InViteUserRoute { get; }
        public abstract string EditionForSpecficConsumerRoute { get; }


        protected readonly IWebUrlService WebUrlService;
        protected readonly ITenantCache TenantCache;

        protected AppUrlServiceBase(IWebUrlService webUrlService, ITenantCache tenantCache)
        {
            WebUrlService = webUrlService;
            TenantCache = tenantCache;
        }

        public string CreateEmailActivationUrlFormat(int? tenantId)
        {
            return CreateEmailActivationUrlFormat(GetTenancyName(tenantId));
        }

        public string CreatePasswordResetUrlFormat(int? tenantId)
        {
            return CreatePasswordResetUrlFormat(GetTenancyName(tenantId));
        }
        public string CreateEditionForSpecficCustomerUrlFormat(int? tenantId,string link)
        {
            return CreateEditionForSpecficCustomerUrlFormat(GetTenancyName(tenantId), link);
        }
        public string CreateCustomPlanUrlFormat(int? tenantId)
        {
            return CreateCustomPlanPaymentUrlFormat(GetTenancyName(tenantId));
        }

        public string CreateInviteUserUrlFormat(int? tenantId)
        {
            return CreateInviteUserUrlFormat(GetTenancyName(tenantId));
        }

        public string CreateInviteUserUrlFormat(string tenancyName)
        {
            var activationLink = WebUrlService.GetSiteRootAddress(tenancyName).EnsureEndsWith('/') + InViteUserRoute + "?tenantId={tenantId}&email={email}";

            return activationLink;
        }

        public string CreateEmailActivationUrlFormat(string tenancyName)
        {
            var activationLink = WebUrlService.GetSiteRootAddress(tenancyName).EnsureEndsWith('/') + EmailActivationRoute + "?userId={userId}&confirmationCode={confirmationCode}";

            if (tenancyName != null)
            {
                activationLink = activationLink + "&tenantId={tenantId}";
            }

            return activationLink;
        }

        public string CreateCustomPlanPaymentUrlFormat(string tenancyName)
        {
            var activationLink = WebUrlService.GetSiteRootAddress(tenancyName).EnsureEndsWith('/') + BuyCustomPlaRoute + "?tenantId={tenantId}&editionId={editionId}&subscriptionStartType={subscriptionStartType}&editionPaymentType={editionPaymentType}&price={price}&commitment={commitment}";
            return activationLink;
        }

        public string CreatePasswordResetUrlFormat(string tenancyName)
        {
            var resetLink = WebUrlService.GetSiteRootAddress(tenancyName).EnsureEndsWith('/') + PasswordResetRoute + "?userId={userId}&resetCode={resetCode}";

            if (tenancyName != null)
            {
                resetLink = resetLink + "&tenantId={tenantId}";
            }

            return resetLink;
        }

        public string CreateEditionForSpecficCustomerUrlFormat(string tenancyName,string link)
        {
            var resetLink = WebUrlService.GetSiteRootAddress(tenancyName).EnsureEndsWith('/') + EditionForSpecficConsumerRoute + link;

            return resetLink;
        }

        private string GetTenancyName(int? tenantId)
        {
            return tenantId.HasValue ? TenantCache.Get(tenantId.Value).TenancyName : null;
        }

        
    }
}