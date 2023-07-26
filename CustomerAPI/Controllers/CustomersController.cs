using CustomerAPI.Interfaces;
using CustomerAPI.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace CustomerAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomersController : ControllerBase
    {
        private readonly ICustomerRepository _customerRepository;

        public CustomersController(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }

        [HttpGet]
        public ActionResult<IEnumerable<Customer>> GetCustomers()
        {
            var customers =  _customerRepository.GetCustomers();
            return Ok(customers);
        }

        [HttpGet("{id}")]
        public ActionResult<Customer> GetCustomer(int id)
        {
            var customer = _customerRepository.GetCustomerById(id);
            if (customer == null)
                return NotFound();

            return Ok(customer);
        }

        [HttpPost]
        public ActionResult<Customer> AddCustomer(Customer customer)
        {
            if (customer == null)
                return BadRequest();

            var existingCustomer = _customerRepository.GetCustomerById(customer.Id);
            if (existingCustomer != null)
            {
                return BadRequest(new { message = "A customer with the same Id already exists." });
            }

            _customerRepository.AddCustomer(customer);
            return CreatedAtAction(nameof(GetCustomer), new { id = customer.Id }, customer);
        }

        [HttpPatch("{id}")]
        public IActionResult UpdateCustomer(int id, Customer customer)
        {
            if (id != customer.Id)
                return BadRequest();

            _customerRepository.UpdateCustomer(customer);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteCustomer(int id)
        {
            var customer = _customerRepository.GetCustomerById(id);
            if (customer == null)
                return NotFound();

            _customerRepository.DeleteCustomer(id);
            return NoContent();
        }

        [HttpGet("search")]
        public IEnumerable<Customer> SearchCustomers(string name)
        {
            return _customerRepository.SearchCustomersByName(name);
        }
    }
}
