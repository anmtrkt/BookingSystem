using BookingSystem.Application.DTOs;

namespace BookingSystem.Application.Services;

public interface IOfficeService
{
    Task<OfficeDto> CreateOfficeAsync(CreateOfficeRequest request);
    Task<OfficeDto> UpdateOfficeAsync(UpdateOfficeRequest request);
    Task DeleteOfficeAsync(Guid id);
    Task<OfficeDto> GetOfficeByIdAsync(Guid id);
    Task<IEnumerable<OfficeDto>> GetAllOfficesAsync();
}