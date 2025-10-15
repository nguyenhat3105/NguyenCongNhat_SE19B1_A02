using FUMiniHotelManagement.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FUMiniHotelManagement.DAL.Repositories
{
    public class BookingReservationRepository: IBookingReservationRepository
    {
        private readonly IBookingReservationRepository _repo;
        private readonly ICustomerRepository _customerRepo;
        private readonly IRoomRepository _roomRepo; // optional if you have one
        private BookingReservationRepository bookingReservationRepository;
        private CustomerRepository customerRepository;

        public BookingReservationRepository() : this(new BookingReservationRepository(), new CustomerRepository()) { }

        public BookingReservationRepository(IBookingReservationRepository repo, ICustomerRepository customerRepo)
        {
            _repo = repo ?? throw new ArgumentNullException(nameof(repo));
            _customerRepo = customerRepo ?? throw new ArgumentNullException(nameof(customerRepo));
        }

        public BookingReservationRepository(BookingReservationRepository bookingReservationRepository, CustomerRepository customerRepository)
        {
            this.bookingReservationRepository = bookingReservationRepository;
            this.customerRepository = customerRepository;
        }

        public IEnumerable<RoomInformation> GetAvailableRooms(DateOnly start, DateOnly end)
        {
            return _repo.GetAvailableRooms(start, end);
        }

        // Create reservation with details; compute TotalPrice from room price * nights
        public BookingReservationRepository CreateReservation(int customerId, DateOnly bookingDate, IEnumerable<(int roomId, DateOnly start, DateOnly end)> rooms)
        {
            var customer = _customerRepo.GetById(customerId);
            if (customer == null) throw new InvalidOperationException("Customer not found.");

            var reservation = new BookingReservation
            {
                BookingDate = bookingDate,
                CustomerId = customerId,
                BookingStatus = 1
            };

            // Add details
            decimal total = 0m;
            foreach (var r in rooms)
            {
                // get room info from DB context via repo (we used BookingRepository so access context)
                var room = _repo is BookingReservationRepository br
                    ? br.GetRoomByID(r.roomId) // we'll add this method below OR use context directly
                    : null;

                // fallback: try to get via customerRepo's context or throw if not available
                if (room == null)
                {
                    // naive: create detail without price
                    var detail = new BookingDetail
                    {
                        RoomId = r.roomId,
                        StartDate = r.start,
                        EndDate = r.end,
                        ActualPrice = 0m
                    };
                    reservation.BookingDetails.Add(detail);
                    continue;
                }

                // calculate nights = (end - start) in days (inclusive or exclusive? here treat as nights = (end - start).Days)
                int nights = (r.end.ToDateTime(TimeOnly.MinValue) - r.start.ToDateTime(TimeOnly.MinValue)).Days;
                if (nights <= 0) nights = 1;

                var detail2 = new BookingDetail
                {
                    RoomId = r.roomId,
                    StartDate = r.start,
                    EndDate = r.end,
                    ActualPrice = room.RoomPricePerDay
                };
                reservation.BookingDetails.Add(detail2);
                total += room.RoomPricePerDay.GetValueOrDefault() * nights;
            }

            reservation.TotalPrice = total;
            _repo.AddBooking(reservation);

            // if repository implementation didn't auto-add details, add them now
            foreach (var d in reservation.BookingDetails)
            {
                d.BookingReservationId = reservation.BookingReservationId;
                _repo.AddBookingDetail(d);
            }

            return reservation;
        }

        public IEnumerable<BookingReservationRepository> GetReservationsByCustomer(int customerId)
        {
            return _repo.GetByCustomerId(customerId);
        }

        public IEnumerable<BookingReservationRepository> GetReservationsBetween(DateOnly start, DateOnly end)
        {
            return _repo.GetReservationsBetween(start, end);
        }

        public void CancelReservation(int reservationId)
        {
            var r = _repo.GetById(reservationId);
            if (r == null) throw new InvalidOperationException("Reservation not found.");
            r.BookingStatus = 0; // canceled
            _repo.SaveChanges();
        }

        public RoomInformation? GetRoomByID(int roomId)
        {
            return _roomRepo?.GetById(roomId);
        }
    }
}
