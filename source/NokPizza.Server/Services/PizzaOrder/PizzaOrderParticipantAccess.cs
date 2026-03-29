using NokPizza.Server.Database.Models;

namespace NokPizza.Server.Services.PizzaOrder;

public sealed class PizzaOrderParticipantAccess
{
    internal PizzaOrderParticipantAccess(
        PizzaOrderModel order,
        bool isPasswordValid,
        bool isRsvpOpen
    )
    {
        Order = order;
        IsPasswordValid = isPasswordValid;
        IsRsvpOpen = isRsvpOpen;
    }

    internal PizzaOrderModel Order { get; }
    public bool IsPasswordValid { get; }
    public bool IsRsvpOpen { get; }
}
