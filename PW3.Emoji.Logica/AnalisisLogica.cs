using Microsoft.EntityFrameworkCore;
using PW3.Emoji.Entidades.EF;

namespace PW3.Emoji.Logica;

public interface IAnalisisLogica
{
    List<AnalisisResultado> ObtenerAnalisis(int? usuarioId, int? emocionFilter, DateTime? fechaDesde, DateTime? fechaHasta, int pagina, int? usuarioFilter = null, bool esAdmin = false);
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

    public List<AnalisisResultado> ObtenerAnalisis(int? usuarioId, int? emocionFilter, DateTime? fechaDesde, DateTime? fechaHasta, int pagina, int? usuarioFilter = null, bool esAdmin = false)
    {
        var query = _context.AnalisisResultados
            .Include(a => a.Emocion)
            .Include(a => a.Usuario)
            .Include(a => a.Imagen)
            .Where(a => (!emocionFilter.HasValue || a.EmocionId == emocionFilter.Value) &&
                        (!fechaDesde.HasValue || a.FechaAnalisis >= fechaDesde.Value) &&
                        (!fechaHasta.HasValue || a.FechaAnalisis <= fechaHasta.Value) &&
                        (esAdmin || a.Usuario.Id == usuarioId));

        if (usuarioFilter.HasValue)
            query = query.Where(a => a.UsuarioId == usuarioFilter.Value);

        int pageSize = esAdmin ? 3 : 4;
        pagina = Math.Max(1, pagina);

        return query
            .OrderByDescending(a => a.FechaAnalisis)
            .Skip((pagina - 1) * pageSize)
            .Take(pageSize)
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
