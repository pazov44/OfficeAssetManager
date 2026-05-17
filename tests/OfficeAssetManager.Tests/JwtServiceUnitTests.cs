using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Moq;
using OfficeAssetManager.Core.Configuration;
using OfficeAssetManager.Core.Domain.Entities;
using OfficeAssetManager.Core.Services;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;

namespace OfficeAssetManager.Tests
{
    public class JwtServiceTests
    {
        private readonly Mock<UserManager<ApplicationUser>> _userManagerMock;
        private readonly JwtService _jwtService;

        // 32 byte secret key
        private const string TestKey = "d2Bpr4gLb/uypsaQKK9CWr1RUiEuNJInT0GjliJ+0/I=";

        public JwtServiceTests()
        {
            var jwtOptions = new JwtOptions
            {
                SecretKey = TestKey,
                Issuer = "TestIssuer",
                Audience = "TestAudience",
                TokenExpirationMinutes = 30,
                RefreshTokenExpirationDays = 7
            };

            var store = new Mock<IUserStore<ApplicationUser>>();
            _userManagerMock = new Mock<UserManager<ApplicationUser>>(store.Object, null!, null!, null!, null!, null!, null!, null!, null!);

            _jwtService = new JwtService(jwtOptions, _userManagerMock.Object);
        }

        [Fact]
        public async Task GetJwtToken_ShouldReturnValidToken_WhenUserIsValid()
        {
            // Arrange
            var user = new ApplicationUser { Id = 1, UserName = "testuser" };
            _userManagerMock.Setup(m => m.GetRolesAsync(user))
                            .ReturnsAsync(new List<string> { "User" });

            // Act
            var token = await _jwtService.GetJwtToken(user);

            // Assert
            Assert.False(string.IsNullOrEmpty(token));

            var principal = _jwtService.GetPrincipalFromExpiredToken(token);
            Assert.Equal("testuser", principal.Identity?.Name);
        }

        [Fact]
        public void GenerateRefreshToken_ShouldReturnSecureString()
        {
            // Act
            var token1 = _jwtService.GenerateRefreshToken();
            var token2 = _jwtService.GenerateRefreshToken();

            // Assert
            Assert.NotNull(token1);
            // Check uniqueness
            Assert.NotEqual(token1, token2);
            // Check complexity
            Assert.True(token1.Length > 20);
        }

        [Fact]
        public async Task GetPrincipalFromExpiredToken_ShouldRecoverClaims_EvenIfExpired()
        {
            // Arrange
            var user = new ApplicationUser { Id = 1, UserName = "expiredUser" };
            _userManagerMock.Setup(m => m.GetRolesAsync(user)).ReturnsAsync(new List<string>());

            // Create a token
            var token = await _jwtService.GetJwtToken(user);

            // Act
            var principal = _jwtService.GetPrincipalFromExpiredToken(token);

            // Assert
            var nameClaim = principal.FindFirst(ClaimTypes.Name)?.Value;
            Assert.Equal("expiredUser", nameClaim);
        }
    }
}