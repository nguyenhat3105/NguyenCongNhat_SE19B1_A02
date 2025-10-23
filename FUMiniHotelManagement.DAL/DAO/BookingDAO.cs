using FUMiniHotelManagement.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace FUMiniHotelManagement.DAL.DAO
{
    public static class BookingDAO
    {
        private static List<BookingReservation> _reservations = new List<BookingReservation>
        {
            new BookingReservation
            {
                BookingReservationId = 1,
                BookingDate = DateOnly.FromDateTime(new DateTime(2024, 10, 1)),
                CustomerId = 3, TotalPrice = 300, BookingStatus = 1,
                BookingDetails = new List<BookingDetail>
                {
                    // Đơn đặt phòng mẫu đang hoạt động
                    new BookingDetail { BookingReservationId = 1, RoomId = 1, StartDate = DateOnly.FromDateTime(new DateTime(2024, 10, 1)), EndDate = DateOnly.FromDateTime(new DateTime(2024, 10, 3)), ActualPrice = 150.00m },
                    new BookingDetail { BookingReservationId = 1, RoomId = 3, StartDate = DateOnly.FromDateTime(new DateTime(2024, 10, 1)), EndDate = DateOnly.FromDateTime(new DateTime(2024, 10, 5)), ActualPrice = 150.00m },
                }
            }
        };

        private static int _nextReservationId = 2; // Bắt đầu từ 2

        public static IEnumerable<BookingReservation> GetAll()
        {
            return _reservations
            .Select(r => MapNavigations(r)) // Áp dụng hàm ánh xạ
            .ToList();
        }

        public static BookingReservation? GetById(int id)
        {
            var reservation = _reservations.FirstOrDefault(r => r.BookingReservationId == id);

            return reservation != null ? MapNavigations(reservation) : null; // Áp dụng hàm ánh xạ
        }

        private static BookingReservation MapNavigations(BookingReservation reservation)
        {
            if (reservation == null) return null;

            // 1. Ánh xạ Khách hàng (Customer)
            if (reservation.CustomerId > 0)
            {
                // Gọi CustomerDAO để tìm và gán đối tượng Customer
                reservation.Customer = CustomerDAO.GetById(reservation.CustomerId);
            }

            // 2. Ánh xạ Phòng (Room) cho từng chi tiết đặt phòng (BookingDetail)
            if (reservation.BookingDetails != null)
            {
                foreach (var detail in reservation.BookingDetails)
                {
                    // Gọi RoomDAO để tìm và gán đối tượng RoomInformation
                    detail.Room = RoomDAO.GetById(detail.RoomId);
                }
            }

            return reservation;
        }

        // Phương thức quan trọng cho việc kiểm tra phòng trống
        public static IEnumerable<BookingDetail> GetActiveBookingDetails()
        {
            return _reservations
                .Where(r => r.BookingStatus == 1) // CHỈ LẤY ĐƠN ĐANG HOẠT ĐỘNG
                .SelectMany(r => r.BookingDetails)
                .ToList();
        }

        public static void Add(BookingReservation reservation)
        {
            reservation.BookingReservationId = _nextReservationId++; // Giả lập ID tự tăng

            // Đảm bảo chi tiết đặt phòng có ID của đơn đặt phòng
            foreach (var detail in reservation.BookingDetails)
            {
                detail.BookingReservationId = reservation.BookingReservationId;
            }

            _reservations.Add(reservation);
        }

        public static void Update(BookingReservation reservation)
        {
            var existing = GetById(reservation.BookingReservationId);
            if (existing != null)
            {
                // Chỉ cho phép cập nhật các trường nhất định
                existing.BookingDate = reservation.BookingDate;
                existing.TotalPrice = reservation.TotalPrice;
                existing.BookingStatus = reservation.BookingStatus;

                // Giả lập cập nhật chi tiết: xóa cũ, thêm mới
                existing.BookingDetails.Clear();
                foreach (var detail in reservation.BookingDetails)
                {
                    detail.BookingReservationId = existing.BookingReservationId;
                    existing.BookingDetails.Add(detail);
                }
            }
        }

        public static void Cancel(int id)
        {
            var r = GetById(id);
            if (r != null) r.BookingStatus = 0; // Xóa mềm
        }

        public static void Delete(int id)
        {
            var r = GetById(id);
            if (r != null) _reservations.Remove(r);
        }
    }
}
