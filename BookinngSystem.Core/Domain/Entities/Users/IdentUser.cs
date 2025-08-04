using BookingSystem.Core.Domain.Common;
using BookingSystem.Core.Domain.Entities.Aggregates;

using BookingSystem.Core.Domain.Entities.Institutions;
using BookingSystem.Core.Domain.Entities.Notifications;
using BookingSystem.Core.Domain.Events;
using BookingSystem.Core.Domain.Events.UserEvents;
using BookingSystem.Core.Domain.Models.UserModels;
using BookingSystem.Core.Domain.ValueObjects;
using BookingSystem.Core.Utils;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingSystem.Core.Domain.Entities.Users
{
    public class User : IdentityUser<Guid>
    {
        public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
        public DateTime? ModifiedAt { get; private set; }
        public bool IsArchive { get; private set; } = false;
        public string Post { get; private set; }
        public string LastPost { get; private set; } = "";
        public string Surname { get; private set; }
        public string Name { get; private set; }
        public string? Patronymic { get; private set; }
        public string FullName { get; private set; }
        public string NormalizedSurname { get; private set; }
        public string NormalizedName { get; private set; }
        public string? NormalizedPatronymic { get; private set; }
        public string NormalizedFullName { get; private set; }
        public override string? NormalizedEmail { get; set; }
        public bool IsManager { get; private set; }
        public string? RefreshToken { get; set; }
        public DateTime RefreshTokenExpiryTime { get; set; }


        public Guid InstitutionId { get; private set; }
        public Institution Institution { get; private set; }


        private readonly List<Guid> _managedUsersId  = new();
        public List<Guid> ManagerUsersId => _managedUsersId;
        private readonly List<User> _managedUsers = new();
        public List<User> ManagedUsers => _managedUsers;


        private readonly List<Guid> _meetingsId = new();
        public List<Guid> CreatedMeetingsId => _meetingsId;

        private readonly List<Meeting> _meetings = new();
        public List<Meeting> CreatedMeetings => _meetings;
        private readonly List<Guid> _notificationsId = new();
        public List<Guid> NotificationsId => _notificationsId;
        private readonly List<Notification> _notifications = new();
        public List<Notification> Notifications => _notifications;
#pragma warning disable CS8618 // Поле, не допускающее значения NULL, должно содержать значение, отличное от NULL, при выходе из конструктора. Рассмотрите возможность добавления модификатора "required" или объявления значения, допускающего значение NULL.
        private User() { }
#pragma warning restore CS8618 // Поле, не допускающее значения NULL, должно содержать значение, отличное от NULL, при выходе из конструктора. Рассмотрите возможность добавления модификатора "required" или объявления значения, допускающего значение NULL.
        private User(DateTime createdAt, DateTime? modifiedAt,
            string post, string lastPost, 
            string surname, string name, string? patronymic, 
            string fullName, string normalizedSurname, string normalizedName, 
            string? normalizedPatronymic, string normalizedFullName, string? normalizedEmail, 
            Guid id, Institution institution, string? phoneNumber = null)
        {
            CreatedAt = createdAt;
            ModifiedAt = modifiedAt;
            IsArchive = false;
            Post = post;
            LastPost = lastPost;
            Surname = surname;
            Name = name;
            Patronymic = patronymic;
            FullName = fullName;
            NormalizedSurname = normalizedSurname;
            NormalizedName = normalizedName;
            NormalizedPatronymic = normalizedPatronymic;
            NormalizedFullName = normalizedFullName;
            NormalizedEmail = normalizedEmail;
            Id = id;
            InstitutionId = institution.Id;
            Institution = institution;
            PhoneNumber = phoneNumber;
            UserName = Name;
            
        }
        public static User Create( string name, string surname, string? patronymic, string? phoneNumber, string? email, Institution institution, string post, string lastPost = "")
        {
            string normalizedName = name.ToUpper();
            string normalizedSurname = surname.ToUpper();
            string? normalizedPatronymic = patronymic?.ToUpper();
            string fullname = String.Join(" ", surname, name, patronymic).Trim();
            string normalizedFullname = fullname.ToUpper();
            string? normPhoneNNumb = ContactValidator.ValidatePhoneNumber(phoneNumber); 
            string? normalizedEmail = ContactValidator.ValidateEmail(email);
            DateTime modifiedAt = DateTime.UtcNow;
            DateTime createdAt = DateTime.UtcNow;
            Guid id = Guid.NewGuid();
            

            return new User(createdAt, modifiedAt, post, 
                lastPost, surname, name, patronymic,
                fullname, normalizedSurname, normalizedName, 
                normalizedPatronymic, normalizedFullname, 
                normalizedEmail, id, institution, phoneNumber)
            { Email = email};
        }
        public static UserDto TransformToDto(User user)
        {
            return new UserDto { 
                Id = user.Id,
                Email = user.Email ??= string.Empty,
                CreatedAt = user.CreatedAt,
                ModifiedAt = user.ModifiedAt,
                Post = user.Post,
                LastPost = user.LastPost,
                FullName = user.FullName,
                IsManager = user.IsManager,
                Institution = Institution.TransformToDto(user.Institution),
            };

        }
        public static List<UserDto> TransformToDto(IEnumerable<User> users)
        {
            List<UserDto> result = new(users.Count());
            foreach (var user in users)
            {
                result.Add(TransformToDto(user));
            }
            return result;
        }
        public void AddNotification(Notification notification)
        {
            Notifications.Add(notification);
            NotificationsId.Add(notification.Id);
        }
        public void SetArchive()
        {
            IsArchive = true;
            MarkAsModified();
        }
        public void SetUnarchive()
        {
            IsArchive = false;
            MarkAsModified();
        }

        public void AddManagedUsers(List<User> user)
        {
            _managedUsers.AddRange(user);
            user.ForEach(u => _managedUsersId.Add(u.Id));
        }
        public void AddManagedUser(User user)
        {
            _managedUsers.Add(user);
            _managedUsersId.Add(user.Id);
        }

        protected void UpdateEmail(string email)
        {
            ContactValidator.ValidateEmail(email);
            Email = email;
            NormalizedEmail = ContactValidator.NormalizeEmail(email);
            MarkAsModified();
        }
        
        protected void UpdatePhoneNumber(string phoneNumber)
        {
            ContactValidator.ValidatePhoneNumber(phoneNumber);
            PhoneNumber = phoneNumber;
            MarkAsModified();
        }
        public void ChangePost(string Post)
        {
            LastPost = this.Post;
            this.Post = Post;
            DomainEvents.Raise<UserInfoUpdateEvent>(new UserInfoUpdateEvent(Id, Post, LastPost));
        }

        public void UpdateInstitution(Institution institution)
        {
            DomainEvents.Raise<UserUpdateInstitutionEvent>(new UserUpdateInstitutionEvent(Id, InstitutionId, Institution.Id));
            InstitutionId = institution.Id;
            Institution = institution;

            MarkAsModified();
        }
        public void MarkAsModified()
        {
            ModifiedAt = DateTime.UtcNow;
        }

    }
}
