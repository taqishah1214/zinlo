using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Zinlo.MultiTenancy.Dto;
using Zinlo.Register_User.Dto;

namespace Zinlo.Register_User
{
    public interface IUserRegisterService
    {
        Task<RegisterTenantOutput> RegisterUserWithTenant(RegisterUserInput registerUser);
    }
}
