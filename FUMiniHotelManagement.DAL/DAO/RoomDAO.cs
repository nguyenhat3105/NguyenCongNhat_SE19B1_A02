using FUMiniHotelManagement.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FUMiniHotelManagement.DAL.DAO
{
    public static class RoomDAO
    {
        
        private static List<RoomInformation> _rooms = new List<RoomInformation>
        {
            new RoomInformation { RoomId = 1, RoomNumber = "2364", RoomTypeId = 1, RoomPricePerDay = 149.00m, RoomStatus = 1, RoomMaxCapacity = 3, RoomDetailDescription = "A basic room" },
            new RoomInformation { RoomId = 2, RoomNumber = "3345", RoomTypeId = 3, RoomPricePerDay = 299.00m, RoomStatus = 1, RoomMaxCapacity = 5, RoomDetailDescription = "Deluxe rooms offer additional features" },
            new RoomInformation { RoomId = 3, RoomNumber = "4432", RoomTypeId = 2, RoomPricePerDay = 199.00m, RoomStatus = 1, RoomMaxCapacity = 4, RoomDetailDescription = "A luxurious and spacious room" },
        };

        private static int _nextRoomId = 4;

        public static IEnumerable<RoomInformation> GetAll()
        {
            return _rooms
                .Where(r => r.RoomStatus == 1)
                .Select(r =>
                {
                    // ✅ SỬ DỤNG: Gọi RoomTypeDAO để map RoomType
                    // (Giả lập Include)
                    r.RoomType = RoomTypeDAO.GetById(r.RoomTypeId);
                    return r;
                }).ToList();
        }

        public static RoomInformation? GetById(int roomId)
        {
            var room = _rooms.FirstOrDefault(r => r.RoomId == roomId);
            if (room != null)
            {
                // ✅ SỬ DỤNG: Gọi RoomTypeDAO để map RoomType
                room.RoomType = RoomTypeDAO.GetById(room.RoomTypeId);
            }
            return room;
        }

        public static void Create(RoomInformation room)
        {
            room.RoomId = _nextRoomId++;
            _rooms.Add(room);
        }

        public static void Update(RoomInformation room)
        {
            var existing = _rooms.FirstOrDefault(r => r.RoomId == room.RoomId);
            if (existing != null)
            {
                // Map dữ liệu thủ công
                existing.RoomNumber = room.RoomNumber;
                existing.RoomPricePerDay = room.RoomPricePerDay;
                existing.RoomStatus = room.RoomStatus;
                existing.RoomMaxCapacity = room.RoomMaxCapacity;
                existing.RoomDetailDescription = room.RoomDetailDescription;
                existing.RoomTypeId = room.RoomTypeId;
            }
        }

        public static void Delete(int roomId)
        {
            var room = _rooms.FirstOrDefault(r => r.RoomId == roomId);
            if (room != null)
            {
                _rooms.Remove(room);
            }
        }
    }
}
