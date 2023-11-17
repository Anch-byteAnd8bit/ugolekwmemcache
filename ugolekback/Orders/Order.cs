using ugolekback.Coals.Model;
using ugolekback.Core;
using ugolekback.CustomerF;

namespace ugolekback.OrderF;

public record Order : IEntity
{
    public long Id { get; set; }
    public DateTime OrderDate { get; set; }
    public decimal OrderPrice { get; set; }
    public required Customer Customer { get; set; }

    public required List<OrderItem> OrderItems { get; set; }
}


/*public class OrderDB
{
    public static List<Order> _orders = new List<Order>()
    {
        new Order
        {
            Id=1,
            OrderDate= DateTime.Now,
            OrderPrice = 520000,
            Customer= CustomerDB._customers[0],
            OrderItems= new List<OrderItem>() { OrderItemDB._orderitems[0], OrderItemDB._orderitems[1], OrderItemDB._orderitems[2]}
        },
        new Order
        {
            Id=2,
            OrderDate= DateTime.Now,
            OrderPrice = 300000,
            Customer= CustomerDB._customers[1],
            OrderItems= new List<OrderItem>() { OrderItemDB._orderitems[3]}
        }
    };


    //ввод заказа
    public static void AddOrder(List<ItemTemp> orders, HttpContext context)
    {
        int? idC = context.Session.GetInt32("_id");

        //Нашли ID.
        if (idC.HasValue)
        {
            Customer? customer = CustomerDB._customers.SingleOrDefault(customer => customer.Id == idC);
            // Нашли пользователя по ID.
            if (customer != null)
            {
                ///создаем заказ
                Order or = new Order
                {
                    Id = OrderDB._orders.Last().Id + 1,
                    OrderDate = DateTime.Now,
                    OrderPrice = 0,
                    Customer = customer,
                    OrderItems = new List<OrderItem>() { }

                };
                foreach (var item in orders)
                {
                    //создаем подзаказ
                    OrderItem oi = new OrderItem
                    {
                        Id = OrderItemDB._orderitems.Last().Id + 1,
                        // цена за тонну делить на 1000 кг умножить на кг
                        Price = CoalsInMemRepository._coals[item.Id].Price / 1000 * item.Weight,
                        Weight = item.Weight,
                        Coal = CoalsInMemRepository._coals[item.Id]
                    };

                    OrderItemDB._orderitems.Add(oi);
                    or.OrderItems.Add(oi);


                }
                foreach (var item in or.OrderItems)
                {
                    or.OrderPrice += item.Price;
                }
                _orders.Add(or);
            }
        }
        // Не нашли ID.
        else
        {
            Console.WriteLine("Bad");
        }



    }

    //получить заказы пользователя
    public static List<Order> GetCustomerOrders(HttpContext context)
    {
        int? idC = context.Session.GetInt32("_id");
        List<Order> _customerorder = new List<Order>() { };
        //Нашли ID.
        if (idC.HasValue)
        {
            Customer? customer = CustomerDB._customers.SingleOrDefault(customer => customer.Id == idC);
            if (customer != null)
            {
                foreach (var item in _orders)
                {
                    if (item.Customer.Id == customer.Id)
                    {
                        _customerorder.Add(item);
                    }
                }
            }


            return _customerorder;
        }
        else
        {
            Console.WriteLine("Bad");
            return _customerorder;
        }
    }



}*/