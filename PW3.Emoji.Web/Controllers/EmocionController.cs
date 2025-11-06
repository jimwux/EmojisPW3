using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using PW3.Emoji.Logica;
using System;
using System.IO;
using PW3.Emoji.Logica.Utils;

namespace PW3.Emoji.Web.Controllers;

public class EmocionController : Controller
{
    private readonly ILogger<EmocionController> _logger;
    private readonly IAnalisisEmocionLogica _analisisEmocionLogica;
    private readonly IWebHostEnvironment _webHostEnvironment;

    public EmocionController(ILogger<EmocionController> logger, IAnalisisEmocionLogica analisisEmocionLogica, IWebHostEnvironment webHostEnvironment)
    {
        _logger = logger;
        _analisisEmocionLogica = analisisEmocionLogica;
        _webHostEnvironment = webHostEnvironment;
    }

    [HttpGet]
    public IActionResult Analizar()
    {
        if (!HttpContext.Session.TryGetValue("UsuarioId", out _))
        {
            return RedirectToAction("Login", "Usuario");
        }
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Analizar(IFormFile imagen)
    {
        if (imagen == null || imagen.Length == 0)
            return View("Analizar");

        // Obtener el ID del usuario
        if (!HttpContext.Session.TryGetValue("UsuarioId", out _))
        {
            return RedirectToAction("Login", "Usuario");
        }

        int? usuarioId = HttpContext.Session.GetInt32("UsuarioId");
        if (usuarioId == null)
        {
            TempData["Error"] = "Tu sesión ha expirado. Por favor, inicia sesión.";
            return RedirectToAction("Login", "Usuario");
        }

        var nombreArchivo = Guid.NewGuid().ToString() + Path.GetExtension(imagen.FileName);
        string wwwRootPath = _webHostEnvironment.WebRootPath;
        var rutaUploads = Path.Combine(wwwRootPath, "uploads");
        var rutaDestino = Path.Combine(rutaUploads, nombreArchivo);
        var rutaParaDb = "/uploads/" + nombreArchivo;

        try
        {
            // Guardar la imagen en wwwroot/uploads
            if (!Directory.Exists(rutaUploads))
                Directory.CreateDirectory(rutaUploads);

            using var memoryStream = new MemoryStream();
            await imagen.CopyToAsync(memoryStream);
            byte[] imageBytes = memoryStream.ToArray();

            // Guardar archivo en disco
            await System.IO.File.WriteAllBytesAsync(rutaDestino, imageBytes);

            // Detectar caras en la imagen
            var faces = _analisisEmocionLogica.DetectFaces(imageBytes);
            _logger.LogInformation($"POST Analizar: caras detectadas = {faces.Count}");

            if (faces.Count == 0)
            {
                TempData["Message"] = "No se detectaron caras en la imagen. Intenta con otra imagen.";
                return View("Analizar");
            }

            // Procesar las caras detectadas
            var results = new List<EmocionResult>();
            const float CONFIDENCE_THRESHOLD = 0.50f;
            await _analisisEmocionLogica.ProcessFacesAsync(faces, imageBytes, CONFIDENCE_THRESHOLD, results);

            _logger.LogInformation($"POST Analizar: resultados totales = {results.Count}, reconocidos = {results.Count(r => r.IsRecognized)}");

            if (results.Count == 0)
            {
                TempData["Message"] = "No se obtuvieron predicciones del modelo. Revisa los logs.";
                return View("Analizar");
            }

            // Selecciona la emoción con mayor confianza
            var emocionPrincipal = results.OrderByDescending(r => r.Confidence).First();
            string emocionNombre = emocionPrincipal.Emocion;

            // Validar que la emoción sea válida
            if (string.IsNullOrEmpty(emocionNombre) || emocionNombre == "Desconocida")
            {
                throw new Exception($"El modelo de IA no pudo detectar la emoción. Resultado: '{emocionNombre}'.");
            }

            // Guardar el análisis en la BD
            await _analisisEmocionLogica.GuardarAnalisisAsync(emocionNombre, usuarioId.Value, rutaParaDb, emocionPrincipal.Confidence);

            // Pasar datos a la vista
            ViewBag.Emocion = EmotionTraduction.Traduct(emocionNombre);
            ViewBag.ImagenRuta = rutaParaDb;
            ViewBag.Confidence = emocionPrincipal.Confidence;

            return View("Resultado");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al procesar la imagen");
            TempData["Error"] = $"ERROR: El análisis falló. Mensaje: {ex.Message}";
            return View("Analizar");
        }
    }
}
