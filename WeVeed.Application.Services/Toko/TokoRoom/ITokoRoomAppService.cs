using System.Collections.Generic;
using System.Threading.Tasks;
using WeVeed.Application.Dtos.Toko;

namespace WeVeed.Application.Services.Toko
{
    public interface ITokoRoomAppService
    {
        Task<string> CreateAsync(string roomType, int roomNumber, int numberOfAttendants);

        Task<List<TokoRoomViewDto>> GetAllRoomsAsync(string roomType);

        Task<bool> AddAttendantToRoomAsync(string roomType, int roomNumber);

        Task<bool> RemoveAttendantFromRoomAsync(string roomType, int roomNumber);
    }
}
