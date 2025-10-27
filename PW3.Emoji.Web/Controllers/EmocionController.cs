using Microsoft.AspNetCore.Mvc;
using PW3.Emoji.Logica;

namespace PW3.Emoji.Web.Controllers;
public class EmocionController : Controller
{
    private readonly IAnalisisEmocionLogica _analisisEmocionLogica;

    public EmocionController(IAnalisisEmocionLogica analisisEmocionLogica)
    {
        _analisisEmocionLogica = analisisEmocionLogica;
    }

    [HttpGet]
    public IActionResult Analizar()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Analizar(IFormFile imagen)
    {
        if (imagen == null || imagen.Length == 0)
            return View("Analizar");

        // 1️⃣ Guardar la imagen temporalmente
        var rutaTemporal = Path.Combine(Path.GetTempPath(), imagen.FileName);
        using (var stream = new FileStream(rutaTemporal, FileMode.Create))
        {
            imagen.CopyTo(stream);
        }

        // 2️⃣ Analizar la emoción
        var emocion = _analisisEmocionLogica.ObtenerEmocionDesdeImagen(rutaTemporal);

        // 3️⃣ Pasar la emoción y la ruta a la vista
        ViewBag.Emocion = emocion;
        ViewBag.ImagenRuta = "/uploads/" + imagen.FileName;

        // 4️⃣ Guardar la imagen en wwwroot/uploads (para poder mostrarla)
        var rutaUploads = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
        if (!Directory.Exists(rutaUploads))
            Directory.CreateDirectory(rutaUploads);

        var rutaDestino = Path.Combine(rutaUploads, imagen.FileName);
        System.IO.File.Copy(rutaTemporal, rutaDestino, true);

        return View("Resultado");
    }

}
