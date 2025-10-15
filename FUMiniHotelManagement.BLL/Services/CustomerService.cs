using FUMiniHotelManagement.DAL.Entities;
using FUMiniHotelManagement.DAL.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FUMiniHotelManagement.BLL.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly ICustomerRepository iCustomerRepository;

        public CustomerService()
        {
            iCustomerRepository = new CustomerRepository();
        }

        public Customer? GetByEmail(string email)
        {
            return iCustomerRepository.GetByEmail(email);
        }

        public void Add(Customer customer)
        {
            iCustomerRepository.Add(customer);
        }

        public void Delete(int customerId)
        {
            iCustomerRepository.Delete(customerId);
        }

        public void Update(Customer customer)
        {
            iCustomerRepository.Update(customer);
        }

        public Customer? GetById(int customerId)
        {
            return iCustomerRepository.GetById(customerId);
        }

        public IEnumerable<Customer> GetAll()
        {
            return iCustomerRepository.GetAll();
        }

        public Customer? Login(string email, string password)
        {
            var customer = iCustomerRepository.GetByEmail(email);
            if (customer != null && customer.Password == password)
            {
                return customer;
            }
            return null;
        }


    }
}
