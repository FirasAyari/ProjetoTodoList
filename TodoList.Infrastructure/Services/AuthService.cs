using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using TodoList.Application.DTOs;
using TodoList.Application.Interfaces;

namespace TodoList.Infrastructure.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IConfiguration _configuration;

        public AuthService(UserManager<IdentityUser> userManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _configuration = configuration;
        }

        public async Task<AuthResultDto> LoginAsync(UserLoginDto loginDto)
        {
            var user = await _userManager.FindByNameAsync(loginDto.UserName);
            if (user == null)
            {
                return new AuthResultDto { Succeeded = false, Errors = new[] { "Invalid username or password." } };
            }

            var isPasswordValid = await _userManager.CheckPasswordAsync(user, loginDto.Password);
            if (!isPasswordValid)
            {
                return new AuthResultDto { Succeeded = false, Errors = new[] { "Invalid username or password." } };
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]
                 ?? throw new InvalidOperationException("JWT Key not configured"));

            var tokenExpiry = DateTime.UtcNow.AddHours(1);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                    new Claim(JwtRegisteredClaimNames.Name, user.UserName!), 
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                }),
                Expires = tokenExpiry,
                Issuer = _configuration["Jwt:Issuer"],
                Audience = _configuration["Jwt:Audience"],
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var securityToken = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(securityToken);

            return new AuthResultDto
            {
                Succeeded = true,
                Token = tokenString,
                Expiration = tokenExpiry,
                User = new UserDto { Id = user.Id, UserName = user.UserName! } 
            };
        }

        public async Task<AuthResultDto> RegisterAsync(UserRegisterDto registerDto)
        {
            var existingUser = await _userManager.FindByNameAsync(registerDto.UserName);
            if (existingUser != null)
            {
                return new AuthResultDto { Succeeded = false, Errors = new[] { "Username already exists." } };
            }

            var user = new IdentityUser
            {
                UserName = registerDto.UserName,
            };

            var result = await _userManager.CreateAsync(user, registerDto.Password);

            if (!result.Succeeded)
            {
                return new AuthResultDto { Succeeded = false, Errors = result.Errors.Select(e => e.Description) };
            }

            var createdUserDto = new UserDto { Id = user.Id, UserName = user.UserName };

            return new AuthResultDto { Succeeded = true, User = createdUserDto };
        }
    }
}