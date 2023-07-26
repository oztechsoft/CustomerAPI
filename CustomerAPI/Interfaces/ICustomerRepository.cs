using CustomerAPI.Models;
using System.Collections.Generic;

namespace CustomerAPI.Interfaces
{
    public interface ICustomerRepository
    {
        IEnumerable<Customer> GetCustomers();
        Customer GetCustomerById(int id);
        void AddCustomer(Customer customer);
        void UpdateCustomer(Customer customer);
        void DeleteCustomer(int id);
        IEnumerable<Customer> SearchCustomersByName(string partialName);
    }
}
