using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace NokPizza.Server.Database.Models;

[Index(nameof(EndTime))]
[Index(nameof(RsvpDeadline))]
[Index(nameof(NormalizedCreatorEmail))]
public class PizzaOrderModel
{
    [Key]
    public Guid Id { get; init; } = Guid.NewGuid();

    [MaxLength(320)]
    public string? CreatorEmail { get; set; }

    [MaxLength(320)]
    public string? NormalizedCreatorEmail { get; set; }

    [MaxLength(500)]
    public string AdminPasswordHash { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? ParticipantPasswordHash { get; set; }

    public DateTime EndTime { get; set; }

    public DateTime RsvpDeadline { get; set; }

    public ICollection<PizzaOrderParticipantModel> PizzaOrderParticipants { get; set; } = [];
}
