using BookingSystem.Application.DTOs;
using BookingSystem.Core.Entities;
using BookingSystem.Core.ValueObjects;
using BookingSystem.Domain.Interfaces;
using BookingSystem.Infrastructure.Repositories.UnitOfWork;
using Microsoft.Extensions.Logging;

namespace BookingSystem.Application.Services;

public class RoomService : IRoomService
{
    private readonly IRoomRepository _roomRepo;
    private readonly IOfficeRepository _officeRepo;
    private readonly ILogger<RoomService> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public RoomService(IRoomRepository roomRepo, IOfficeRepository officeRepo, ILogger<RoomService> logger, IUnitOfWork unitOfWork)
    {
        _roomRepo = roomRepo;
        _officeRepo = officeRepo;
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

    public async Task<RoomDto> CreateRoomAsync(CreateRoomRequest request)
    { 
        _logger.LogInformation("Trying to create a room with number {@Number} in office {@OfficeId}", request.Number, request.OfficeId);
        Office? office = await _officeRepo.GetByIdAsync(request.OfficeId);
        if(office is null)
        {
            throw new KeyNotFoundException($"Cannot find office with ID: {request.OfficeId}");
        }
        Equipment eq = Equipment.Create(request.HasProjector, request.HasSoundproofing, request.HasWhiteboard, 
            request.HasInteractiveWhiteboard, request.HasVideoConferenceSystem, request.HasMicrophones, 
            request.HasWiFi, request.HasAirConditioning, request.HasTelevisions, request.NumberOfMicrophones, 
            request.NumberOfTelevisions, request.NumberOfComputers);
        var room = new Room(request.Number, office.Id, eq, request.CountOfPlaces);
        await _roomRepo.AddAsync(room);
        _logger.LogInformation("Succesfully create a room with Id {@RoomId}", room.Id);

        await _unitOfWork.SaveChangesAsync();

        return MapToDto(room);
    }

    public async Task<RoomDto> UpdateRoomAsync(UpdateRoomRequest request)
    {
        _logger.LogInformation("Trying to update a room with Id {@RoomId}", request.RoomId);
        var room = await _roomRepo.GetByIdAsync(request.RoomId);
        if (room == null) throw new KeyNotFoundException($"Room with ID {request.RoomId} not found.");

        if (!string.IsNullOrEmpty(request.Number))
            room.ChangeNumber(request.Number);
        Equipment eq = Equipment.Create(request.HasProjector, request.HasSoundproofing, request.HasWhiteboard,
    request.HasInteractiveWhiteboard, request.HasVideoConferenceSystem, request.HasMicrophones,
    request.HasWiFi, request.HasAirConditioning, request.HasTelevisions, request.NumberOfMicrophones,
    request.NumberOfTelevisions, request.NumberOfComputers);
        room.UpdateEquipment(eq);
        room.UpdateCountOfPlaces(request.CountOfPlaces);

        if (request.IsAvailable)
            room.SetAvailable();
        else
            room.SetUnavailable();

        await _roomRepo.UpdateAsync(room);
        _logger.LogInformation("Succesfully update a room with Id {@RoomId}", request.RoomId);
        await _unitOfWork.SaveChangesAsync();
        return MapToDto(room);
    }

    public async Task DeleteRoomAsync(Guid id)
    {
        var room = _roomRepo.GetByIdAsync(id);
        if (room == null) throw new KeyNotFoundException($"Room with ID {id} not found");

        await _roomRepo.DeleteAsync(id);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<RoomDto> GetRoomByIdAsync(Guid id)
    {
        var room = await _roomRepo.GetByIdAsync(id);
        if (room == null) throw new KeyNotFoundException($"Room with ID {id} not found.");
        return MapToDto(room);
    }

    public async Task<IEnumerable<RoomDto>> GetAllRoomsAsync()
    {
        var rooms = await _roomRepo.GetAllAsync();
        return rooms.Select(MapToDto);
    }

    public async Task<IEnumerable<RoomDto>> GetAvailableRoomsAsync(DateTime start, DateTime end)
    {
        var allRooms = await _roomRepo.GetAllAsync();
        var available = allRooms.Where(r => r.IsAvailable).ToList();
        return available.Select(MapToDto);
    }

    private static RoomDto MapToDto(Room room) => new()
    {
        Id = room.Id,
        Number = room.Number,
        CountOfPlaces = room.CountOfPlaces,
        IsAvailable = room.IsAvailable,
        OfficeId = room.OfficeId
    };
}