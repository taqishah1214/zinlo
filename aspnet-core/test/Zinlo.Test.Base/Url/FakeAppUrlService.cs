using Zinlo.Url;

namespace Zinlo.Test.Base.Url
{
    public class FakeAppUrlService : IAppUrlService
    {
        public string CreateEmailActivationUrlFormat(int? tenantId)
        {
            return "http://test.com/";
        }

        public string CreatePasswordResetUrlFormat(int? tenantId)
        {
            return "http://test.com/";
        }

        public string CreateEmailActivationUrlFormat(string tenancyName)
        {
            return "http://test.com/";
        }

        public string CreatePasswordResetUrlFormat(string tenancyName)
        {
            return "http://test.com/";
        }

        public string CreateCustomPlanUrlFormat(int? tenantId)
        {
            return "http://test.com/";
        }

        public string CreateCustomPlanPaymentUrlFormat(string tenancyName)
        {
            return "http://test.com/";
        }

        public string CreateInviteUserUrlFormat(int? tenantId)
        {
            return "http://test.com/";
        }

        public string CreateInviteUserUrlFormat(string tenancyName)
        {
            return "http://test.com/";
        }

        public string CreateEditionForSpecficCustomerUrlFormat(int? tenantId, string link)
        {
            return "http://test.com/";
        }
    }
}
