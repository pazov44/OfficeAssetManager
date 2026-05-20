using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Configuration;
using OfficeAssetManager.Core.Configuration;
using OfficeAssetManager.Core.Domain.Entities;
using OfficeAssetManager.Core.DTO;
using OfficeAssetManager.Core.ServiceContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OfficeAssetManager.Core.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;

        private readonly IJwtService _jwtService;

        private readonly JwtOptions _jwtOptions;

        public AuthService(UserManager<ApplicationUser> userManager, IJwtService jwtService, JwtOptions jwtOptions)
        {
            _userManager = userManager;
            _jwtService = jwtService;
            _jwtOptions = jwtOptions;
        }

        public async Task<bool> EmailExists(string email)
        {
            bool exists = await _userManager.FindByEmailAsync(email) != null;

            return exists;
        }
        public async Task<AuthResponseDto> Login(LoginDto model)
        {
            var user = await _userManager.FindByNameAsync(model.UserName);

            if (user == null || !await _userManager.CheckPasswordAsync(user, model.Password))
            {
                return new AuthResponseFailDto { Message = "Invalid username or password" };
            }

            string token = await _jwtService.GetJwtToken(user);
            string refreshToken = _jwtService.GenerateRefreshToken();

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(Convert.ToDouble(_jwtOptions.RefreshTokenExpirationDays));
            await _userManager.UpdateAsync(user);

            return new AuthResponseSuccessDto()
            {            
                Token = token,
                RefreshToken = refreshToken,
                Email = user.Email!,
                Message = "Login is successful"
            };
        }

        public async Task<AuthResponseDto> Register(RegisterDto model)
        {
            bool anyUserExists = _userManager.Users.Count() > 0;

            ApplicationUser user = new()
            {
                UserName = model.UserName,
                Email = model.Email
            };

            IdentityResult res = await _userManager.CreateAsync(user, model.Password);

            if (res.Succeeded)
            {
                if (anyUserExists == false)
                {
                    await _userManager.AddToRoleAsync(user, "Admin");
                }
                else
                {
                    await _userManager.AddToRoleAsync(user, "User");
                }

                string token = await _jwtService.GetJwtToken(user);
                string refreshToken = _jwtService.GenerateRefreshToken();

                user.RefreshToken = refreshToken;
                user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(Convert.ToDouble(_jwtOptions.RefreshTokenExpirationDays));
                await _userManager.UpdateAsync(user);

                return new AuthResponseSuccessDto
                {                
                    Token = token,
                    RefreshToken = refreshToken,
                    Email = user.Email,
                    Message = "Registration successful"
                };
            }

            return new AuthResponseFailDto
            {               
                Message = "Registration failed with validation errors.",
                Errors = res.Errors.Select(e => e.Description)
            };
        }

        public async Task<AuthResponseDto> RefreshToken(TokenRequestDto model)
        {
            var principal = _jwtService.GetPrincipalFromExpiredToken(model.Token);
            var username = principal.Identity?.Name;

            if (string.IsNullOrEmpty(username))
            {
                return new AuthResponseFailDto {   Message = "Invalid token claims" };
            }

            var user = await _userManager.FindByNameAsync(username);

            if (user == null ||
                user.RefreshToken != model.RefreshToken ||
                user.RefreshTokenExpiryTime <= DateTime.UtcNow)
            {
                return new AuthResponseFailDto
                {                
                    Message = "Invalid or expired refresh token. Please login again."
                };
            }

            var newJwtToken = await _jwtService.GetJwtToken(user);
            var newRefreshToken = _jwtService.GenerateRefreshToken();

            user.RefreshToken = newRefreshToken;
            await _userManager.UpdateAsync(user);

            return new AuthResponseSuccessDto
            {                
                Token = newJwtToken,
                RefreshToken = newRefreshToken,
                Email = user.Email!,
                Message = "Token refreshed successfully"
            };
        }
    }
}
