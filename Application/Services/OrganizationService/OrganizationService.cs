using BookingSystem.Application.DTOs;
using BookingSystem.Core.Entities;
using BookingSystem.Infrastructure.Repositories;
using BookingSystem.Infrastructure.Repositories.UnitOfWork;
using Microsoft.Extensions.Logging;

namespace BookingSystem.Application.Services;

public class OrganizationService : IOrganizationService
{
    private readonly IOrganizationRepository _orgRepo;
    private readonly IOfficeRepository _officeRepo;
    private readonly ILogger<OrganizationService> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public OrganizationService(IOrganizationRepository orgRepo, IOfficeRepository officeRepo, ILogger<OrganizationService> logger, IUnitOfWork unitOfWork)
    {
        _orgRepo = orgRepo;
        _officeRepo = officeRepo;
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

    public async Task<OrganizationDto> CreateOrganizationAsync(CreateOrganizationRequest request)
    {
        _logger.LogInformation("Attempting to create an {@Name} organization", request.Name);

        var organization = new Organization(request.Name);
        await _orgRepo.AddAsync(organization);
        await _unitOfWork.SaveChangesAsync();
        _logger.LogInformation("Succesfully created an {@Name} organization", request.Name);
        return MapToDto(organization);



    }
    public async Task<OrganizationDto> UpdateOrganizationAsync(UpdateOrganizationRequest request)
    {
        _logger.LogInformation("Attempting to update an {@Name} organization {@OrganizationId}", request.Name, request.OrganizationId);

        var org = await _orgRepo.GetByIdAsync(request.OrganizationId);
        if (org == null) throw new KeyNotFoundException($"Organization with ID {request.OrganizationId} not found.");

        org.UpdateName(request.Name);

            //получить все офисы организации
            var officies = await _officeRepo.GetByIdsAsync(request.OfficesId);
            //проверка на наличие таких данных в бд вообще
            if (officies.Count() != request.OfficesId.Count())
            {
                var foundIds = officies.Select(r => r.Id);
                var notFoundIds = request.OfficesId.Except(foundIds);
                throw new ArgumentException($"One or more officies not found. Invalid IDs: {string.Join(", ", notFoundIds)}");
            }
            // Определяем, какие офисы УДАЛИТЬ
            var officiesToRemove = org.Officies
                .Where(currentRoom => !request.OfficesId.Contains(currentRoom.Id))
                .ToList();

            // Определяем, какие комнаты ДОБАВИТЬ
            var currentOfficeIds = org.Officies.Select(o => o.Id).ToList();
            var officiesToAdd = officies
                .Where(requestedOfficie => !currentOfficeIds.Contains(requestedOfficie.Id))
                .ToList();

            // Применяем изменения к доменной модели
            foreach (var room in officiesToRemove)
            {
                org.RemoveOffice(room);
            }
            foreach (var room in officiesToAdd)
            {
                org.AddOffice(room);
            }
        

        await _orgRepo.UpdateAsync(org);
        await _unitOfWork.SaveChangesAsync();
        _logger.LogInformation("Succesfully updated an {Name} organization", request.Name);
        return MapToDto(org);

    }

    public async Task DeleteOrganizationAsync(Guid id)
    {
        _logger.LogInformation("Try to delete organization with ID {@OfficeId}", id);

        var office = await _orgRepo.GetByIdAsync(id);
        if (office == null)
        {
            throw new KeyNotFoundException($" Organization with ID {id} not found.");
        }
        await _orgRepo.DeleteAsync(id);
        await _unitOfWork.SaveChangesAsync();
        _logger.LogInformation("Successfully deleted an organization with ID {@OfficeId}", office.Id);


    }
    public async Task<OrganizationDto> GetOrganizationByIdAsync(Guid id)
    {
        _logger.LogInformation("Try to find organization with ID {OfficeId}", id);

        var organization = await _orgRepo.GetByIdAsync(id);
        if (organization == null) throw new KeyNotFoundException($"Organization with ID {id} not found.");
        _logger.LogInformation("Succesfully find organization with ID {OfficeId}", id);
        return MapToDto(organization);

    }

    public async Task<IEnumerable<OrganizationDto>> GetAllOrganizationsAsync()
    {
        _logger.LogInformation("Try to get all organizations");

        var organization = await _orgRepo.GetAllAsync();
        return organization.Select(MapToDto);

    }

    private static OrganizationDto MapToDto(Organization org) => new()
    {
        Id = org.Id,
        Name = org.Name
    };
}