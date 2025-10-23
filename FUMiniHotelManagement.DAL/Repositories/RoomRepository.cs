using FUMiniHotelManagement.DAL.DAO;
using FUMiniHotelManagement.DAL.Entities;
using Microsoft.EntityFrameworkCore;
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
            return RoomDAO.GetAll();
        }

        public RoomInformation? GetById(int roomId)
        {
            return RoomDAO.GetById(roomId);
        }

        public void Update(RoomInformation room)
        {
            if (room == null)
            {
                throw new ArgumentNullException(nameof(room));
            }

            // Logic kiểm tra tồn tại (sử dụng DAO thay vì Context)
            var existingRoom = RoomDAO.GetById(room.RoomId);
            if (existingRoom == null)
            {
                throw new InvalidOperationException("Không tìm thấy phòng để cập nhật");
            }

            // ✅ Map sang DAO (DAO tự xử lý cập nhật các thuộc tính)
            RoomDAO.Update(room);
        }

        public void Delete(RoomInformation roomInformation)
        {
            RoomDAO.Delete(roomInformation.RoomId);
        }
        public void create(RoomInformation room)
        {
            RoomDAO.Create(room);
        }
    }
}
