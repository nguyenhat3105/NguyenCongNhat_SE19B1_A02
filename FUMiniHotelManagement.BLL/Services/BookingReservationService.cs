    using FUMiniHotelManagement.DAL;
    using FUMiniHotelManagement.DAL.Entities;
    using FUMiniHotelManagement.DAL.Repositories;
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    namespace FUMiniHotelManagement.BLL.Services
    {
        
        public class BookingReservationService : IBookingReservationService
        {

        private readonly BookingReservationRepository repo;
        public BookingReservationService()
        {
           repo = new BookingReservationRepository();
        }

        public void CancelReservation(int reservationId)
        {
            repo.CancelReservation(reservationId);
        }

        public BookingReservation CreateReservation(int customerId, DateOnly bookingDate, IEnumerable<(int roomId, DateOnly start, DateOnly end)> rooms)
        {
            return repo.CreateReservation(customerId, bookingDate, rooms);

        }

        public IEnumerable<RoomInformation> GetAvailableRooms(DateOnly start, DateOnly end)
        {
             return repo.GetAvailableRooms(start, end);
        }

        public IEnumerable<BookingReservation> GetReservationsBetween(DateOnly start, DateOnly end)
        {
            return repo.GetReservationsBetween(start, end);        
        }

        public IEnumerable<BookingReservation> GetReservationsByCustomer(int customerId, DateOnly? start = null, DateOnly? end = null)
        {
            var reservations = GetReservationsBetween(start ?? DateOnly.MinValue, end ?? DateOnly.MaxValue);

            return reservations.Where(r => r.CustomerId == customerId);
        }



        public RoomInformation? GetRoomByID(int roomId)
        {
             return repo.GetRoomByID(roomId);

        }
    }
}