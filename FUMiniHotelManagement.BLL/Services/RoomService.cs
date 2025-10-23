using FUMiniHotelManagement.DAL.Entities;
using FUMiniHotelManagement.DAL.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FUMiniHotelManagement.BLL.Services
{
    public class RoomService: IRoomService
    {
        private readonly IRoomRepository iRoomRepository;
        public RoomService()
        {
            iRoomRepository = new RoomRepository();
        }
        public IEnumerable<RoomInformation> GetAll()
        {
            return iRoomRepository.GetAll();
        }
        public RoomInformation? GetById(int roomId)
        {
            return iRoomRepository.GetById(roomId);
        }
        public void Update(RoomInformation roomInformation)
        {
            iRoomRepository.Update(roomInformation);
        }

        public void Delete(RoomInformation roomInformation)
        {
            iRoomRepository.Delete(roomInformation);
        }
        public void Create(RoomInformation roomInformation)
        {
            iRoomRepository.create(roomInformation);
        }
    }
}
