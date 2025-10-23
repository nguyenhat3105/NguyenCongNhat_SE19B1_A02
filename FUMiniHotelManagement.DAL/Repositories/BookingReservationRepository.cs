using FUMiniHotelManagement.DAL.DAO;
using FUMiniHotelManagement.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace FUMiniHotelManagement.DAL.Repositories
{
    public class BookingReservationRepository : IBookingReservationRepository
    {
        // ❌ Đã loại bỏ: private readonly FuminiHotelManagementContext _context;

        // Constructor không còn cần thiết cho EF Core
        public BookingReservationRepository()
        {
            // Logic khởi tạo (nếu có) sẽ được đặt ở đây.
        }

        /// <summary>
        /// Lấy danh sách phòng còn trống trong khoảng thời gian.
        /// </summary>
        public IEnumerable<RoomInformation> GetAvailableRooms(DateOnly start, DateOnly end)
        {
            if (end <= start)
            {
                return Enumerable.Empty<RoomInformation>();
            }

            // 1. Lấy tất cả chi tiết đặt phòng đang hoạt động (BookingStatus = 1)
            var activeBookingDetails = BookingDAO.GetActiveBookingDetails();

            // 2. Lọc ra ID của các phòng đã bị đặt trong khoảng thời gian yêu cầu
            var bookedRoomIds = activeBookingDetails
                .Where(d => d.StartDate < end && d.EndDate > start) // Logic chồng chéo thời gian
                .Select(d => d.RoomId)
                .Distinct()
                .ToList();

            // 3. Lấy tất cả phòng và loại trừ các phòng đã bị đặt.
            // RoomDAO.GetAll() đã tự động "Include" RoomType.
            var allRooms = RoomDAO.GetAll();

            var availableRooms = allRooms
                .Where(r => !bookedRoomIds.Contains(r.RoomId))
                .ToList();

            return availableRooms;
        }

        /// <summary>
        /// Tạo reservation kèm details.
        /// </summary>
        public BookingReservation CreateReservation(int customerId, DateOnly bookingDate, IEnumerable<(int roomId, DateOnly start, DateOnly end)> rooms)
        {
            if (rooms == null) throw new ArgumentNullException(nameof(rooms));

            // 1. Validate customer exists (sử dụng DAO)
            var customer = CustomerDAO.GetById(customerId);
            if (customer == null)
                throw new InvalidOperationException("Customer not found.");

            var roomRequests = rooms.ToList();
            if (!roomRequests.Any())
                throw new ArgumentException("Rooms cannot be null or empty.", nameof(rooms));

            var requestedRoomIds = roomRequests.Select(r => r.roomId).Distinct().ToArray();
            var minStart = roomRequests.Min(r => r.start);
            var maxEnd = roomRequests.Max(r => r.end);

            foreach (var r in roomRequests)
            {
                if (r.end <= r.start) // Đã sửa lỗi: End date phải lớn hơn Start date
                    throw new ArgumentException($"End date {r.end} must be after start date {r.start} for room {r.roomId}.");
            }

            // --- CHECK CONFLICTS ---
            // Lấy tất cả chi tiết đặt phòng đang hoạt động (BookingDAO xử lý lọc theo BookingStatus)
            var potentialConflicts = BookingDAO.GetActiveBookingDetails()
                .Where(d =>
                    requestedRoomIds.Contains(d.RoomId)
                    && d.StartDate < maxEnd
                    && d.EndDate > minStart
                )
                .ToList();

            // Check per requested room/date range
            foreach (var req in roomRequests)
            {
                var overlaps = potentialConflicts.Any(d =>
                    d.RoomId == req.roomId &&
                    d.StartDate < req.end &&
                    d.EndDate > req.start
                );

                if (overlaps)
                {
                    throw new InvalidOperationException($"Room {req.roomId} is not available for {req.start} - {req.end}.");
                }
            }

            // --- BUILD RESERVATION + DETAILS ---
            var reservation = new BookingReservation
            {
                BookingDate = bookingDate,
                CustomerId = customerId,
                BookingStatus = 1,
                BookingDetails = new List<BookingDetail>()
            };

            decimal total = 0m;

            // Load room info for price calculation (sử dụng RoomDAO)
            var roomsInfo = requestedRoomIds.ToDictionary(id => id, id => RoomDAO.GetById(id));
            if (roomsInfo.Any(kvp => kvp.Value == null))
                throw new InvalidOperationException($"One or more rooms requested were not found.");

            foreach (var req in roomRequests)
            {
                var roomInfo = roomsInfo[req.roomId]!;

                int nights = (req.end.DayNumber - req.start.DayNumber);
                if (nights <= 0) nights = 1;

                var detail = new BookingDetail
                {
                    RoomId = req.roomId,
                    StartDate = req.start,
                    EndDate = req.end,
                    ActualPrice = roomInfo.RoomPricePerDay
                };

                reservation.BookingDetails.Add(detail);
                total += roomInfo.RoomPricePerDay * nights;
            }

            reservation.TotalPrice = total;

            // Persist (Sử dụng BookingDAO.Add)
            try
            {
                BookingDAO.Add(reservation);
            }
            catch (Exception)
            {
                throw;
            }

            return reservation;
        }


        /// <summary>
        /// Lấy tất cả đơn đặt phòng của một khách hàng.
        /// </summary>
        public IEnumerable<BookingReservation> GetReservationsByCustomer(int customerId)
        {
            // BookingDAO.GetAll() đã tự động "Include" tất cả các mối quan hệ cần thiết
            return BookingDAO.GetAll()
                .Where(r => r.CustomerId == customerId)
                .ToList();
        }

        /// <summary>
        /// Lấy tất cả đơn đặt phòng đang hoạt động có chi tiết nằm trong khoảng thời gian.
        /// </summary>
        public IEnumerable<BookingReservation> GetReservationsBetween(DateOnly start, DateOnly end)
        {
            if (end < start) throw new ArgumentException("End date must be >= start date.");

            return BookingDAO.GetAll()
                .Where(r => r.BookingStatus == 1
                            && r.BookingDetails.Any(d => d.StartDate < end && d.EndDate > start))
                .ToList();
        }

        /// <summary>
        /// Hủy đơn đặt phòng (Xóa mềm - set status = 0).
        /// </summary>
        public void CancelReservation(int reservationId)
        {
            var r = BookingDAO.GetById(reservationId);
            if (r == null) throw new InvalidOperationException("Reservation not found.");

            // ✅ Map sang DAO
            BookingDAO.Cancel(reservationId);
        }

        /// <summary>
        /// Lấy thông tin phòng theo ID.
        /// </summary>
        public RoomInformation? GetRoomByID(int roomId)
        {
            // ✅ Map sang DAO
            return RoomDAO.GetById(roomId);
        }
    }
}