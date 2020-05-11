﻿using System.Threading.Tasks;
using Abp.Application.Services;
using Zinlo.Authorization.Accounts.Dto;
using Zinlo.Contactus.Dto;

namespace Zinlo.Authorization.Accounts
{
    public interface IAccountAppService : IApplicationService
    {

        //
        Task<UserLinkResolverDto> RegsiterLinkResolve(CustomTenantRequestLinkResolveInput input);
        Task<CustomTenantRequestLinkResolverDto> LinkResolve(CustomTenantRequestLinkResolveInput input);
        Task<IsTenantAvailableOutput> IsTenantAvailable(IsTenantAvailableInput input);

        Task<int?> ResolveTenantId(ResolveTenantIdInput input);

        Task<RegisterOutput> Register(RegisterInput input);
        Task<RegisterOutput> RegisterTenantUser(RegisterTenantUserInput input);

        Task SendPasswordResetCode(SendPasswordResetCodeInput input);

        Task<ResetPasswordOutput> ResetPassword(ResetPasswordInput input);

        Task SendEmailActivationLink(SendEmailActivationLinkInput input);

        Task ActivateEmail(ActivateEmailInput input);

        Task<ImpersonateOutput> Impersonate(ImpersonateInput input);

        Task<ImpersonateOutput> BackToImpersonator();

        Task<SwitchToLinkedAccountOutput> SwitchToLinkedAccount(SwitchToLinkedAccountInput input);
    }
}
