using FUMiniHotelManagement.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FUMiniHotelManagement.DAL.Repositories
{
    public class CustomerRepository : ICustomerRepository
    {
       
        //public CustomerRepository(FuminiHotelManagementContext context)
        //{
        //    var _context = new FuminiHotelManagementContext();
        //    _context = context ?? throw new ArgumentNullException(nameof(context));

        //    //Gán giá trị của tham số context cho trường _context. Nhưng trước đó, nếu tham số context là null, thì hãy ném ra một ngoại lệ ArgumentNullException để báo hiệu rằng tham số context không được phép là null
        //}

        public CustomerRepository() { }

        public Customer? GetByEmail(string email)
        {
            var _context = new FuminiHotelManagementContext();
            return _context.Customers.FirstOrDefault(c => c.EmailAddress == email);
        }
        public void Add(Customer customer)
        {
            var _context = new FuminiHotelManagementContext();
            if (customer == null)
            {
                throw new ArgumentNullException(nameof(customer));
            }
            _context.Customers.Add(customer);
            _context.SaveChanges();
        }
        public void Delete(int customerId)
        {
            var _context = new FuminiHotelManagementContext();
            var customer = _context.Customers.Find(customerId);
            if (customer != null)
            {
                _context.Customers.Remove(customer);
                _context.SaveChanges();
            }
        }

        public void Update(Customer customer)
        {
            var _context = new FuminiHotelManagementContext();
            if (customer == null)
            {
                throw new ArgumentNullException(nameof(customer));
            }

            var existingCustomer = _context.Customers.FirstOrDefault(c => c.CustomerId == customer.CustomerId);
            if (existingCustomer == null)
            {
                throw new InvalidOperationException("Không tìm thấy khách hàng để cập nhật");
            }

            // Cập nhật từng thuộc tính
            existingCustomer.CustomerFullName = customer.CustomerFullName;
            existingCustomer.Telephone = customer.Telephone;
            existingCustomer.EmailAddress = customer.EmailAddress;
            existingCustomer.CustomerBirthday = customer.CustomerBirthday;
            existingCustomer.CustomerStatus = customer.CustomerStatus;
            existingCustomer.Password = customer.Password;

            _context.SaveChanges();
        }
        public Customer? GetById(int customerId)
        {
            var _context = new FuminiHotelManagementContext();
            return _context.Customers.Find(customerId);
        }
        public IEnumerable<Customer> GetAll()
        {
            var _context = new FuminiHotelManagementContext();
            return _context.Customers.ToList();
        }
    }
}



//public void Update(Customer customer)
//{
//    if (customer == null)
//    {
//        throw new ArgumentNullException(nameof(customer));
//    }

//    var existingCustomer = _context.Customers.FirstOrDefault(c => c.CustomerId == customer.CustomerId);
//    if (existingCustomer == null)
//    {
//        throw new InvalidOperationException("Không tìm thấy khách hàng để cập nhật");
//    }

//    // Cập nhật từng thuộc tính
//    existingCustomer.CustomerFullName = customer.CustomerFullName;
//    existingCustomer.Telephone = customer.Telephone;
//    existingCustomer.EmailAddress = customer.EmailAddress;
//    existingCustomer.CustomerBirthday = customer.CustomerBirthday;
//    existingCustomer.CustomerStatus = customer.CustomerStatus;
//    existingCustomer.Password = customer.Password;

//    _context.SaveChanges();
//}







//public void Delete(int id)
//{
//    var customer = _context.Customers.FirstOrDefault(c => c.CustomerId == id);
//    if (customer == null)
//    {
//        throw new InvalidOperationException("Không tìm thấy khách hàng để xóa");
//    }

//    customer.CustomerStatus = 0;
//    _context.SaveChanges();
//}
