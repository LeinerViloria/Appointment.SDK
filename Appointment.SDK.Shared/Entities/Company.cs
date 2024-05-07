
using System.ComponentModel.DataAnnotations;

namespace Appointment.SDK.Entities;

public class Company : BaseEntity<int>
{
    [Required]
    public string Name { get; set; } = null!;

    [Required]
    public bool WorksOnMonday {get; set; }

    [Required]
    public bool WorksOnTuesday {get; set; }

    [Required]
    public bool WorksOnWednesday {get; set; }

    [Required]
    public bool WorksOnThursday {get; set; }

    [Required]
    public bool WorksOnFriday {get; set; }

    [Required]
    public bool WorksOnSaturday {get; set; }

    [Required]
    public bool WorksOnSunday {get; set; }
}