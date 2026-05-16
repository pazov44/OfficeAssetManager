using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using OfficeAssetManager.Core.Configuration;
using OfficeAssetManager.Core.Domain.Entities;
using OfficeAssetManager.Core.ServiceContracts;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace OfficeAssetManager.Core.Services
{
    public class JwtService : IJwtService
    {
        private readonly JwtOptions _jwtOptions;

        private readonly UserManager<ApplicationUser> _userManager;
        public JwtService(JwtOptions jwtOptions, UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
            _jwtOptions = jwtOptions;
        }
        public async Task<string> GetJwtToken(ApplicationUser user)
        {

            //Create jwt claims
            List<Claim> claims = new()
            {
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.UserName!)
            };

            var roles = await _userManager.GetRolesAsync(user);

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            //Create security data for jwt, hashing + key
            SymmetricSecurityKey securityKey = new(
                Convert.FromBase64String(_jwtOptions.SecretKey));

            SigningCredentials credentials = new(securityKey, SecurityAlgorithms.HmacSha256);

            //token expiration
            DateTime expiration = DateTime.UtcNow.AddMinutes(
                Convert.ToDouble(_jwtOptions.TokenExpirationMinutes));

            JwtSecurityToken jwtGen = new(issuer: _jwtOptions.Issuer,
                audience: _jwtOptions.Audience,
                claims: claims,
                signingCredentials: credentials,
                expires: expiration
                );

            JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();

            string token = handler.WriteToken(jwtGen);

            return token;

        }

        public string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using var rng = System.Security.Cryptography.RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        public ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Convert.FromBase64String(_jwtOptions.SecretKey)),
                ValidateLifetime = false
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);

            return principal;
        }
    }
}
