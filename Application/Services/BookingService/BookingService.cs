using BookingSystem.Application.DTOs;
using BookingSystem.Application.Exceptions;
using BookingSystem.Core.Entities;
using BookingSystem.Core.Entities.Aggregates;
using BookingSystem.Domain.Interfaces;
using BookingSystem.Infrastructure.Repositories.UnitOfWork;
using Microsoft.Extensions.Logging;

namespace BookingSystem.Application.Services;

public class BookingService : IBookingService
{
    private readonly IBookingRepository _bookingRepo;
    private readonly IRoomRepository _roomRepo;
    private readonly IUserRepository _userRepo;
    private readonly ILogger<BookingService> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public BookingService(IBookingRepository bookingRepo, IRoomRepository roomRepo, IUserRepository userRepo, ILogger<BookingService> logger,IUnitOfWork unitOfWork)
    {
        _bookingRepo = bookingRepo;
        _roomRepo = roomRepo;
        _userRepo = userRepo;
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

    public async Task<BookingDto> CreateBookingAsync(CreateBookingRequest request)
    {
        _logger.LogInformation("Attempting to create an meeting in Room {@RoomId}, creator Id is {@UserId}, Time range: {@StartTime} -- {@EndTime}", request.RoomId, request.UserId, request.StartTime, request.EndTime);
        var room = await _roomRepo.GetByIdAsync(request.RoomId);
        if (room == null) throw new KeyNotFoundException($"Room with ID {request.RoomId} not found.");

        // Проверка конфликта по времени
        var existingBookings = await _bookingRepo.GetByRoomIdAsync(request.RoomId, request.StartTime, request.EndTime);
        if (existingBookings.Any(b => !b.IsCancelled))
        {
            throw new BookingConflictException("Room is already booked during this time.");
        }
        List<AppUser> Subscribers = new();
        //подписка всех указанных подписчиков
        if (request.SubscribersId.Any())
        {

            IEnumerable<AppUser> users = await _userRepo.GetByIdsAsync(request.SubscribersId);
            //если кто то не нашелся - эксепшен
            if (users.Count() != request.SubscribersId.Count())
            {
                var foundIds = users.Select(r => r.Id);
                var notFoundIds = request.SubscribersId.Except(foundIds);
                throw new ArgumentException($"One or more users not found. Invalid IDs: {string.Join(", ", notFoundIds)}");
            }
            foreach (var item in users)
            {
                Subscribers.Add(item);
            }
        }
        var creator = await _userRepo.GetByIdAsync(request.UserId);
        if (creator == null) throw new KeyNotFoundException($"User with ID {request.UserId} not found.");

        var meeting = new Meeting(creator.Id,
            room.Id, request.Purpose, request.StartTime, request.EndTime);

        await _bookingRepo.AddAsync(meeting);
        await _unitOfWork.SaveChangesAsync();
        _logger.LogInformation("Succesfully created an meeting with Id {@MeetingId}", meeting.Id );

        return MapToDto(meeting);
    }
    public async Task<BookingDto> UpdateBookingAsync(UpdateBookingRequest request)
    {
        _logger.LogInformation("Attempting to update an meeting {@BookingId}, Time range: {@StartTime} -- {@EndTime}", request.BookingId, request.StartTime, request.EndTime);

        var booking = await _bookingRepo.GetByIdAsyncWithInclude(request.BookingId);
        if (booking == null) throw new KeyNotFoundException($"Booking with ID {request.BookingId} not found.");

        
        var existingBookings = await _bookingRepo.GetByRoomIdAsync(booking.Room.Id, request.StartTime, request.EndTime);
        if (existingBookings.Any(b => b.Id != request.BookingId && !b.IsCancelled))
        {
            throw new BookingConflictException("Room is already booked during this time.");
        }

        booking.UpdateTimeRange(request.StartTime, request.EndTime);

            IEnumerable<AppUser> newUsers = await _userRepo.GetByIdsAsync(request.SubscribersId);
     
            //если кто то не нашелся - эксепшен
            if (newUsers.Count() != request.SubscribersId.Count())
            {
                var foundIds = newUsers.Select(r => r.Id);
                var notFoundIds = request.SubscribersId.Except(foundIds);
                throw new ArgumentException($"One or more users not found. Invalid IDs: {string.Join(", ", notFoundIds)}");
            }


                var newbie = newUsers.Where(u => !booking.Subscribers.Contains(u)).ToList();
                var unsubs = booking.Subscribers.Where(u => !newUsers.Contains(u)).ToList();
            foreach (var item in newbie)
            {
                booking.Subscribe(item);
            }
            foreach (var item in unsubs)
            {
                booking.Unsubscribe(item);
            }
        
        await _bookingRepo.UpdateAsync(booking);
        await _unitOfWork.SaveChangesAsync();
        _logger.LogInformation("Succesfully update an meeting {@BookingId}, Time range: {@StartTime} -- {@EndTime}", request.BookingId, request.StartTime, request.EndTime);

        return MapToDto(booking);
    }

    public async Task CancelBookingAsync(Guid bookingId)
    {
        _logger.LogInformation("Attempting to cancel an meeting {@BookingId}", bookingId);

        var booking = await _bookingRepo.GetByIdAsync(bookingId);
        if (booking == null) throw new KeyNotFoundException($"Booking with ID {bookingId} not found.");

        booking.Cancel();
        
        await _bookingRepo.UpdateAsync(booking);
        _logger.LogInformation("Succesfully cancel an meeting {@BookingId}", bookingId);
        await _unitOfWork.SaveChangesAsync();

    }

    public async Task<BookingDto> GetBookingByIdAsync(Guid bookingId)
    {
        _logger.LogInformation("Trying to find an meeting {@BookingId}", bookingId);

        var booking = await _bookingRepo.GetByIdAsync(bookingId);
        if (booking == null) throw new KeyNotFoundException($"Booking with ID {bookingId} not found.");
        _logger.LogInformation("Succesfully find an meeting {@BookingId}", bookingId);

        return MapToDto(booking);
    }

    public async Task<IEnumerable<BookingDto>> GetBookingsByRoomIdAsync(Guid roomId)
    {
        _logger.LogInformation("Trying to find an meetings by room {@RoomId}", roomId);
        var bookings = await _bookingRepo.GetByRoomIdAsync(roomId, DateTime.MinValue, DateTime.MaxValue);
        return bookings.Select(MapToDto);
    }

    public async Task<IEnumerable<BookingDto>> GetBookingsByUserIdAsync(Guid userId)
    {
        _logger.LogInformation("Trying to find an meetings by room {@RoomId}", userId);
        var bookings = await _bookingRepo.GetByUserIdAsync(userId);
        return bookings.Select(MapToDto);
    }

    private static BookingDto MapToDto(Meeting meeting)
    {

        return new()
        {
            Id = meeting.Id,
            RoomId = meeting.RoomId,
            CreatorId = meeting.CreatorId,
            StartTime = meeting.TimeRange.Start,
            EndTime = meeting.TimeRange.End,
            IsCancelled = meeting.IsCancelled,
            Purpose = meeting.Reason 
        };
    }
}

