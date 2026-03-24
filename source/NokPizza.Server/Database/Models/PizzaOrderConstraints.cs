namespace NokPizza.Server.Database.Models;

public class PizzaOrderConstraints
{
    public Guid PizzaOrderId { get; set; }
    public PizzaOrder PizzaOrder { get; set; } = null!;

    public int ConstraintId { get; set; }
    public DietaryConstraint DietaryConstraint { get; set; } = null!;

    public int Count { get; set; }
}
