using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WeVeed.Domain.Entities.Toko;

namespace WeVeed.Domain.Services.Toko
{
    public class TokoRoomService : ITokoRoomService
    {
        private IMongoCollection<TokoRoom> _tokoRoomCollection;

        public TokoRoomService(IMongoDatabase mongoDatabase)
        {
            _tokoRoomCollection = mongoDatabase.GetCollection<TokoRoom>("tokoroom");
        }

        public async Task<string> CreateAsync(string roomType, int roomNumber, int numberOfAttendants)
        {
            var newTokoRoom = new TokoRoom
            {
                RoomType = roomType,
                RoomNumber = roomNumber,
                NumberOfAttendants = numberOfAttendants,
                CreatedDate = DateTime.Now,
                ModifiedDate = DateTime.Now
            };

            await _tokoRoomCollection.InsertOneAsync(newTokoRoom);
            return newTokoRoom.Id.ToString();
        }

        public async Task<TokoRoom> GetRoomByNumberAsync(string roomType, int roomNumber)
        {
            var filter = Builders<TokoRoom>.Filter.Eq(a => a.RoomNumber, roomNumber) & Builders<TokoRoom>.Filter.Eq(a => a.RoomType, roomType);
            var room = (await _tokoRoomCollection.FindAsync(filter)).FirstOrDefault();
            return room;
        }

        public async Task<List<TokoRoom>> GetAllRoomsAsync(string roomType)
        {
            var filter = Builders<TokoRoom>.Filter.Eq(a => a.RoomType, roomType);
            var rooms = (await _tokoRoomCollection.FindAsync(filter)).ToList();

            if (rooms == null || !rooms.Any())
            {
                await CreateAsync(roomType, 1, 0);
                await CreateAsync(roomType, 2, 0);
                await CreateAsync(roomType, 3, 0);
                await CreateAsync(roomType, 4, 0);

                rooms = (await _tokoRoomCollection.FindAsync(filter)).ToList();
            }

            return rooms;
        }

        public async Task<bool> AddAttendantToRoomAsync(string roomType, int roomNumber)
        {
            var filter = Builders<TokoRoom>.Filter.Eq(a => a.RoomType, roomType) & Builders<TokoRoom>.Filter.Eq(a => a.RoomNumber, roomNumber);
            var increment = Builders<TokoRoom>.Update.Inc(a => a.NumberOfAttendants, 1);
            var updateResult = await _tokoRoomCollection.UpdateOneAsync(filter, increment);

            return updateResult.IsAcknowledged;
        }

        public async Task<bool> RemoveAttendantFromRoomAsync(string roomType, int roomNumber)
        {
            var room = await GetRoomByNumberAsync(roomType, roomNumber);
            if(room.NumberOfAttendants == 0)
            {
                return true;
            }

            var filter = Builders<TokoRoom>.Filter.Eq(a => a.RoomType, roomType) & Builders<TokoRoom>.Filter.Eq(a => a.RoomNumber, roomNumber);
            var increment = Builders<TokoRoom>.Update.Inc(a => a.NumberOfAttendants, -1);
            var updateResult = await _tokoRoomCollection.UpdateOneAsync(filter, increment);

            return updateResult.IsAcknowledged;
        }
    }
}
