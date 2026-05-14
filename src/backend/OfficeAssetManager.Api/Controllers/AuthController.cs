using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OfficeAssetManager.Api.Helpers;
using OfficeAssetManager.Core.DTO;
using OfficeAssetManager.Core.ServiceContracts;

namespace OfficeAssetManager.Api.Controllers
{
    [AllowAnonymous]
    public class AuthController : ApiControllerBase
    {
        private readonly IAuthService _authService;
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]

        public async Task<IActionResult> Register(RegisterDto registerDto)
        {
            var authResponse = await _authService.Register(registerDto);

            if(authResponse.Success == false)
            {
                return BadRequest(authResponse);
            }
            return Ok(authResponse);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            var authResponse = await _authService.Login(loginDto);

            if (authResponse.Success == false)
            {
                return BadRequest(authResponse);
            }

            return Ok(authResponse);
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh(TokenRequestDto tokenRequestDto)
        {
            var authResponse = await _authService.RefreshToken(tokenRequestDto);

            if (authResponse.Success == false)
            {
                return BadRequest(authResponse);
            }

            return Ok(authResponse);
        }
    }
}
