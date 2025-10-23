using FUMiniHotelManagement.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FUMiniHotelManagement.BLL.Services
{
    public interface IBookingReservationService
    {
        IEnumerable<RoomInformation> GetAvailableRooms(DateOnly start, DateOnly end);

        BookingReservation CreateReservation(int customerId, DateOnly bookingDate, IEnumerable<(int roomId, DateOnly start, DateOnly end)> rooms);

        IEnumerable<BookingReservation> GetReservationsByCustomer(int customerId, DateOnly? start = null, DateOnly? end = null);

        IEnumerable<BookingReservation> GetReservationsBetween(DateOnly start, DateOnly end);

        void CancelReservation(int reservationId);

        RoomInformation? GetRoomByID(int roomId);
    }
}
