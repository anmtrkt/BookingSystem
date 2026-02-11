using BookingSystem.Application.DTOs;

namespace BookingSystem.Application.Services;
public interface IBookingService
{
    Task<BookingDto> CreateBookingAsync(CreateBookingRequest request);
    Task<BookingDto> UpdateBookingAsync(UpdateBookingRequest request);
    Task CancelBookingAsync(Guid bookingId);
    Task<BookingDto> GetBookingByIdAsync(Guid id);
    Task<IEnumerable<BookingDto>> GetBookingsByRoomIdAsync(Guid roomId);
    Task<IEnumerable<BookingDto>> GetBookingsByUserIdAsync(Guid userId);
}