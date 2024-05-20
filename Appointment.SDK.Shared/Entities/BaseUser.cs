
using Appointment.SDK.Dataanotations;
using Appointment.Globals.Enums;
using System.ComponentModel.DataAnnotations;

namespace Appointment.SDK.Entities
{
    public abstract class BaseUser<T> : BaseEntity<T>
    {
        [Required]
        [RegularExpression(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", ErrorMessage = "Correo inválido")]
        public string Email { get; set; } = null!;

        [Required]
        public EnumRecordStatus Status { get; set; }
    }
}
