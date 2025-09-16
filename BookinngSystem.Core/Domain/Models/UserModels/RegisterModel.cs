using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace BookingSystem.Core.Domain.Models.UserModels
{
    public class RegisterModel
    {
    [Required(ErrorMessage = "Имя обязательно для заполнения.")]
    [StringLength(50, ErrorMessage = "Имя не должно превышать 50 символов.")]
    public required string Name { get; set; }

    [Required(ErrorMessage = "Фамилия обязательна для заполнения.")]
    [StringLength(50, ErrorMessage = "Фамилия не должна превышать 50 символов.")]
    public required string Surname { get; set; }

    [StringLength(50, ErrorMessage = "Отчество не должно превышать 50 символов.")]
    public required string? Patronymic { get; set; }

    [Required(ErrorMessage = "Должность обязательна для заполнения.")]
    [StringLength(100, ErrorMessage = "Должность не должна превышать 100 символов.")]
    public required string Post { get; set; }

    [Required(ErrorMessage = "Пароль обязателен для заполнения.")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "Пароль должен содержать от 6 до 100 символов.")]
    public required string Password { get; set; }

    [Required(ErrorMessage = "Подтверждение пароля обязательно для заполнения.")]
    [Compare("Password", ErrorMessage = "Пароли не совпадают.")]
    public required string ConfirmPassword { get; set; }

    [EmailAddress(ErrorMessage = "Некорректный адрес электронной почты.")]
    public string? Email { get; set; }

    [Phone(ErrorMessage = "Некорректный номер телефона.")]
    public string? PhoneNumber { get; set; }
        [Required]
        public int? PostPriority { get; set; }
}
}
