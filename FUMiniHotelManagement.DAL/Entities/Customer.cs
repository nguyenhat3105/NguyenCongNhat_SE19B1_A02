using System;
using System.Collections.Generic;

namespace FUMiniHotelManagement.DAL.Entities;

public partial class Customer
{
    public int CustomerId { get; set; }

    public string? CustomerFullName { get; set; }

    public string? Telephone { get; set; }

    public string EmailAddress { get; set; } = null!;

    public DateOnly? CustomerBirthday { get; set; }

    public byte? CustomerStatus { get; set; }

    public string? Password { get; set; }

    public int Role { get; set; }

    public virtual ICollection<BookingReservation> BookingReservations { get; set; } = new List<BookingReservation>();


    // Phương thức tạo bản sao nông (Shallow Copy)
    public Customer Clone()
    {
        return new Customer
        {
            CustomerId = this.CustomerId,
            CustomerFullName = this.CustomerFullName,
            Telephone = this.Telephone,
            EmailAddress = this.EmailAddress,
            CustomerBirthday = this.CustomerBirthday,
            CustomerStatus = this.CustomerStatus,
            Password = this.Password, // Rất quan trọng: Copy Password
            Role = this.Role,
            // KHÔNG copy BookingReservations để tránh lặp vô hạn và lỗi
            BookingReservations = new List<BookingReservation>()
        };
    }
}
