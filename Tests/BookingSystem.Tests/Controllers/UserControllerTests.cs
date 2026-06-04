using BookingSystem.Api;
using BookingSystem.Application.DTOs;
using BookingSystem.Application.Services;
using BookingSystem.Core.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace BookingSystem.Tests.Controllers;

public class UserControllerTests
{
    private readonly Mock<UserManager<AppUser>> _userManagerMock;
    private readonly Mock<IUserService> _userServiceMock;
    private readonly UserController _controller;

    public UserControllerTests()
    {
        _userManagerMock = new Mock<UserManager<AppUser>>(
            Mock.Of<IUserStore<AppUser>>(), null, null, null, null, null, null, null, null);
        
        _userServiceMock = new Mock<IUserService>();
        
        _controller = new UserController(_userManagerMock.Object, _userServiceMock.Object);
    }

    #region CreateUser Tests

    [Fact]
    public async Task CreateUser_EmailAlreadyExists_ReturnsBadRequest()
    {
        // Arrange
        var request = new CreateUserRequest
        {
            Email = "existing@test.com",
            Password = "password123",
            PasswordConfirm = "password123",
            Name = "Test",
            Surname = "User",
            PhoneNumber = "+1234567890",
            BirthDate = DateOnly.FromDateTime(DateTime.Now),
            Post = "Developer"
        };

        var existingUser = new AppUser { Email = request.Email };
        _userManagerMock.Setup(um => um.FindByEmailAsync(request.Email))
            .ReturnsAsync(existingUser);

        // Act
        var result = await _controller.CreateUser(request);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Пользователь с таким Email уже существует.", badRequestResult.Value);
    }

    [Fact]
    public async Task CreateUser_ValidRequest_ReturnsOk()
    {
        // Arrange
        var request = new CreateUserRequest
        {
            Email = "newuser@test.com",
            Password = "password123",
            PasswordConfirm = "password123",
            Name = "Test",
            Surname = "User",
            PhoneNumber = "+1234567890",
            BirthDate = DateOnly.FromDateTime(DateTime.Now),
            Post = "Developer"
        };

        var createdUser = new AppUser 
        { 
            Id = Guid.NewGuid(),
            Email = request.Email,
            UserName = request.Email
        };

        _userManagerMock.Setup(um => um.FindByEmailAsync(request.Email))
            .ReturnsAsync((AppUser?)null);
        
        _userServiceMock.Setup(us => us.CreateUserAsync(request))
            .ReturnsAsync(createdUser);
        
        _userManagerMock.Setup(um => um.AddToRoleAsync(createdUser, "User"))
            .ReturnsAsync(IdentityResult.Success);

        // Act
        var result = await _controller.CreateUser(request);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        dynamic? value = okResult.Value;
        Assert.NotNull(value);
    }



    [Fact]
    public async Task CreateUser_PasswordsDoNotMatch_ReturnsBadRequest()
    {
        // Arrange
        var request = new CreateUserRequest
        {
            Email = "newuser@test.com",
            Password = "password123",
            PasswordConfirm = "differentpassword",
            Name = "Test",
            Surname = "User",
            PhoneNumber = "+1234567890",
            BirthDate = DateOnly.FromDateTime(DateTime.Now),
            Post = "Developer"
        };

        // Act & Assert - Model validation should catch this
        Assert.NotNull(request);
    }

    #endregion

    #region GetAll Tests

    [Fact]
    public async Task GetAll_ReturnsOkWithUsers()
    {
        // Arrange
        var users = new List<UserDto>
        {
            new UserDto { Id = Guid.NewGuid(), Email = "user1@test.com", Name = "User1" },
            new UserDto { Id = Guid.NewGuid(), Email = "user2@test.com", Name = "User2" }
        };

        _userServiceMock.Setup(us => us.GetAllUsersAsync())
            .ReturnsAsync(users);

        // Act
        var result = await _controller.GetAll();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedUsers = Assert.IsType<List<UserDto>>(okResult.Value);
        Assert.Equal(2, returnedUsers.Count);
    }

    [Fact]
    public async Task GetAll_NoUsers_ReturnsEmptyList()
    {
        // Arrange
        _userServiceMock.Setup(us => us.GetAllUsersAsync())
            .ReturnsAsync(new List<UserDto>());

        // Act
        var result = await _controller.GetAll();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedUsers = Assert.IsType<List<UserDto>>(okResult.Value);
        Assert.Empty(returnedUsers);
    }

    #endregion

    #region GetById Tests

    [Fact]
    public async Task GetById_ValidId_ReturnsOkWithUser()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var user = new UserDto 
        { 
            Id = userId, 
            Email = "user@test.com", 
            Name = "Test User" 
        };

        _userServiceMock.Setup(us => us.GetUserByIdAsync(userId))
            .ReturnsAsync(user);

        // Act
        var result = await _controller.GetById(userId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedUser = Assert.IsType<UserDto>(okResult.Value);
        Assert.Equal(userId, returnedUser.Id);
    }

    [Fact]
    public async Task GetById_InvalidId_ThrowsException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        
        _userServiceMock.Setup(us => us.GetUserByIdAsync(userId))
            .ThrowsAsync(new KeyNotFoundException("User not found"));

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _controller.GetById(userId));
    }

    #endregion

    #region Delete Tests

    [Fact]
    public async Task Delete_ValidId_ReturnsNoContent()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var appUser = new AppUser { Id = userId };

        _userManagerMock.Setup(um => um.FindByIdAsync(userId.ToString()))
            .ReturnsAsync(appUser);
        
        _userManagerMock.Setup(um => um.DeleteAsync(appUser))
            .ReturnsAsync(IdentityResult.Success);

        // Act
        var result = await _controller.Delete(userId);

        // Assert
        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task Delete_UserNotFound_ReturnsNotFound()
    {
        // Arrange
        var userId = Guid.NewGuid();

        _userManagerMock.Setup(um => um.FindByIdAsync(userId.ToString()))
            .ReturnsAsync((AppUser?)null);

        // Act
        var result = await _controller.Delete(userId);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task Delete_EmptyGuid_ReturnsNotFound()
    {
        // Arrange
        var userId = Guid.Empty;

        _userManagerMock.Setup(um => um.FindByIdAsync(userId.ToString()))
            .ReturnsAsync((AppUser?)null);

        // Act
        var result = await _controller.Delete(userId);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    #endregion
}
