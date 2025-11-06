using Microsoft.EntityFrameworkCore;
using PW3.Emoji.Entidades.EF;

namespace PW3.Emoji.Logica;

public interface IAnalisisLogica
{
    List<AnalisisResultado> ObtenerAnalisis(int? emocionFilter, DateTime? fechaDesde, DateTime? fechaHasta, int pagina);
    List<Emocion> ObtenerEmociones();
}

public class AnalisisLogica : IAnalisisLogica
{
    private readonly PW3_EmojiContext _context;

    public AnalisisLogica(PW3_EmojiContext context)
    {
        _context = context;
    }

    public List<AnalisisResultado> ObtenerAnalisis(int? emocionFilter, DateTime? fechaDesde, DateTime? fechaHasta, int pagina)
    {
        List<AnalisisResultado> resultados = new List<AnalisisResultado>();
        resultados = _context.AnalisisResultados
            .Include(a => a.Emocion)
            .Include(a => a.Usuario)
            .Include(a => a.Imagen)
            .Where(a => (!emocionFilter.HasValue || a.EmocionId == emocionFilter.Value) &&
                        (!fechaDesde.HasValue || a.FechaAnalisis >= fechaDesde.Value) &&
                        (!fechaHasta.HasValue || a.FechaAnalisis <= fechaHasta.Value))
            .OrderByDescending(a => a.FechaAnalisis)
            .Skip((pagina - 1) * 10)
            .Take(10)
            .ToList();

        return resultados;
    }

    public List<Emocion> ObtenerEmociones()
    {
        return _context.Emocion.ToList();
    }
}
