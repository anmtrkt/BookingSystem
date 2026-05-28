using BookingSystem.Api.Services;
using BookingSystem.Core.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace BookingSystem.Tests.Controllers;

public class AuthControllerTests
{
    private readonly Mock<UserManager<AppUser>> _userManagerMock;
    private readonly Mock<TokenService> _tokenServiceMock;
    private readonly AuthController _controller;

    public AuthControllerTests()
    {
        var userStoreMock = new Mock<IUserStore<AppUser>>();
        _userManagerMock = new Mock<UserManager<AppUser>>(
            userStoreMock.Object, null, null, null, null, null, null, null, null);
        
        _tokenServiceMock = new Mock<TokenService>(
            Mock.Of<ILogger<TokenService>>(),
            Mock.Of<Microsoft.Extensions.Configuration.IConfiguration>());

        _controller = new AuthController(_userManagerMock.Object, _tokenServiceMock.Object);
    }

    #region Login Tests

    [Fact]
    public async Task Login_UserNotFound_ReturnsUnauthorized()
    {
        // Arrange
        var request = new Microsoft.AspNetCore.Identity.Data.LoginRequest
        {
            Email = "nonexistent@test.com",
            Password = "password123"
        };

        _userManagerMock.Setup(um => um.FindByEmailAsync(request.Email))
            .ReturnsAsync((AppUser?)null);

        // Act
        var result = await _controller.Login(request);

        // Assert
        var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
        Assert.Equal("Пользователь не найден.", unauthorizedResult.Value);
    }

    [Fact]
    public async Task Login_InvalidPassword_ReturnsUnauthorized()
    {
        // Arrange
        var request = new Microsoft.AspNetCore.Identity.Data.LoginRequest
        {
            Email = "user@test.com",
            Password = "wrongpassword"
        };

        var user = new AppUser { Email = request.Email, UserName = request.Email };
        
        _userManagerMock.Setup(um => um.FindByEmailAsync(request.Email))
            .ReturnsAsync(user);
        
        _userManagerMock.Setup(um => um.CheckPasswordAsync(user, request.Password))
            .ReturnsAsync(false);

        // Act
        var result = await _controller.Login(request);

        // Assert
        var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
        Assert.Equal("Неверный пароль.", unauthorizedResult.Value);
    }

    [Fact]
    public async Task Login_ValidCredentials_ReturnsOkWithCookie()
    {
        // Arrange
        var request = new Microsoft.AspNetCore.Identity.Data.LoginRequest
        {
            Email = "user@test.com",
            Password = "correctpassword"
        };

        var user = new AppUser { Email = request.Email, UserName = request.Email, Id = Guid.NewGuid().ToString() };
        var roles = new List<string> { "User" };
        var token = "fake_jwt_token";

        _userManagerMock.Setup(um => um.FindByEmailAsync(request.Email))
            .ReturnsAsync(user);
        
        _userManagerMock.Setup(um => um.CheckPasswordAsync(user, request.Password))
            .ReturnsAsync(true);
        
        _userManagerMock.Setup(um => um.GetRolesAsync(user))
            .ReturnsAsync(roles);
        
        _tokenServiceMock.Setup(ts => ts.GenerateToken(user, roles))
            .Returns(token);

        // Setup mock HttpResponse
        var httpContext = new DefaultHttpContext();
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = httpContext
        };

        // Act
        var result = await _controller.Login(request);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.NotNull(okResult.Value);
        
        // Verify cookie was set
        var responseCookie = httpContext.Response.Cookies["auth_token"];
        Assert.Equal(token, responseCookie);
    }

    [Fact]
    public async Task Login_NullEmail_ReturnsUnauthorized()
    {
        // Arrange
        var request = new Microsoft.AspNetCore.Identity.Data.LoginRequest
        {
            Email = "",
            Password = "password123"
        };

        _userManagerMock.Setup(um => um.FindByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync((AppUser?)null);

        // Act
        var result = await _controller.Login(request);

        // Assert
        Assert.IsType<UnauthorizedObjectResult>(result);
    }

    [Fact]
    public async Task Login_EmptyPassword_ReturnsUnauthorized()
    {
        // Arrange
        var request = new Microsoft.AspNetCore.Identity.Data.LoginRequest
        {
            Email = "user@test.com",
            Password = ""
        };

        var user = new AppUser { Email = request.Email, UserName = request.Email };
        
        _userManagerMock.Setup(um => um.FindByEmailAsync(request.Email))
            .ReturnsAsync(user);
        
        _userManagerMock.Setup(um => um.CheckPasswordAsync(user, request.Password))
            .ReturnsAsync(false);

        // Act
        var result = await _controller.Login(request);

        // Assert
        Assert.IsType<UnauthorizedObjectResult>(result);
    }

    #endregion
}
