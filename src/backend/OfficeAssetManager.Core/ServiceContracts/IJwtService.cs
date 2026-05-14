using OfficeAssetManager.Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace OfficeAssetManager.Core.ServiceContracts
{
    public interface IJwtService
    {
        /// <summary>
        /// Generates a JWT containing user claims and roles.
        /// </summary>
        /// <param name="user">The application user entity for whom the token is generated.</param>
        /// <returns>A string representing the encoded JWT.</returns>
        public Task<string> GetJwtToken(ApplicationUser user);

        public string GenerateRefreshToken();
        public ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
    }
}
