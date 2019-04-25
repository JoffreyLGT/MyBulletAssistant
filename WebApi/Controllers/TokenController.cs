using AutoMapper;
using Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using WebApi.Data;
using WebApi.Data.Entities;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    public class TokenController : ControllerBase
    {
        private IConfiguration config;
        private readonly SignInManager<User> signInManager;
        private readonly IMbaRepository repository;
        private readonly IMapper mapper;

        public TokenController(IMbaRepository repository, IMapper mapper, IConfiguration config, SignInManager<User> signInManager)
        {
            this.config = config;
            this.signInManager = signInManager;
            this.repository = repository;
            this.mapper = mapper;
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> CreateToken([FromBody]LoginModel login)
        {
            var user = await Authenticate(login);

            if (user != null)
            {
                var tokenString = BuildToken(user);
                return Ok(new { user.Id, token = tokenString });
            }

            return Unauthorized();
        }

        private string BuildToken(UserModel user)
        {
            var claims = new[] {
                new Claim(JwtRegisteredClaimNames.UniqueName, user.Id),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(config["Jwt:Issuer"],
              config["Jwt:Issuer"],
              claims,
              expires: DateTime.Now.AddMinutes(30),
              signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private async Task<UserModel> Authenticate(LoginModel login)
        {
            var result = await signInManager.PasswordSignInAsync(login.Email, login.Password, true, lockoutOnFailure: false);
            if (result.Succeeded)
            {
                var user = await repository.GetUserByEmail(login.Email);
                if (user != null)
                {
                    return mapper.Map<User, UserModel>(user);
                }
            }
            return null;
        }
    }
}
