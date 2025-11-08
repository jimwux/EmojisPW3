using Microsoft.AspNetCore.Mvc;
using PW3.Emoji.Logica;
using PW3.Emoji.Web.Filters;

namespace PW3.Emoji.Web.Controllers;

[RequireLogin]
public class AnalisisController : Controller
{

    private readonly IAnalisisLogica _analisisLogica;
    private readonly IUsuarioLogica _usuarioLogica;

    public AnalisisController(IAnalisisLogica analisisLogica, IUsuarioLogica usuarioLogica)
    {
        _analisisLogica = analisisLogica;
        _usuarioLogica = usuarioLogica;
    }

    [HttpGet]
    public IActionResult ListarAnalisis(int? emocionFilter, DateTime? fechaDesde = null, DateTime? fechaHasta = null, int pagina = 1, int? usuarioFilter = null)
    {
        bool esAdmin = HttpContext.Session.GetString("Rol") == "ADMIN";
        int? usuarioIdInt = esAdmin ? null : HttpContext.Session.GetInt32("UsuarioId");
        var analisis = _analisisLogica.ObtenerAnalisis(usuarioIdInt, emocionFilter, fechaDesde, fechaHasta, pagina, usuarioFilter, esAdmin);

        ViewBag.Emociones = _analisisLogica.ObtenerEmociones();
        ViewBag.PaginaActual = pagina;
        ViewBag.EmocionFilter = emocionFilter;
        ViewBag.FechaDesde = fechaDesde;
        ViewBag.FechaHasta = fechaHasta;
        ViewBag.UsuarioFilter = usuarioFilter;

        ViewBag.EsAdmin = esAdmin;
        if (esAdmin)
        {
            ViewBag.Usuarios = _analisisLogica.ObtenerUsuarios();
        }

        var usuarioId = HttpContext.Session.GetInt32("UsuarioId");
        if (usuarioId.HasValue)
        {
            var nombre = _usuarioLogica.ObtenerUsuarioPorId(usuarioId.Value)?.Nombre ?? "Usuario";
            ViewBag.NombreUsuario = nombre;
        }

        return View(analisis);
    }
}
