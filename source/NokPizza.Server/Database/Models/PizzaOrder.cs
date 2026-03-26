using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace NokPizza.Server.Database.Models;

[Index(nameof(EndTime))]
public class PizzaOrder
{
    [Key]
    public Guid Id { get; init; } = Guid.NewGuid();

    public int NumberOfPeople { get; set; }

    public DateTime EndTime { get; set; }

    public ICollection<PizzaOrderConstraints> PizzaOrderConstraints { get; set; } = [];
}
