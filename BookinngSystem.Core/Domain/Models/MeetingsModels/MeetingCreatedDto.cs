using System.ComponentModel.DataAnnotations;

namespace BookingSystem.Core.Domain.Models.MeetingsModels
{
    public class MeetingCreatedDto
    {
        [Required(ErrorMessage = "RoomId is required")]
        public Guid RoomId { get; set; }

        [Required(ErrorMessage = "InstitutionId is required")]
        public Guid InstitutionId { get; set; }

        [Required(ErrorMessage = "Start time is required")]
        public DateTime StartTime { get; set; }

        [Required(ErrorMessage = "End time is required")]
        public DateTime EndTime { get; set; }

        public bool IsOverridden { get; set; } = false;

        [MinLength(0)]
        public List<Guid> SubscriberIds { get; set; } = new List<Guid>();
    }
}

