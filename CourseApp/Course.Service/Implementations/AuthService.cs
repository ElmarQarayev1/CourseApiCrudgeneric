using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Course.Core.Entities;
using Course.Service.Dtos.UserDtos;
using Course.Service.Exceptions;
using Course.Service.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Course.Service.Implementations
{
	public class AuthService:IAuthService
	{
        private readonly UserManager<AppUser> _userManager;
        private readonly IConfiguration _configuration;

        public AuthService(UserManager<AppUser> userManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _configuration = configuration;
        }

        public string Login(UserLoginDto loginDto)
        {
            AppUser? user = _userManager.FindByNameAsync(loginDto.UserName).Result;

            if (user == null || !_userManager.CheckPasswordAsync(user, loginDto.Password).Result) throw new RestException(StatusCodes.Status401Unauthorized, "UserName or Password incorrect!");

            List<Claim> claims = new List<Claim>();
            claims.Add(new Claim(ClaimTypes.NameIdentifier, user.Id));
            claims.Add(new Claim(ClaimTypes.Name, user.UserName));
            claims.Add(new Claim("FullName", user.FullName));

            var roles = _userManager.GetRolesAsync(user).Result;

            claims.AddRange(roles.Select(x => new Claim(ClaimTypes.Role, x)).ToList());

            string secret = _configuration.GetSection("JWT:Secret").Value;

            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(secret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            JwtSecurityToken token = new JwtSecurityToken(
                claims: claims,
                signingCredentials: creds,
                issuer: _configuration.GetSection("JWT:Issuer").Value,
                audience: _configuration.GetSection("JWT:Audience").Value,
                expires: DateTime.Now.AddDays(2)
                );

            string tokenStr = new JwtSecurityTokenHandler().WriteToken(token);

            return tokenStr;
        }
    }
}

