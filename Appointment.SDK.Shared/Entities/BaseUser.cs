
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
        [SensitiveData]
        [StringLength(128, MinimumLength = 6, ErrorMessage = "La contraseña debe tener entre 6 y 14 caracteres.")]
        public string Password { get; set; } = null!;

        [Required]
        public EnumRecordStatus Status { get; set; }
    }
}
