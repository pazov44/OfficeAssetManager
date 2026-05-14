using OfficeAssetManager.Core.Domain.Entities;
using OfficeAssetManager.Core.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OfficeAssetManager.Core.ServiceContracts
{
    public interface IAuthService
    {
        /// <summary>
        /// Registers a new user and returns authentication details.
        /// </summary>
        Task<AuthResponseDto> Register(RegisterDto model);

        /// <summary>
        /// Authenticates a user and generates a JWT token.
        /// </summary>
        Task<AuthResponseDto> Login(LoginDto model);

        /// <summary>
        /// Checks if the specified email is already registered.
        /// </summary>
        Task<bool> EmailExists(string email);

        Task<AuthResponseDto> RefreshToken(TokenRequestDto model);
    }
}
