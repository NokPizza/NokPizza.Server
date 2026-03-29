using Microsoft.EntityFrameworkCore;

namespace NokPizza.Server.Database.Models;

[PrimaryKey(nameof(ParticipantId), nameof(ConstraintId))]
public class PizzaOrderParticipantConstraintModel
{
    public Guid ParticipantId { get; set; }
    public PizzaOrderParticipantModel Participant { get; set; } = null!;

    public int ConstraintId { get; set; }
    public DietaryConstraintModel DietaryConstraint { get; set; } = null!;
}
