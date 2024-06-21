using System;
using Course.Core.Entities;
using Course.Service.Dtos.UserDtos;
using Course.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Course.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController:ControllerBase
	{
        private readonly IAuthService _authService;
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;


        public AuthController(IAuthService authService,UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _authService = authService;
            _userManager = userManager;
            _roleManager = roleManager;

        }
        //[HttpGet("users")]
        //public async Task<IActionResult> CreateUser()
        //{
        //    AppUser user = new AppUser
        //    {
        //        FullName = "Elmar Qarayev",
        //        UserName = "elm111",
        //    };


        //    IdentityResult result = await _userManager.CreateAsync(user, "Elmar123");

        //    if (result.Succeeded)
        //    {
        //        return Ok(user.Id);
        //    }

        //    var errors = result.Errors.Select(e => e.Description);
        //    return BadRequest(new { Errors = errors });
        //}

        [HttpGet("users")]
        public async Task<IActionResult> CreateUser()
        {

            await _roleManager.CreateAsync(new IdentityRole("Admin"));
            await _roleManager.CreateAsync(new IdentityRole("Member"));


            AppUser user1 = new AppUser
            {
                FullName = "Admin",
                UserName = "admin",
            };

            await _userManager.CreateAsync(user1, "Admin123");

            AppUser user2 = new AppUser
            {
                FullName = "Member",
                UserName = "member",
            };

            await _userManager.CreateAsync(user2, "Member123");

            await _userManager.AddToRoleAsync(user1, "Admin");
            await _userManager.AddToRoleAsync(user2, "Member");

            return Ok(user1.Id);
        }


        [HttpPost("login")]
        public ActionResult Login(UserLoginDto loginDto)
        {
            var token = _authService.Login(loginDto);
            return Ok(new { token });
        }


        [Authorize]
        [HttpGet("profile")]
        public ActionResult Profile()
        {
            return Ok(User.Identity.Name);
        }

    }
}

