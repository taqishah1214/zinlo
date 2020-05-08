using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Zinlo.Authorization.Users.Dto;

namespace Zinlo.Authorization.Users
{
    public interface IInviteUserService
    {
        Task Create(CreateOrUpdateInviteUser input);
        Task<InviteUserDto> GetByEmail(string email);
    }
}
