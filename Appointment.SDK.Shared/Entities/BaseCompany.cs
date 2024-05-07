
using System.ComponentModel.DataAnnotations;

namespace Appointment.SDK.Entities;

public class BaseCompany<T> : BaseEntity<T>
{
    [Required]
    public int RowidCompany {get; set;}

    public Company? Company { get; set;}
}