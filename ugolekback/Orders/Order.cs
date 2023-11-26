using Microsoft.Extensions.Caching.Memory;
using Ugolek.Backend.Web.Coals.Model;
using Ugolek.Backend.Web.Core;
using Ugolek.Backend.Web.Customers;

namespace Ugolek.Backend.Web.Orders;

public class Order : IEntity
{
    public long Id { get; set; }
    public DateTime OrderDate { get; set; }
    public decimal OrderPrice { get; set; }
    public required Customer Customer { get; set; }

    public required List<OrderItem> OrderItems { get; set; }
}

public class CustomerOrderReq
{
    public string settlement { get; init; }
    public string street { get; init; }
    public string house { get; init; }
    public List<ItemTemp> OrderItems { get; init; }
}

public class OrderService
{
    private readonly IRepository<Customer> customers;
    private readonly IRepository<OrderItem> orderItems;
    private readonly IRepository<Order> orders;
    private readonly IRepository<Coal> coals;
    private readonly IMemoryCache memoryCache;

    public OrderService(
        IRepository<Customer> customers,
        IRepository<OrderItem> orderItems,
        IRepository<Order> orders,
        IRepository<Coal> coals,
        IMemoryCache memoryCache
    )
    {
        this.customers = customers;
        this.orderItems = orderItems;
        this.orders = orders;
        this.coals = coals;
        this.memoryCache = memoryCache;
    }

    public void GetAddress(long customerId, string settlement, string street, string house)
    {
        if (customers.GetCustomerById(customerId) is { } customer)
        {
            customer.City = settlement;
            customer.Street = street;
            customer.House = house;
        }
    }

    public void AddOrder(List<ItemTemp> iorders, Customer customer)
    {

        ///создаем заказ
        Order order = new Order
        {
            Id = Random.Shared.NextInt64(),
            OrderDate = DateTime.Now,
            OrderPrice = 0,
            Customer = customer,
            OrderItems = new List<OrderItem>() { }

        };
        foreach (var item in iorders)
        {
            //создаем подзаказ
            OrderItem oi = new OrderItem
            {
                Id = Random.Shared.NextInt64(),
                // цена за тонну делить на 1000 кг умножить на кг
                Price = coals.GetById(item.Id).Price / 1000 * item.Weight,
                Weight = item.Weight,
                Coal = coals.GetById(item.Id)
            };
            orderItems.Insert(oi);
            order.OrderItems.Add(oi);
        }
        foreach (var item in order.OrderItems)
        {
            order.OrderPrice += item.Price;
        }
        orders.Insert(order);
    }
}