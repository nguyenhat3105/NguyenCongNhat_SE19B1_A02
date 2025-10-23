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
            // 🔥 BỎ HẲN KHAI BÁO FIELD CẤP CLASS (private readonly IBookingReservationRepository iBookingRepo;
            // và private readonly FuminiHotelManagementContext _context;)

            public BookingReservationService()
            {
                // Constructor KHÔNG NÊN làm gì cả, không nên new Context ở đây.
            }

            // --- Sửa các hàm CRUD/Query để dùng Context riêng ---

            public void CancelReservation(int reservationId)
            {
                // Tạo Context và Repo mới, đảm bảo Context được Dispose
                using (var context = new FuminiHotelManagementContext())
                {
                    var repo = new BookingReservationRepository(context);
                    repo.CancelReservation(reservationId);
                }
            }

            public BookingReservation CreateReservation(int customerId, DateOnly bookingDate, IEnumerable<(int roomId, DateOnly start, DateOnly end)> rooms)
            {
                // 🔥 TẠO CONTEXT MỚI VÀ REPOSITORY MỚI CHO MỖI GIAO DỊCH
                using (var context = new FuminiHotelManagementContext())
                {
                    var repo = new BookingReservationRepository(context);
                    return repo.CreateReservation(customerId, bookingDate, rooms);
                }
            }

            public IEnumerable<RoomInformation> GetAvailableRooms(DateOnly start, DateOnly end)
            {
                // LƯU Ý: Chuyển giao hoàn toàn cho Repository, vì logic trong Repository đã được sửa và tối ưu hơn.
                using (var context = new FuminiHotelManagementContext())
                {
                    var repo = new BookingReservationRepository(context);
                    return repo.GetAvailableRooms(start, end);
                }
            }

            public IEnumerable<BookingReservation> GetReservationsBetween(DateOnly start, DateOnly end)
            {
                // Chuyển giao hoàn toàn cho Repository
                using (var context = new FuminiHotelManagementContext())
                {
                    var repo = new BookingReservationRepository(context);
                    return repo.GetReservationsBetween(start, end);
                }
            }

        public IEnumerable<BookingReservation> GetReservationsByCustomer(int customerId, DateOnly? start = null, DateOnly? end = null)
        {
            var reservations = GetReservationsBetween(start ?? DateOnly.MinValue, end ?? DateOnly.MaxValue);

            return reservations.Where(r => r.CustomerId == customerId);
        }



        public RoomInformation? GetRoomByID(int roomId)
            {
                // Chuyển giao hoàn toàn cho Repository
                using (var context = new FuminiHotelManagementContext())
                {
                    var repo = new BookingReservationRepository(context);
                    return repo.GetRoomByID(roomId);
                }
            }
        }
    }