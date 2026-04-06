using System;
using System.Collections.Generic;
using System.Text;

namespace BookingSystem.Application.DTOs;

public class NotificationDto
{
    public required string Title { get; set; }
    public required string Description { get; set; }
    public DateTime Created { get; set; } = DateTime.Now;
    public Guid ReceiverId { get; set; }
    public required string Sender { get; set; }
    public bool IsReaded {get; set;}
}
