using AutoMapper;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WeVeed.Application.Dtos.Toko;
using WeVeed.Domain.Services.Toko;

namespace WeVeed.Application.Services.Toko
{
    public class TokoRoomAppService : ITokoRoomAppService
    {
        private readonly ITokoRoomService _tokoRoomService;

        public TokoRoomAppService(ITokoRoomService tokoRoomService)
        {
            _tokoRoomService = tokoRoomService;
        }

        public async Task<string> CreateAsync(string roomType, int roomNumber, int numberOfAttendants)
        {
            return (await _tokoRoomService.CreateAsync(roomType, roomNumber, numberOfAttendants));
        }

        public async Task<List<TokoRoomViewDto>> GetAllRoomsAsync(string roomType)
        {
            var rooms = await _tokoRoomService.GetAllRoomsAsync(roomType);
            var dtos = rooms.Select(a => Mapper.Map<TokoRoomViewDto>(a)).ToList();
            return dtos;
        }

        public async Task<bool> AddAttendantToRoomAsync(string roomType, int roomNumber)
        {
            var isSuccess = await _tokoRoomService.AddAttendantToRoomAsync(roomType, roomNumber);
            return isSuccess;
        }

        public async Task<bool> RemoveAttendantFromRoomAsync(string roomType, int roomNumber)
        {
            var isSuccess = await _tokoRoomService.RemoveAttendantFromRoomAsync(roomType, roomNumber);
            return isSuccess;
        }
    }
}
