using System.Collections.Generic;
using System.Threading.Tasks;
using WeVeed.Domain.Entities.Toko;

namespace WeVeed.Domain.Services.Toko
{
    public interface ITokoRoomService
    {
        Task<string> CreateAsync(string roomType, int roomNumber, int numberOfAttendants);

        Task<TokoRoom> GetRoomByNumberAsync(string roomType, int roomNumber);

        Task<List<TokoRoom>> GetAllRoomsAsync(string roomType);

        Task<bool> AddAttendantToRoomAsync(string roomType, int roomNumber);

        Task<bool> RemoveAttendantFromRoomAsync(string roomType, int roomNumber);
    }
}
