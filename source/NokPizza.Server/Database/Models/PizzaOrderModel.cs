using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace NokPizza.Server.Database.Models;

[Index(nameof(EndTime))]
public class PizzaOrderModel
{
    [Key]
    public Guid Id { get; init; } = Guid.NewGuid();

    public int NumberOfPeople { get; set; }

    public DateTime EndTime { get; set; }

    public ICollection<PizzaOrderConstraintsModel> PizzaOrderConstraints { get; set; } = [];
}
