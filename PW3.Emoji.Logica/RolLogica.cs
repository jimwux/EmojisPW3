using PW3.Emoji.Entidades.EF;

namespace PW3.Emoji.Logica;

public interface IRolLogica
{
    List<Rol> ObtenerRoles();
    Rol? ObtenerRolPorNombre(string nombre);
}

public class RolLogica : IRolLogica
{
    private readonly PW3_EmojiContext _context;

    public RolLogica(PW3_EmojiContext context)
    {
        _context = context;
    }

    public List<Rol> ObtenerRoles()
    {
        return _context.Rol.ToList();
    }

    public Rol? ObtenerRolPorNombre(string nombre)
    {
        return _context.Rol.FirstOrDefault(r => r.Nombre == nombre);
    }
}