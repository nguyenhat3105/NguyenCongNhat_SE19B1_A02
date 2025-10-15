using FUMiniHotelManagement.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FUMiniHotelManagement.DAL.Repositories
{
    public class RoomRepository: IRoomRepository
    {
        public RoomRepository() { }
        public IEnumerable<RoomInformation> GetAll()
        {
            var context = new FuminiHotelManagementContext();
            return context.RoomInformations.ToList();
        }

        public RoomInformation? GetById(int roomId)
        {
            var context = new FuminiHotelManagementContext();
            return context.RoomInformations.Find(roomId);
        }

        public void Update(RoomInformation room)
        {
            var context = new FuminiHotelManagementContext();
            if (room == null)
            {
                throw new ArgumentNullException(nameof(room));
            }
            var existingRoom = context.RoomInformations.FirstOrDefault(r => r.RoomId == room.RoomId);
            if (existingRoom == null)
            {
                throw new InvalidOperationException("Không tìm thấy phòng để cập nhật");
            }
            // Cập nhật từng thuộc tính
            existingRoom.RoomNumber = room.RoomNumber;
            existingRoom.RoomPricePerDay = room.RoomPricePerDay;
            existingRoom.RoomStatus = room.RoomStatus;
            existingRoom.RoomMaxCapacity = room.RoomMaxCapacity;
            existingRoom.RoomDetailDescription = room.RoomDetailDescription;
            context.SaveChanges();
        }

        public void Delete(RoomInformation roomInformation)
        {
            var context = new FuminiHotelManagementContext();
            var room = context.RoomInformations.Find(roomInformation.RoomId);
            if (room != null)
            {
                context.RoomInformations.Remove(room);
                context.SaveChanges();
            }
        }
    }
}
