using System.ComponentModel.DataAnnotations;

namespace Appointment.SDK.Entities
{
    public abstract class BaseEntity<T>
    {
        [Key]
        public virtual T Rowid { get; set; } = default!;
    }
}
