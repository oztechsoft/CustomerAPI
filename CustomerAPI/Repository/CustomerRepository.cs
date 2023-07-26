using CustomerAPI.Data;
using CustomerAPI.Interfaces;
using CustomerAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CustomerAPI.Repository
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly CustomerDbContext _context;

        public CustomerRepository(CustomerDbContext context)
        {
            _context = context;
        }

        public IEnumerable<Customer> GetCustomers()
        {
            return _context.Customers.ToList();
        }

        public Customer GetCustomerById(int id)
        {
            return _context.Customers.FirstOrDefault(c => c.Id == id);
        }

        public void AddCustomer(Customer customer)
        {
            _context.Customers.Add(customer);
            _context.SaveChanges();
        }

        public void UpdateCustomer(Customer customer)
        {
            var existingCustomer = _context.Customers.Find(customer.Id);
            if (existingCustomer == null) return;

            existingCustomer.FirstName = customer.FirstName;
            existingCustomer.LastName = customer.LastName;
            existingCustomer.DateOfBirth = customer.DateOfBirth;

            _context.SaveChanges();
        }

        public void DeleteCustomer(int id)
        {
            var customerToDelete = _context.Customers.Find(id);
            if (customerToDelete == null) return;

            _context.Customers.Remove(customerToDelete);
            _context.SaveChanges();
        }

        public IEnumerable<Customer> SearchCustomersByName(string partialName)
        {
            return _context.Customers
    .Where(c => c.FirstName.IndexOf(partialName, StringComparison.OrdinalIgnoreCase) >= 0
             || c.LastName.IndexOf(partialName, StringComparison.OrdinalIgnoreCase) >= 0)
    .ToList();
        }
    }
}
