using Application.DTOs.RoomDTOs;
using BookingSystem.Application.DTOs;

namespace BookingSystem.Application.Services;
public interface IRoomService
{
    public Task<RoomDto> CreateRoomAsync(CreateRoomRequest request);
    public Task<RoomDto> UpdateRoomAsync(UpdateRoomRequest request);
    public Task DeleteRoomAsync(Guid id);
    public Task<RoomDto> GetRoomByIdAsync(Guid id);
    public Task<IEnumerable<RoomDto>> GetAllRoomsAsync();
    public Task<IEnumerable<RoomDto>> SearchByFilterAsync(FilterRoomDto filter);
    public Task<IEnumerable<RoomDto>> GetAvailableRoomsAsync(DateTime start, DateTime end);
}