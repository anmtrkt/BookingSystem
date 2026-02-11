namespace BookingSystem.Core.Entities;

public class User : BaseEntity
{
    public string Post { get; private set; }
    public string Surname { get; private set; }
    public string Name { get; private set; }
    public string? Patronymic { get; private set; }
    public string FullName => string.Join(" ", Surname, Name, Patronymic);
    public string NormalizedSurname { get; private set; }
    public string NormalizedName { get; private set; }
    public string? NormalizedPatronymic { get; private set; }
    public string NormalizedFullName => FullName.ToUpper();
    public string Email { get; private set; }
    public string NormalizedEmail => Email.ToUpper();
    public string PhoneNumber { get; private set; }
    public User(string email,
        string post, string surname, string name, string? patronymic,
        string phoneNumber)
    {
        Email = email;
        Post = post;
        Surname = surname;
        Name = name;
        Patronymic = patronymic;
        Id = Guid.NewGuid();
        NormalizedSurname = surname.ToUpper();
        NormalizedName = name.ToUpper();
        NormalizedPatronymic = patronymic?.ToUpper();
        PhoneNumber = phoneNumber;
    }
    public void UpdateEmail(string email) => Email = email;
    public void UpdatePhoneNumber(string phoneNumber) => PhoneNumber = phoneNumber;
    public void ChangePost(string post) => Post = post;
    public void ChangeSurname(string surname) => Surname = surname;
    public void ChangeName(string name) => Name = name;


}
