using System.ComponentModel.DataAnnotations;

namespace NokPizza.Server.Database.Models;

public class PizzaOrder
{
    [Key]
    public Guid Id { get; init; } = Guid.NewGuid();

    public int NumberOfPeople { get; set; }

    public DateTime EndTime { get; set; }

    public ICollection<PizzaOrderConstraints> PizzaOrderConstraints { get; set; } = [];
}
