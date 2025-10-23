using FUMiniHotelManagement.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FUMiniHotelManagement.DAL.DAO
{
    public static class RoomTypeDAO
    {
        // Dữ liệu giả lập tĩnh cho RoomType
        private static List<RoomType> _roomTypes = new List<RoomType>
        {
            new RoomType { RoomTypeId = 1, RoomTypeName = "Standard room", TypeDescription = "Phòng tiêu chuẩn", TypeNote = "Bao gồm bữa sáng" },
            new RoomType { RoomTypeId = 2, RoomTypeName = "Suite", TypeDescription = "Phòng sang trọng với phòng khách riêng", TypeNote = "View đẹp" },
            new RoomType { RoomTypeId = 3, RoomTypeName = "Deluxe room", TypeDescription = "Phòng cao cấp", TypeNote = null }
        };

        /// <summary>
        /// Lấy tất cả các loại phòng.
        /// </summary>
        public static IEnumerable<RoomType> GetAll() => _roomTypes.ToList();

        /// <summary>
        /// Lấy loại phòng theo RoomTypeId.
        /// </summary>
        public static RoomType? GetById(int id) => _roomTypes.FirstOrDefault(rt => rt.RoomTypeId == id);

        // LƯU Ý: Không cần phương thức Add/Update/Delete vì RoomType thường là dữ liệu tĩnh.
    }
}
