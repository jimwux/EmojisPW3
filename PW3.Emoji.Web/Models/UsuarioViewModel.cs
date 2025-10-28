using System.ComponentModel.DataAnnotations;

namespace PW3.Emoji.Web.Models
{
    public class UsuarioViewModel
    {
        [Required(ErrorMessage = "El nombre es obligatorio.")]
        public string Nombre { get; set; } = null!;

        [Required(ErrorMessage = "El correo electrónico es obligatorio.")]
        [EmailAddress(ErrorMessage = "El correo electrónico no es válido.")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "La contraseña es obligatoria.")]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "La contraseña debe tener entre 8 y 100 caracteres.")]
        public string HashPassword { get; set; } = null!;

        [Required(ErrorMessage = "El rol es obligatorio.")]
        public int RolId { get; set; }
    }
}