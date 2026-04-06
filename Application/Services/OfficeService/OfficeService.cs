using BookingSystem.Application.DTOs;
using BookingSystem.Core.Entities;
using BookingSystem.Domain.Interfaces;
using BookingSystem.Infrastructure.Repositories.UnitOfWork;
using Microsoft.Extensions.Logging;

namespace BookingSystem.Application.Services;

public class OfficeService : IOfficeService
{
    private readonly IOfficeRepository _officeRepo;
    private readonly IRoomRepository _roomRepo;
    private readonly IOrganizationRepository _organizationRepo;
    private readonly ILogger<OfficeService> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public OfficeService(IOfficeRepository officeRepo, IRoomRepository roomRepo, IOrganizationRepository organizationRepo,ILogger<OfficeService> logger, IUnitOfWork unitOfWork)
    {
        _officeRepo = officeRepo;
        _roomRepo = roomRepo;
        _organizationRepo = organizationRepo;
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

    public async Task<OfficeDto> CreateOfficeAsync(CreateOfficeRequest request)
    {
        _logger.LogInformation("Attempting to create an office for organization {@OrganizationId} at address {@Address}",
            request.OrganizationId, request.Address);
        var org = _organizationRepo.GetByIdAsync(request.OrganizationId);
        if (org is null) throw new KeyNotFoundException($"Organization with ID {request.OrganizationId} not found.");
        var office = new Office(request.Address, request.OrganizationId);
        await _officeRepo.AddAsync(office);
        await _unitOfWork.SaveChangesAsync();
        _logger.LogInformation("Successfully created office with ID {@OfficeId}", office.Id);
        return MapToDto(office);

    }

    public async Task<OfficeDto> UpdateOfficeAsync(UpdateOfficeRequest request)
    {
        _logger.LogInformation("Attempting to update an office {@OfficeId}", request.OfficeId);

        var office = await _officeRepo.GetByIdAsync(request.OfficeId); 
        if (office == null) throw new KeyNotFoundException($"Office with ID {request.OfficeId} not found.");
        office.ChangeAddress(request.Address);

  
            var requestedRooms = await _roomRepo.GetByIdsAsync(request.RoomsId);
            if (requestedRooms.Count() != request.RoomsId.Count())
            {
                var foundIds = requestedRooms.Select(r => r.Id);
                var notFoundIds = request.RoomsId.Except(foundIds);
                throw new ArgumentException($"One or more rooms not found. Invalid IDs: {string.Join(", ", notFoundIds)}");
            }


            // Определяем, какие комнаты УДАЛИТЬ
            var roomsToRemove = office.Rooms
                .Where(currentRoom => !request.RoomsId.Contains(currentRoom.Id))
                .ToList();

            // Определяем, какие комнаты ДОБАВИТЬ
            var currentRoomIds = office.Rooms.Select(r => r.Id).ToList();
            var roomsToAdd = requestedRooms
                .Where(requestedRoom => !currentRoomIds.Contains(requestedRoom.Id))
                .ToList();

            // Применяем изменения к доменной модели
            foreach (var room in roomsToRemove)
            {
                office.RemoveRoom(room);
            }
            foreach (var room in roomsToAdd)
            {
                office.AddRoom(room);
            }
        

        await _officeRepo.UpdateAsync(office);
        await _unitOfWork.SaveChangesAsync();
        _logger.LogInformation("Successfully updated office with ID {@OfficeId}", office.Id);
        return MapToDto(office);

    }
    public async Task DeleteOfficeAsync(Guid id)
    {
        _logger.LogInformation("Try to delete office with ID {@OfficeId}", id);

        var office = await _officeRepo.GetByIdAsync(id);
        if (office == null)
        {
            throw new KeyNotFoundException($"Office with ID {id} not found.");
        }
        await _officeRepo.DeleteAsync(id);
        await _unitOfWork.SaveChangesAsync();
        _logger.LogInformation("Successfully deleted an office with ID {@OfficeId}", office.Id);


    }

    public async Task<OfficeDto> GetOfficeByIdAsync(Guid id)
    {
        _logger.LogInformation("Try to find office with ID {@OfficeId}", id);

        var office = await _officeRepo.GetByIdAsync(id);
        if (office == null) throw new KeyNotFoundException($"Office with ID {id} not found.");
        _logger.LogInformation("Succesfully find office with ID {@OfficeId}", id);
        return MapToDto(office);

    }

    public async Task<IEnumerable<OfficeDto>> GetAllOfficesAsync()
    {
        _logger.LogInformation("Try to get all officies");

        var offices = await _officeRepo.GetAllAsync();
        return offices.Select(MapToDto);

    }

    private static OfficeDto MapToDto(Office office) => new()
    {
        Id = office.Id,
        Address = office.Address,
        Rooms = office.RoomsId
    };
}