using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace NokPizza.Server.Database.Models;

[Index(nameof(NormalizedEmail))]
[Index(nameof(PizzaOrderId), nameof(NormalizedEmail), IsUnique = true)]
public class PizzaOrderParticipantModel
{
    [Key]
    public Guid Id { get; init; } = Guid.NewGuid();

    public Guid PizzaOrderId { get; set; }
    public PizzaOrderModel PizzaOrder { get; set; } = null!;

    [MaxLength(320)]
    public string Email { get; set; } = string.Empty;

    [MaxLength(320)]
    public string NormalizedEmail { get; set; } = string.Empty;

    public DateTime JoinedAt { get; set; }

    public ICollection<PizzaOrderParticipantConstraintModel> PizzaOrderParticipantConstraints { get; set; } =
    [];
}
