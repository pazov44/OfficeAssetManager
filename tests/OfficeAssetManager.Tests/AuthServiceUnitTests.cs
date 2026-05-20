using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Moq;
using OfficeAssetManager.Core.Configuration;
using OfficeAssetManager.Core.Domain.Entities;
using OfficeAssetManager.Core.DTO;
using OfficeAssetManager.Core.ServiceContracts;
using OfficeAssetManager.Core.Services;
using System.Security.Claims;
using Xunit;

namespace OfficeAssetManager.Tests
{
    public class AuthServiceTests
    {
        private readonly Mock<UserManager<ApplicationUser>> _userManagerMock;
        private readonly Mock<IJwtService> _jwtServiceMock;
        private readonly JwtOptions _jwtOptionsInstance;
        private readonly AuthService _authService;

        public AuthServiceTests()
        {
            // Boilerplate for Mocking UserManager
            var store = new Mock<IUserStore<ApplicationUser>>();
            _userManagerMock = new Mock<UserManager<ApplicationUser>>(store.Object, null!, null!, null!, null!, null!, null!, null!, null!);

            _jwtServiceMock = new Mock<IJwtService>();

            // Using a real POCO instance instead of a mock to bypass non-overridable exceptions
            _jwtOptionsInstance = new JwtOptions
            {
                RefreshTokenExpirationDays = 7
            };

            _authService = new AuthService(_userManagerMock.Object, _jwtServiceMock.Object, _jwtOptionsInstance);
        }

        [Fact]
        public async Task Login_ShouldReturnSuccess_WhenCredentialsAreValid()
        {
            // Arrange
            var loginDto = new LoginDto { UserName = "testuser", Password = "Password123!" };
            var user = new ApplicationUser { UserName = "testuser", Email = "test@example.com" };

            _userManagerMock.Setup(m => m.FindByNameAsync(loginDto.UserName))
                .ReturnsAsync(user);
            _userManagerMock.Setup(m => m.CheckPasswordAsync(user, loginDto.Password))
                .ReturnsAsync(true);
            _userManagerMock.Setup(m => m.UpdateAsync(It.IsAny<ApplicationUser>()))
                .ReturnsAsync(IdentityResult.Success);

            _jwtServiceMock.Setup(j => j.GetJwtToken(user)).ReturnsAsync("fake-jwt-token");
            _jwtServiceMock.Setup(j => j.GenerateRefreshToken()).Returns("fake-refresh-token");

            // Act
            var result = await _authService.Login(loginDto);

            // Assert

            // Explicitly cast to verify polymorphic response
            var successResult = Assert.IsType<AuthResponseSuccessDto>(result);
            Assert.Equal("fake-jwt-token", successResult.Token);
            Assert.Equal("fake-refresh-token", successResult.RefreshToken);

            _userManagerMock.Verify(m => m.UpdateAsync(It.Is<ApplicationUser>(u => u.RefreshToken == "fake-refresh-token")), Times.Once);
        }

        [Fact]
        public async Task Login_ShouldReturnFailure_WhenPasswordIsIncorrect()
        {
            // Arrange
            var loginDto = new LoginDto { UserName = "testuser", Password = "wrongpassword" };
            var user = new ApplicationUser { UserName = "testuser" };

            _userManagerMock.Setup(m => m.FindByNameAsync(loginDto.UserName))
                .ReturnsAsync(user);
            _userManagerMock.Setup(m => m.CheckPasswordAsync(user, loginDto.Password))
                .ReturnsAsync(false);

            // Act
            var result = await _authService.Login(loginDto);

            // Assert
            Assert.Equal("Invalid username or password", result.Message);
        }

        [Fact]
        public async Task RefreshToken_ShouldReturnNewTokens_WhenValid()
        {
            // Arrange
            var tokenRequest = new TokenRequestDto { Token = "expired-jwt", RefreshToken = "valid-refresh" };
            var user = new ApplicationUser
            {
                UserName = "testuser",
                RefreshToken = "valid-refresh",
                RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(1)
            };

            // Mock Extracting identity from token
            var claims = new List<Claim> { new Claim(ClaimTypes.Name, "testuser") };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var principal = new ClaimsPrincipal(identity);

            _jwtServiceMock.Setup(j => j.GetPrincipalFromExpiredToken(tokenRequest.Token))
                .Returns(principal);
            _userManagerMock.Setup(m => m.FindByNameAsync("testuser"))
                .ReturnsAsync(user);
            _jwtServiceMock.Setup(j => j.GetJwtToken(user)).ReturnsAsync("new-jwt-token");
            _jwtServiceMock.Setup(j => j.GenerateRefreshToken()).Returns("new-refresh-token");

            // Act
            var result = await _authService.RefreshToken(tokenRequest);

            // Assert

            // Explicitly cast to verify polymorphic response
            var successResult = Assert.IsType<AuthResponseSuccessDto>(result);
            Assert.Equal("new-jwt-token", successResult.Token);
            Assert.Equal("new-refresh-token", successResult.RefreshToken); // FIXED: Changed from "fake-refresh-token" to "new-refresh-token"

            _userManagerMock.Verify(m => m.UpdateAsync(user), Times.Once);
        }
    }
}