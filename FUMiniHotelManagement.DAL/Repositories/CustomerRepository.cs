using FUMiniHotelManagement.DAL.DAO;
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
      

        public CustomerRepository() { }

        public Customer? GetByEmail(string email)
        {
            return CustomerDAO.GetByEmail(email);
        }
        public void Add(Customer customer)
        {
            if (customer == null)
            {
                throw new ArgumentNullException(nameof(customer));
            }
            // ✅ Map sang DAO (DAO tự xử lý gán ID và lưu)
            CustomerDAO.Add(customer);
        }
        public void Delete(int customerId)
        {
            CustomerDAO.Delete(customerId);
        }

        public void Update(Customer customer)
        {
            if (customer == null)
            {
                throw new ArgumentNullException(nameof(customer));
            }

            // Kiểm tra sự tồn tại (chuyển logic kiểm tra sang Repository hoặc sử dụng DAO.GetById)
            var existingCustomer = CustomerDAO.GetById(customer.CustomerId);
            if (existingCustomer == null)
            {
                throw new InvalidOperationException("Không tìm thấy khách hàng để cập nhật");
            }

            // ✅ Map sang DAO (DAO tự xử lý cập nhật các thuộc tính)
            CustomerDAO.Update(customer);
        }
        public Customer? GetById(int customerId)
        {
            return CustomerDAO.GetById(customerId);
        }
        public IEnumerable<Customer> GetAll()
        {
            return CustomerDAO.GetAll();
        }
    }
}




