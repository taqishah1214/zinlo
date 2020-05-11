using Abp.Authorization;
using Abp.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zinlo.Authorization.Users.Dto;
using Zinlo.Url;

namespace Zinlo.Authorization.Users.InviteUsers
{
    public class InviteUserService : ZinloAppServiceBase, IInviteUserService
    {
        public IAppUrlService AppUrlService { get; set; }
        private readonly IRepository<InviteUser, long> _inviteUserRepostiry;
        private readonly IUserEmailer _userEmailer;
        public InviteUserService(IRepository<InviteUser, long> inviteUserRepostiry,
             IUserEmailer userEmailer)
        {
            _inviteUserRepostiry = inviteUserRepostiry;
            _userEmailer = userEmailer;
            AppUrlService = NullAppUrlService.Instance;
        }
        [AbpAuthorize(AppPermissions.Pages_Administration_Users_Edit)]
        public async Task Create(CreateOrUpdateInviteUser input)
        {
            var user=ObjectMapper.Map<InviteUser>(input);
           user.TenantId= AbpSession.TenantId.Value;
            var response = await _inviteUserRepostiry.InsertAsync(user);

            //send email
            await _userEmailer.SendInviteUserEmail(response.Email, response.TenantId, AppUrlService.CreateInviteUserUrlFormat(response.TenantId));

        }

        public async Task<InviteUserDto> GetByEmail(string email)
        {
            return ObjectMapper.Map<InviteUserDto>(_inviteUserRepostiry.GetAll().FirstOrDefault(x => x.Email.Equals(email)));
        }
    }
}
