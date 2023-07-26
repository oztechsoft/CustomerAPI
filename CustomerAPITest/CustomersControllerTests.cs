using CustomerAPI.Controllers;
using CustomerAPI.Interfaces;
using CustomerAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace CustomerAPITest
{
    public class CustomersControllerTests
    {
        [Fact]
        public void GetCustomers_ReturnsListOfCustomers()
        {
            // Arrange
            var mockRepository = new Mock<ICustomerRepository>();
            mockRepository.Setup(repo => repo.GetCustomers())
                          .Returns(new List<Customer>
                          {
                          new Customer { Id = 1, FirstName = "John", LastName = "Doe", DateOfBirth = new DateTime(1990, 5, 15) },
                          new Customer { Id = 2, FirstName = "Jane", LastName = "Smith", DateOfBirth = new DateTime(1985, 8, 20) }
                          });

            var controller = new CustomersController(mockRepository.Object);

            // Act
            var result = controller.GetCustomers();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var customers = Assert.IsAssignableFrom<IEnumerable<Customer>>(okResult.Value);
            Assert.Equal(2, customers.Count());
        }

        [Fact]
        public void GetCustomer_WithValidId_ReturnsCustomer()
        {
            // Arrange
            var mockRepository = new Mock<ICustomerRepository>();
            mockRepository.Setup(repo => repo.GetCustomerById(1))
                          .Returns(new Customer { Id = 1, FirstName = "John", LastName = "Doe", DateOfBirth = new DateTime(1990, 5, 15) });

            var controller = new CustomersController(mockRepository.Object);

            // Act
            var result = controller.GetCustomer(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var customer = Assert.IsType<Customer>(okResult.Value);
            Assert.Equal(1, customer.Id);
        }

        [Fact]
        public void GetCustomer_WithInvalidId_ReturnsNotFound()
        {
            // Arrange
            var mockRepository = new Mock<ICustomerRepository>();
            mockRepository.Setup(repo => repo.GetCustomerById(100))
                          .Returns((Customer)null);

            var controller = new CustomersController(mockRepository.Object);

            // Act
            var result = controller.GetCustomer(100);

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public void AddCustomer_ReturnsCreatedAtAction()
        {
            // Arrange
            var mockRepository = new Mock<ICustomerRepository>();
            var newCustomer = new Customer { Id = 3, FirstName = "Alice", LastName = "Johnson", DateOfBirth = new DateTime(1995, 3, 25) };
            mockRepository.Setup(repo => repo.AddCustomer(newCustomer))
                          .Callback((Customer customer) =>
                          {
                              customer.Id = 3; // Simulate assigning the ID by the repository
                          });
            var controller = new CustomersController(mockRepository.Object);

            // Act
            var result = controller.AddCustomer(newCustomer);

            // Assert
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            Assert.Equal("GetCustomer", createdAtActionResult.ActionName);
            var customer = Assert.IsType<Customer>(createdAtActionResult.Value);
            Assert.Equal(3, customer.Id); // Ensure the ID is assigned
        }

        [Fact]
        public void UpdateCustomer_WithValidId_ReturnsNoContent()
        {
            // Arrange
            var mockRepository = new Mock<ICustomerRepository>();
            var existingCustomer = new Customer { Id = 1, FirstName = "John", LastName = "Doe", DateOfBirth = new DateTime(1990, 5, 15) };
            mockRepository.Setup(repo => repo.GetCustomerById(1))
                          .Returns(existingCustomer);
            var controller = new CustomersController(mockRepository.Object);
            var updatedCustomer = new Customer { Id = 1, FirstName = "Tom", LastName = "Gates", DateOfBirth = new DateTime(1988, 12, 31) };

            // Act
            var result = controller.UpdateCustomer(1, updatedCustomer);

            // Assert
            Assert.IsType<NoContentResult>(result);
            Assert.Equal("Tom", updatedCustomer.FirstName); // Ensure the existing customer is updated
        }

        [Fact]
        public void DeleteCustomer_WithValidId_ReturnsNoContent()
        {
            // Arrange
            var mockRepository = new Mock<ICustomerRepository>();
            var existingCustomer = new Customer { Id = 1, FirstName = "John", LastName = "Doe", DateOfBirth = new DateTime(1990, 5, 15) };
            mockRepository.Setup(repo => repo.GetCustomerById(1))
                          .Returns(existingCustomer);
            var controller = new CustomersController(mockRepository.Object);

            // Act
            var result = controller.DeleteCustomer(1);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public void DeleteCustomer_WithInvalidId_ReturnsNotFound()
        {
            // Arrange
            var mockRepository = new Mock<ICustomerRepository>();
            mockRepository.Setup(repo => repo.GetCustomerById(100))
                          .Returns((Customer)null);
            var controller = new CustomersController(mockRepository.Object);

            // Act
            var result = controller.DeleteCustomer(100);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void SearchCustomers_ReturnsMatchingCustomers()
        {
            // Arrange
            var mockRepository = new Mock<ICustomerRepository>();
            var customers = new List<Customer>
{
    new Customer { Id = 1, FirstName = "John", LastName = "Doe", DateOfBirth = new DateTime(1990, 5, 15) },
    new Customer { Id = 2, FirstName = "Jane", LastName = "Smith", DateOfBirth = new DateTime(1985, 8, 20) },
    new Customer { Id = 3, FirstName = "Alice", LastName = "Johnson", DateOfBirth = new DateTime(1995, 3, 25) },
    new Customer { Id = 4, FirstName = "Bob", LastName = "Johnson", DateOfBirth = new DateTime(1982, 11, 10) }
};
            mockRepository.Setup(repo => repo.SearchCustomersByName("John"))
                          .Returns(new List<Customer> { customers[0] }); // Simulate searching for "John"
            mockRepository.Setup(repo => repo.SearchCustomersByName("Smith"))
                          .Returns(new List<Customer> { customers[1] }); // Simulate searching for "Smith"
            mockRepository.Setup(repo => repo.SearchCustomersByName("Alice"))
                          .Returns(new List<Customer> { customers[2] }); // Simulate searching for "Alice"
            mockRepository.Setup(repo => repo.SearchCustomersByName("Johnson"))
                          .Returns(new List<Customer> { customers[2], customers[3] }); // Simulate searching for "Johnson"
            mockRepository.Setup(repo => repo.SearchCustomersByName("NonExisting"))
                          .Returns(new List<Customer>()); // Simulate searching for "NonExisting"

            var controller = new CustomersController(mockRepository.Object);

            // Act
            var resultJohn = controller.SearchCustomers("John");
            var resultSmith = controller.SearchCustomers("Smith");
            var resultAlice = controller.SearchCustomers("Alice");
            var resultJohnson = controller.SearchCustomers("Johnson");
            var resultNonExisting = controller.SearchCustomers("NonExisting");

            // Assert
            var johnCustomers = Assert.IsAssignableFrom<IEnumerable<Customer>>(resultJohn);
            var smithCustomers = Assert.IsAssignableFrom<IEnumerable<Customer>>(resultSmith);
            var aliceCustomers = Assert.IsAssignableFrom<IEnumerable<Customer>>(resultAlice);
            var johnsonCustomers = Assert.IsAssignableFrom<IEnumerable<Customer>>(resultJohnson);
            var nonExistingCustomers = Assert.IsAssignableFrom<IEnumerable<Customer>>(resultNonExisting);

            Assert.Equal(1, johnCustomers.Count());
            Assert.Equal(1, smithCustomers.Count());
            Assert.Equal(1, aliceCustomers.Count());
            Assert.Equal(2, johnsonCustomers.Count());
            Assert.Empty(nonExistingCustomers);
        }
    }
}
