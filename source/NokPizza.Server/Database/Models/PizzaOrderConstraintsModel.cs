namespace NokPizza.Server.Database.Models;

public class PizzaOrderConstraintsModel
{
    public Guid PizzaOrderId { get; set; }
    public PizzaOrderModel PizzaOrder { get; set; } = null!;

    public int ConstraintId { get; set; }
    public DietaryConstraintModel DietaryConstraint { get; set; } = null!;

    public int Count { get; set; }
}
