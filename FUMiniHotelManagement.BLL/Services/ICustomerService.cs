using FUMiniHotelManagement.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FUMiniHotelManagement.BLL.Services
{
    public interface ICustomerService
    {
        Customer? GetByEmail(string email);
        void Add(Customer customer);
        void Delete(int customerId);
        void Update(Customer customer);
        Customer? GetById(int customerId);

        Customer? Login(string email, string password);
        IEnumerable<Customer> GetAll();
    }
}
