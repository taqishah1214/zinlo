using System;

namespace Zinlo.Url
{
    public class NullAppUrlService : IAppUrlService
    {
        public static IAppUrlService Instance { get; } = new NullAppUrlService();

        private NullAppUrlService()
        {
            
        }

        public string CreateEmailActivationUrlFormat(int? tenantId)
        {
            throw new NotImplementedException();
        }

        public string CreatePasswordResetUrlFormat(int? tenantId)
        {
            throw new NotImplementedException();
        }

        public string CreateEmailActivationUrlFormat(string tenancyName)
        {
            throw new NotImplementedException();
        }

        public string CreatePasswordResetUrlFormat(string tenancyName)
        {
            throw new NotImplementedException();
        }

        public string CreateCustomPlanUrlFormat(int? tenantId)
        {
            throw new NotImplementedException();
        }

        public string CreateCustomPlanPaymentUrlFormat(string tenancyName)
        {
            throw new NotImplementedException();
        }

        public string CreateInviteUserUrlFormat(int? tenantId)
        {
            throw new NotImplementedException();
        }

        public string CreateInviteUserUrlFormat(string tenancyName)
        {
            throw new NotImplementedException();
        }
    }
}