using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Zinlo.Register_User.Dto;

namespace Zinlo.Register_User
{
    public interface IUserRegisterService
    {
        Task RegisterUser(RegisterUserDto registerUser);
    }
}
