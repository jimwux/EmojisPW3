using Microsoft.AspNetCore.Identity;
using PW3.Emoji.Entidades.EF;

namespace PW3.Emoji.Logica
{
    public interface IUsuarioLogica
    {
        void CrearUsuario(Usuario usuario);
        Usuario? Login(string email, string password);
    }

    public class UsuarioLogica : IUsuarioLogica
    {

        private readonly PW3_EmojiContext _context;
        private readonly IRolLogica _rolLogica;

        public UsuarioLogica(PW3_EmojiContext context, IRolLogica rolLogica)
        {
            _context = context;
            _rolLogica = rolLogica;
        }

        public void CrearUsuario(Usuario usuario)
        {
            usuario.HashPassword = new PasswordHasher<Usuario>()
                .HashPassword(usuario, usuario.HashPassword);
            usuario.Rol = _rolLogica.ObtenerRolPorNombre("USUARIO")!;
            _context.Usuario.Add(usuario);
            _context.SaveChanges();
        }

        public Usuario? Login(string email, string password)
        {
            var usuario = _context.Usuario
                .FirstOrDefault(u => u.Email == email);
            if (usuario != null && VerificarPassword(usuario, password))
            {
                return usuario;
            }
            return null;
        }

        public static bool VerificarPassword(Usuario usuario, string passwordIngresada)
        {
            var hasher = new PasswordHasher<Usuario>();
            var resultado = hasher.VerifyHashedPassword(usuario, usuario.HashPassword, passwordIngresada);
            return resultado == PasswordVerificationResult.Success;
        }
    }
}