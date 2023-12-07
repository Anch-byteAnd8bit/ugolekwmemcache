
using MailKit.Search;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Moq;
using Org.BouncyCastle.Tls;
using Ugolek.Backend.Web.Application.Features.Coals;
using Ugolek.Backend.Web.Application.Features.Customers;
using Ugolek.Backend.Web.Application.Features.Orders;
using Ugolek.Backend.Web.Application.Services;
using Ugolek.Backend.Web.Configuration.Services;
using Ugolek.Backend.Web.Core;

namespace TestOrdersPost
{
    public class UnitTestGetOrder
    {
        [Theory]
        [InlineData("av", "stree", "1")]
        [InlineData("av", "stree", "2")]
        [InlineData("av", "stree", "100")]
        public void TestProvideAddress(string city, string street, string home)
        {
            // Arrange
            Customer customer = new Customer { Id = 1, Email = "qwqw@aqsa.ry", City = city, House = home, Street = street };
            Customer customer2 = new Customer { Id = 1, Email = "qwqw@aqsa.ry" };

            // Act
            customer2.ProvideAddress(city, street, home);

            // Assert
            Assert.Equal(customer.City, customer2.City);
            Assert.Equal(customer.Street, customer2.Street);
            Assert.Equal(customer.House, customer2.House);
        }

        [Fact]
        public void TestPlaceOrder()
        {
            // Arrange
            IRepository<Order> orders = new BaseInMemRepository<Order>(new List<Order> { });

            IRepository<Coal> coals = new BaseInMemRepository<Coal>(new List<Coal> {
                new() { Id = 1, Name = "дляь 0-25", Price = 150000, },
                new() { Id = 2, Name = "дн 25-50", Price = 210000, },
                new() { Id = 3, Name = "Tдой 50-200", Price = 230000 }
                });
            OrderService orderService = new OrderService(orders, coals);
            Customer customer = new Customer { Id = 10, Email = "s@gmail.com", City = "Abakan", House = "2", Street = "Kirova" };
            List<OrderItemDto> orderItems = new List<OrderItemDto> { new OrderItemDto { Id = 1, Weight = 1000 },
                    new OrderItemDto { Id = 2, Weight = 2000 } };
            // Act
            orderService.PlaceOrder(orderItems, customer);

            // Assert
            Assert.True(orders.GetMany().Single(el => el.Customer == customer) != null);

        }
    }
}

