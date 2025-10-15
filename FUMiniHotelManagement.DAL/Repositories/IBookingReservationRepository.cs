using FUMiniHotelManagement.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FUMiniHotelManagement.DAL.Repositories
{
    public interface IBookingReservationRepository
    {
        IEnumerable<RoomInformation> GetAvailableRooms(DateOnly start, DateOnly end);
        BookingReservationRepository CreateReservation(int customerId, DateOnly bookingDate, IEnumerable<(int roomId, DateOnly start, DateOnly end)> rooms);
        IEnumerable<BookingReservationRepository> GetReservationsByCustomer(int customerId);
        IEnumerable<BookingReservationRepository> GetReservationsBetween(DateOnly start, DateOnly end);
        void CancelReservation(int reservationId);

        RoomInformation? GetRoomByID(int roomId); // Optional, if needed
    }
}
