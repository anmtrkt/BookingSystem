using BookingSystem.Application.DTOs;
using BookingSystem.Core.Entities;
using BookingSystem.Domain.Interfaces;
using BookingSystem.Infrastructure.Repositories.UnitOfWork;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging; // Не забудьте добавить using для ILogger

namespace BookingSystem.Application.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepo;
    private readonly ILogger<UserService> _logger;
    private readonly IUnitOfWork _unitOfWork;
    private readonly INotificationService _notificationService;


    public UserService(IUserRepository userRepo, ILogger<UserService> logger, IUnitOfWork unitOfWork, INotificationService notificationService)
    {
        _userRepo = userRepo;
        _logger = logger;
        _unitOfWork = unitOfWork;
        _notificationService = notificationService;
    }

    public async Task<AppUser> CreateUserAsync(CreateUserRequest request)
    {
        _logger.LogInformation("Attempting to create a user with email {Email}", request.Email);

        var user = new AppUser(
            request.Email,
            request.Post,
            request.Surname,
            request.Name,
            request.Patronymic,
            request.PhoneNumber
        );
        
        await _userRepo.AddAsync(user, request.Password);
        await _unitOfWork.SaveChangesAsync(); // Сохраняем изменения

        // Отправка уведомления о регистрации
        await _notificationService.SendRegistrationEmailAsync(user.Id);

        _logger.LogInformation("Successfully created user with ID {UserId}", user.Id);
        return user;

    }

    public async Task<UserDto> UpdateUserAsync(UpdateUserRequest request)
    {
        _logger.LogInformation("Attempting to update user with ID {UserId}", request.UserId);

        var user = await _userRepo.GetByIdAsync(request.UserId);
        if (user == null)
        {
            throw new KeyNotFoundException($"User with ID {request.UserId} not found.");
        }

        if (!string.IsNullOrEmpty(request.Email)) user.ChangeEmail(request.Email);
        if (!string.IsNullOrEmpty(request.Name)) user.ChangeName(request.Name);
        if (!string.IsNullOrEmpty(request.Surname)) user.ChangeSurname(request.Surname);
        if (!string.IsNullOrEmpty(request.Post)) user.ChangePost(request.Post);
        if (!string.IsNullOrEmpty(request.PhoneNumber)) user.ChangePhoneNumber(request.PhoneNumber);

        await _userRepo.UpdateAsync(user);
        await _unitOfWork.SaveChangesAsync(); 

        _logger.LogInformation("Successfully updated user with ID {UserId}", user.Id);
        return MapToDto(user);

    }

    public async Task DeleteUserAsync(Guid id)
    {
        _logger.LogInformation("Attempting to delete user with ID {UserId}", id);

        var user = await _userRepo.GetByIdAsync(id);
        if (user == null)
        {
            throw new KeyNotFoundException($"User with ID {id} not found.");
        }

        await _userRepo.DeleteAsync(id);
        await _unitOfWork.SaveChangesAsync(); // Сохраняем изменения

        _logger.LogInformation("Successfully deleted user with ID {UserId}", id);

    }

    public async Task<UserDto> GetUserByIdAsync(Guid id)
    {
        _logger.LogInformation("Attempting to find user by ID {UserId}", id);

        var user = await _userRepo.GetByIdAsync(id);
        if (user == null)
        {
            throw new KeyNotFoundException($"User with ID {id} not found.");
        }

        _logger.LogInformation("Successfully found user with ID {UserId}", id);
        return MapToDto(user);

    }

    public async Task<UserDto> GetUserByEmailAsync(string email)
    {
        _logger.LogInformation("Attempting to find user by email {Email}", email);

        var user = await _userRepo.GetByEmailAsync(email);
        if (user == null)
        {
            throw new KeyNotFoundException($"User with email {email} not found.");
        }

        _logger.LogInformation("Successfully found user with email {Email}", email);
        return MapToDto(user);

    }

    public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
    {
        _logger.LogInformation("Attempting to get all users");

        var users = await _userRepo.GetAllAsync();
        _logger.LogInformation("Successfully retrieved {UserCount} users", users.Count());
        return users.Select(MapToDto);

    }

    private static UserDto MapToDto(AppUser user) => new()
    {
        Id = user.Id,
        Name = user.Name,
        Surname = user.Surname,
        Email = user.Email,
        Post = user.Post,
        Patronymic = user.Patronymic, 
        PhoneNumber = user.PhoneNumber,
    };
}
