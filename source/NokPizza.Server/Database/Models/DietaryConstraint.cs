using System.ComponentModel.DataAnnotations;

namespace NokPizza.Server.Database.Models;

public class DietaryConstraint
{
    [Key]
    public int Id { get; set; }

    [MaxLength(50)]
    public string Name { get; set; } = string.Empty;
}
