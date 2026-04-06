using Microsoft.AspNetCore.Identity;

namespace BookingSystem.Core.Entities;

public class AppUser : IdentityUser<Guid>
{
    public string Post { get; private set; }
    public string Surname { get; private set; }
    public string Name { get; private set; }
    public string? Patronymic { get; private set; }
    public string FullName => string.Join(" ", Surname, Name, Patronymic);
    public string NormalizedSurname => Surname.ToUpperInvariant();
    public string NormalizedName => Name.ToUpperInvariant();
    public string? NormalizedPatronymic => Patronymic?.ToUpperInvariant();
    public string NormalizedFullName => FullName.ToUpperInvariant();
    public AppUser(string email,
        string post, string surname, string name, string? patronymic,
        string phoneNumber)
    {
        UserName = email;
        Email = email;
        Post = post;
        Surname = surname;
        Name = name;
        Patronymic = patronymic;
        Id = Guid.NewGuid();
        PhoneNumber = phoneNumber;
    }
    public void ChangeEmail(string email) => Email = email;
    public void ChangePhoneNumber(string phoneNumber) => PhoneNumber = phoneNumber;
    public void ChangePost(string post) => Post = post;
    public void ChangeSurname(string surname) => Surname = surname;
    public void ChangeName(string name) => Name = name;
    public void ChangePatronymic(string patronymic) => Patronymic = patronymic;


}
