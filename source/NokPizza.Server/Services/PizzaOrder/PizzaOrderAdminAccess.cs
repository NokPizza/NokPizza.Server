using NokPizza.Server.Database.Models;

namespace NokPizza.Server.Services.PizzaOrder;

public sealed class PizzaOrderAdminAccess
{
    internal PizzaOrderAdminAccess(PizzaOrderModel order, bool isPasswordValid)
    {
        Order = order;
        IsPasswordValid = isPasswordValid;
    }

    internal PizzaOrderModel Order { get; }
    public bool IsPasswordValid { get; }
}
