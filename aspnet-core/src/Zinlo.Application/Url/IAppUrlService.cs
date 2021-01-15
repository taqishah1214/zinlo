namespace Zinlo.Url
{
    public interface IAppUrlService
    {
        string CreateEmailActivationUrlFormat(int? tenantId);

        string CreatePasswordResetUrlFormat(int? tenantId);

        string CreateEditionForSpecficCustomerUrlFormat(int? tenantId,string link);

        string CreateCustomPlanUrlFormat(int? tenantId);
        string CreateInviteUserUrlFormat(int? tenantId);
        string CreateInviteUserUrlFormat(string tenancyName);

        string CreateEmailActivationUrlFormat(string tenancyName);

        string CreatePasswordResetUrlFormat(string tenancyName);
        string CreateCustomPlanPaymentUrlFormat(string tenancyName);
    }
}
