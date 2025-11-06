using Microsoft.EntityFrameworkCore;
using PW3.Emoji.Entidades.EF;

namespace PW3.Emoji.Logica;

public interface IAnalisisLogica
{
    List<AnalisisResultado> ObtenerAnalisis(int? emocionFilter, DateTime? fechaDesde, DateTime? fechaHasta, int pagina, int? usuarioFilter = null);
    List<Emocion> ObtenerEmociones();
    List<Usuario> ObtenerUsuarios();
}

public class AnalisisLogica : IAnalisisLogica
{
    private readonly PW3_EmojiContext _context;

    public AnalisisLogica(PW3_EmojiContext context)
    {
        _context = context;
    }

    public List<AnalisisResultado> ObtenerAnalisis(int? emocionFilter, DateTime? fechaDesde, DateTime? fechaHasta, int pagina, int? usuarioFilter = null)
    {
        var query = _context.AnalisisResultados
            .Include(a => a.Emocion)
            .Include(a => a.Usuario)
            .Include(a => a.Imagen)
            .Where(a => (!emocionFilter.HasValue || a.EmocionId == emocionFilter.Value) &&
                        (!fechaDesde.HasValue || a.FechaAnalisis >= fechaDesde.Value) &&
                        (!fechaHasta.HasValue || a.FechaAnalisis <= fechaHasta.Value));

        if (usuarioFilter.HasValue)
            query = query.Where(a => a.UsuarioId == usuarioFilter.Value);

        return query
            .OrderByDescending(a => a.FechaAnalisis)
            .Skip((pagina - 1) * 10)
            .Take(10)
            .ToList();
    }

    public List<Emocion> ObtenerEmociones()
    {
        return _context.Emocion.ToList();
    }

    public List<Usuario> ObtenerUsuarios()
    {
        return _context.Usuario.ToList();
    }
}
