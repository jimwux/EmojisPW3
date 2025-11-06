using Microsoft.AspNetCore.Mvc;
using PW3.Emoji.Logica;

namespace PW3.Emoji.Web.Controllers;

public class AnalisisController : Controller
{

    private readonly IAnalisisLogica _analisisLogica;

    public AnalisisController(IAnalisisLogica analisisLogica)
    {
        _analisisLogica = analisisLogica;
    }

    [HttpGet]
    public IActionResult ListarAnalisis(int? emocionFilter, DateTime? fechaDesde = null, DateTime? fechaHasta = null, int pagina = 1, int? usuarioFilter = null)
    {
        bool esAdmin = HttpContext.Request.Cookies["Rol"] == "ADMIN";
        string? usuarioId = esAdmin ? null : HttpContext.Request.Cookies["UsuarioId"];
        int? usuarioIdInt = string.IsNullOrEmpty(usuarioId) ? null : Convert.ToInt32(usuarioId);
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

        return View(analisis);
    }
}
