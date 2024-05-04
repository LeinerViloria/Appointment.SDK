
using SDK.Shared.Dataanotations;
using SDK.Shared.Enums;
using System.ComponentModel.DataAnnotations;

namespace SDK.Shared.Entities
{
    public class User : BaseEntity<int>
    {
        [Required]
        [RegularExpression(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", ErrorMessage = "Correo inválido")]
        public string Email { get; set; } = null!;

        [Required]
        [SensitiveData]
        [StringLength(128, MinimumLength = 6, ErrorMessage = "La contraseña debe tener entre 6 y 14 caracteres.")]
        public string Password { get; set; } = null!;

        [Required]
        public bool IsAdmin { get; set; }

        [Required]
        public EnumRecordStatus Status { get; set; }
    }
}
