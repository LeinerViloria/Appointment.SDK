
using System.ComponentModel.DataAnnotations;

namespace SDK.Shared.Entities
{
    public class User : BaseEntity<int>
    {
        [Required]
        [RegularExpression(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", ErrorMessage = "Correo inválido")]
        public string Email { get; set; } = null!;

        [Required]
        [StringLength(128, MinimumLength = 6, ErrorMessage = "The password must have between 6 and 14 characters.")]
        public string Password { get; set; } = null!;

        [Required]
        public bool IsAdmin { get; set; }
    }
}
