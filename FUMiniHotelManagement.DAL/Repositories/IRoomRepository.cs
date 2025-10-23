using FUMiniHotelManagement.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FUMiniHotelManagement.DAL.Repositories
{
    public interface IRoomRepository
    {
        IEnumerable<RoomInformation> GetAll();
        RoomInformation? GetById(int roomId);
        void Update(RoomInformation room);
        void Delete(RoomInformation room);
        void create(RoomInformation room);



    }
}
