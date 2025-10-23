// Path: /BLL/Services/AuthenticationService.cs
using FUMiniHotelManagement.DAL.DAO;
using FUMiniHotelManagement.DAL.Entities;
using FUMiniHotelManagement.DAL.Repositories;
using FUMiniHotelManagement.Helper;
using System;

namespace FUMiniHotelManagement.BLL.Services
{
    public class AuthenticationService
    {
        private readonly CustomerRepository _customerRepo; // hoặc ICustomerRepository via DI

        public AuthenticationService(CustomerRepository customerRepo)
        {
            _customerRepo = customerRepo ?? throw new ArgumentNullException(nameof(customerRepo));
        }

        // VERY SIMPLE login: tìm theo email + so sánh password thô (bạn nên hash+salt ở production)
        public Customer? Login(string email, string password)
        {
            var originalCustomer = CustomerDAO.GetByEmail(email);

            if (originalCustomer == null || originalCustomer.CustomerStatus == 0)
            {
                return null;
            }

            // 2. So sánh mật khẩu với đối tượng GỐC
            if (originalCustomer.Password != password)
            {
                return null;
            }

            // 3. ✅ TRẢ VỀ BẢN SAO (CLONE)
            return originalCustomer.Clone();
        }


        public void Logout()
        {
            // Xoá session và các data nhạy cảm
            SessionManager.Clear();

            // Nếu bạn lưu "remember me" tokens, xóa ở đây.
        }
    }
}
