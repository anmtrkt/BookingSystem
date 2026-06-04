using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace BookingSystem.Application.DTOs;
public class CreateUserRequest
{
    [Required]
    [EmailAddress]
    [NotNull]
    [MinLength(1)]
    [Display(Name = "Email")]
    public string Email { get; set; } = null!;

    [Required]
    [Phone]
    [Display(Name = "Телефон")]
    public string PhoneNumber { get; set; } = null!;

    [Required]
    [Display(Name = "Дата рождения")]
    public DateOnly BirthDate { get; set; }

    [Required]
    [DataType(DataType.Password)]
    [Display(Name = "Пароль")]
    public string Password { get; set; } = null!;

    [Required]
    [Compare("Password", ErrorMessage = "Пароли не совпадают")]
    [DataType(DataType.Password)]
    [Display(Name = "Подтвердить пароль")]
    public string PasswordConfirm { get; set; } = null!;

    [Required]
    [Display(Name = "Имя")]
    public string Name { get; set; } = null!;

    [Required]
    [Display(Name = "Фамилия")]
    public string Surname { get; set; } = null!;

    [Display(Name = "Отчество")]
    public string? Patronymic { get; set; }
    [Required]
    [Display(Name = "Должность")]
    public string Post { get; set; } = null!;
}



