using FUMiniHotelManagement.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FUMiniHotelManagement.DAL.DAO
{
    public static class CustomerDAO
    {
        // Dữ liệu giả lập tĩnh
        private static List<Customer> _customers = new List<Customer>
        {
            new Customer
            {
                CustomerId = 1,
                CustomerFullName = "Administrator",
                EmailAddress = "admin@FUMiniHotelSystem.com",
                Password = "@@abc123@@",
                CustomerStatus = 1,
                CustomerBirthday = null,
                Telephone = "0335465330",
            },
            new Customer { CustomerId = 3, CustomerFullName = "William Shakespeare", EmailAddress = "william@hotel.org", Password = "123@", CustomerStatus = 1, CustomerBirthday = DateOnly.FromDateTime(new DateTime(1990, 2, 2)), Telephone = "0903939393" },
            new Customer { CustomerId = 5, CustomerFullName = "Elizabeth Taylor", EmailAddress = "elizabeth@hotel.org", Password = "144@", CustomerStatus = 1, CustomerBirthday = DateOnly.FromDateTime(new DateTime(1991, 3, 3)), Telephone = "0903939377" },
            new Customer { CustomerId = 8, CustomerFullName = "James Cameron", EmailAddress = "james@hotel.org", Password = "443@", CustomerStatus = 1, CustomerBirthday = DateOnly.FromDateTime(new DateTime(1992, 11, 10)), Telephone = "0903946582" },
        };

        private static int _nextCustomerId = 12;

        public static IEnumerable<Customer> GetAll() => _customers.Where(c => c.CustomerStatus == 1).ToList();
        public static Customer? GetById(int id) => _customers.FirstOrDefault(c => c.CustomerId == id);
        public static Customer? GetByEmail(string email) => _customers.FirstOrDefault(c => c.EmailAddress.Equals(email, StringComparison.OrdinalIgnoreCase));

        public static void Add(Customer customer)
        {
            customer.CustomerId = _nextCustomerId++;
            _customers.Add(customer);
        }

        public static void Update(Customer customer)
        {
            var existing = _customers.FirstOrDefault(c => c.CustomerId == customer.CustomerId);
            if (existing != null)
            {
                // Cập nhật thủ công (Map)
                existing.CustomerFullName = customer.CustomerFullName;
                existing.Telephone = customer.Telephone;
                existing.EmailAddress = customer.EmailAddress;
                existing.CustomerBirthday = customer.CustomerBirthday;
                existing.CustomerStatus = customer.CustomerStatus;
                existing.Password = customer.Password;
            }
        }

        public static void Delete(int customerId)
        {
            var customer = _customers.FirstOrDefault(c => c.CustomerId == customerId);
            if (customer != null)
            {
                customer.CustomerStatus = 0; // Xóa mềm
            }
        }
    }
}
