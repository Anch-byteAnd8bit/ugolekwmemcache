using Ugolek.Backend.Web.Application.Features.Coals;
using Ugolek.Backend.Web.Application.Features.Customers;
using Ugolek.Backend.Web.Application.Services;

namespace Ugolek.Backend.Web.Application.Features.Orders;

public class OrderService {
    private readonly IRepository<Order> _orders;

    private readonly IRepository<Coal> _coals;

    public OrderService(IRepository<Order> orders, IRepository<Coal> coals) {
        _orders = orders;
        _coals = coals;
    }

    /// <summary>
    /// Оформление/размещение заказа клиента.
    /// </summary>
    public void PlaceOrder(List<OrderItemDto> orderItems, Customer customer) {
        var coalsInOrder = _coals.Query().Where(x => orderItems.Select(y => y.Id).Contains(x.Id)).ToList();

        Order order = new Order {
            OrderDate = DateTime.Now,
            Id = Random.Shared.NextInt64(),
            OrderPrice = 0,
            Customer = customer,
            OrderItems = orderItems
                .Select(x => {
                    var coal = coalsInOrder.Single(y => y.Id == x.Id);

                    return OrderItem.CreateForPosition(coal, x.Weight);
                }).ToList()
        };

        foreach (var item in order.OrderItems)
        {
            order.OrderPrice += item.Price;
        }
        _orders.Insert(order);
    }

    /// <summary>
    /// Получение истории заказов клиента.
    /// </summary>
    public List<Order> GetCustomerOrders(string email)
    {
        var customerOrders = _orders.Query().Where(x => x.Customer.Email == email).ToList();
        return customerOrders;
    }
}