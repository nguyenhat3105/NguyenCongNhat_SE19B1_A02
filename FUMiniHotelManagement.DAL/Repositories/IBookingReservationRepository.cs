using FUMiniHotelManagement.DAL.Entities;
using System;
using System.Collections.Generic;

namespace FUMiniHotelManagement.DAL.Repositories
{
    public interface IBookingReservationRepository
    {
        IEnumerable<RoomInformation> GetAvailableRooms(DateOnly start, DateOnly end);

        BookingReservation CreateReservation(int customerId, DateOnly bookingDate, IEnumerable<(int roomId, DateOnly start, DateOnly end)> rooms);

        IEnumerable<BookingReservation> GetReservationsByCustomer(int customerId);

        IEnumerable<BookingReservation> GetReservationsBetween(DateOnly start, DateOnly end);

        void CancelReservation(int reservationId);

        RoomInformation? GetRoomByID(int roomId);
    }
}
