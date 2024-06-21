using System;
using Course.Service.Dtos.UserDtos;

namespace Course.Service.Interfaces
{
	public interface IAuthService
	{
        string Login(UserLoginDto loginDto);

    }
}

