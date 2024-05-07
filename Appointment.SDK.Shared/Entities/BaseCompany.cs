
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Appointment.SDK.Entities;

public class BaseCompany<T> : BaseEntity<T>
{
    [Required]
    [ForeignKey("Company")]
    public int RowidCompany {get; set;}

    public Company? Company { get; set;}
}