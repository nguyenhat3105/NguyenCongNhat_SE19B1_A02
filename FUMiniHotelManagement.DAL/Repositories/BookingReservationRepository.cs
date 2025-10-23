using FUMiniHotelManagement.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace FUMiniHotelManagement.DAL.Repositories
{
    public class BookingReservationRepository: IBookingReservationRepository
    {
        private readonly FuminiHotelManagementContext _context;

        public BookingReservationRepository(FuminiHotelManagementContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        // --- Các phương thức khác (GetAvailableRooms, GetReservationsByCustomer, v.v.) không cần sửa ---

        // using Microsoft.EntityFrameworkCore; // Đảm bảo đã có using này ở đầu file

        public IEnumerable<RoomInformation> GetAvailableRooms(DateOnly start, DateOnly end)
        {
            if (end <= start)
            {
                return Enumerable.Empty<RoomInformation>();
            }

            // 🔥 SỬA LỖI: Dùng JOIN tường minh để đảm bảo lọc được theo BookingStatus
            var bookedRoomIds = _context.BookingDetails
                .AsNoTracking()
                .Join( // Join với bảng BookingReservations
                    _context.BookingReservations.AsNoTracking(),
                    detail => detail.BookingReservationId,      // Khóa từ BookingDetail
                    reservation => reservation.BookingReservationId, // Khóa từ BookingReservation
                    (detail, reservation) => new { detail, reservation } // Tạo ra một đối tượng kết quả tạm thời
                )
                // 1. Lọc theo trạng thái của đơn đặt phòng (CHỈ LẤY ĐƠN HOẠT ĐỘNG)
                .Where(joinedResult => joinedResult.reservation.BookingStatus != 0)
                // 2. Lọc theo khoảng thời gian chồng chéo
                .Where(joinedResult => joinedResult.detail.StartDate < end && joinedResult.detail.EndDate > start)
                // 3. Lấy ra ID phòng
                .Select(joinedResult => joinedResult.detail.RoomId)
                .Distinct()
                .ToList();

            // Logic lấy phòng trống không thay đổi: Lấy tất cả phòng trừ đi những phòng đã bị đặt
            var availableRooms = _context.RoomInformations
                .AsNoTracking()
                .Where(r => !bookedRoomIds.Contains(r.RoomId))
                .Include(r => r.RoomType)
                .ToList();

            return availableRooms;
        }

        /// <summary>
        /// Tạo reservation kèm details.
        /// </summary>
        public BookingReservation CreateReservation(int customerId, DateOnly bookingDate, IEnumerable<(int roomId, DateOnly start, DateOnly end)> rooms)
        {
            if (rooms == null) throw new ArgumentNullException(nameof(rooms));

            Debug.WriteLine("[CreateReservation] start");

            // Validate customer exists (read-only check)
            var customer = _context.Customers.AsNoTracking().FirstOrDefault(c => c.CustomerId == customerId);
            if (customer == null)
                throw new InvalidOperationException("Customer not found.");

            var roomRequests = rooms.ToList();
            if (!roomRequests.Any())
                throw new ArgumentException("Rooms cannot be null or empty.", nameof(rooms));

            // Normalize requested ids and date range
            var requestedRoomIds = roomRequests.Select(r => r.roomId).Distinct().ToArray();
            var minStart = roomRequests.Min(r => r.start);
            var maxEnd = roomRequests.Max(r => r.end);

            // Basic date validation (each request)
            foreach (var r in roomRequests)
            {
                if (r.end < r.start)
                    throw new ArgumentException($"End date {r.end} is before start date {r.start} for room {r.roomId}.");
            }

            // --- CHECK CONFLICTS ---
            // Only consider BookingDetails that belong to active reservations (BookingStatus == 1)
            var potentialConflicts = _context.BookingDetails
                .AsNoTracking()
                .Join(
                    _context.BookingReservations.AsNoTracking(),
                    detail => detail.BookingReservationId,
                    reservation => reservation.BookingReservationId,
                    (detail, reservation) => new { detail, reservation }
                )
                .Where(x =>
                    requestedRoomIds.Contains(x.detail.RoomId)
                    && x.reservation.BookingStatus == 1           // <-- only active reservations
                    && x.detail.StartDate < maxEnd
                    && x.detail.EndDate > minStart
                )
                .Select(x => x.detail)
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
                    // Optional: detailed debug output for conflicts
                    Debug.WriteLine($"[CreateReservation] Conflict detected for room {req.roomId} ({req.start} - {req.end}). Conflicting records:");
                    foreach (var d in potentialConflicts.Where(d => d.RoomId == req.roomId))
                    {
                        Debug.WriteLine($"   bookingDetail: resId={d.BookingReservationId}, start={d.StartDate}, end={d.EndDate}");
                    }

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

            // Load room info for price calculation
            var roomsInfo = _context.RoomInformations
                .AsNoTracking()
                .Where(r => requestedRoomIds.Contains(r.RoomId))
                .ToDictionary(r => r.RoomId);

            foreach (var req in roomRequests)
            {
                if (!roomsInfo.TryGetValue(req.roomId, out var roomInfo))
                    throw new InvalidOperationException($"Room with id {req.roomId} not found.");

                // nights calculation: difference in days; if zero or negative, use 1
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

            // Persist (EF Core will insert reservation and details in one SaveChanges)
            try
            {
                _context.BookingReservations.Add(reservation);
                _context.SaveChanges();

                Debug.WriteLine($"[CreateReservation] Saved reservation id={reservation.BookingReservationId}, total={reservation.TotalPrice}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine("[CreateReservation] SaveChanges failed: " + (ex.InnerException?.Message ?? ex.Message));
                throw;
            }

            return reservation;
        }


        // --- Các phương thức khác ---

        public IEnumerable<BookingReservation> GetReservationsByCustomer(int customerId)
        {
            return _context.BookingReservations
                .AsNoTracking()
                .Include(r => r.Customer)
                .Include(r => r.BookingDetails)
                .ThenInclude(bd => bd.Room)
                .Where(r => r.CustomerId == customerId)
                .ToList();
        }

        public IEnumerable<BookingReservation> GetReservationsBetween(DateOnly start, DateOnly end)
        {
            if (end < start) throw new ArgumentException("End date must be >= start date.");

            // Debug
            System.Diagnostics.Debug.WriteLine($"[Repo] GetReservationsBetween: {start} - {end}");

            var result = _context.BookingReservations
                .AsNoTracking()
                .Include(r => r.Customer)
                .Include(r => r.BookingDetails)
                    .ThenInclude(bd => bd.Room)
                .Where(r => r.BookingStatus == 1
                            && r.BookingDetails.Any(d => d.StartDate < end && d.EndDate > start))
                .ToList();

            System.Diagnostics.Debug.WriteLine("[Repo] returned reservations:");
            foreach (var rr in result)
            {
                System.Diagnostics.Debug.WriteLine($"  id={rr.BookingReservationId}, status={rr.BookingStatus}");
            }

            return result;
        }




        public void CancelReservation(int reservationId)
        {
            var r = _context.BookingReservations.Find(reservationId);
            if (r == null) throw new InvalidOperationException("Reservation not found.");
            r.BookingStatus = 0;
            _context.SaveChanges();

            _context.Entry(r).State = EntityState.Detached; // ✅ giải phóng khỏi tracking
        }


        public RoomInformation? GetRoomByID(int roomId)
        {
            return _context.RoomInformations
                .AsNoTracking()
                .FirstOrDefault(r => r.RoomId == roomId);
        }
    }
}