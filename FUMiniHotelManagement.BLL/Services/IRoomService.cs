using FUMiniHotelManagement.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FUMiniHotelManagement.BLL.Services
{
    public interface IRoomService
    {
        IEnumerable<RoomInformation> GetAll();
        RoomInformation? GetById(int roomId);
        void Update(RoomInformation roomInformation);
        void Delete(RoomInformation roomInformation);
        void Create(RoomInformation roomInformation);
    }
}
