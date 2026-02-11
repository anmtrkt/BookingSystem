using BookingSystem.Application.DTOs;
namespace BookingSystem.Application.Services;
public interface IOrganizationService
{
    Task<OrganizationDto> CreateOrganizationAsync(CreateOrganizationRequest request);
    Task<OrganizationDto> UpdateOrganizationAsync(UpdateOrganizationRequest request);
    Task DeleteOrganizationAsync(Guid id);
    Task<OrganizationDto> GetOrganizationByIdAsync(Guid id);
    Task<IEnumerable<OrganizationDto>> GetAllOrganizationsAsync();
    
}