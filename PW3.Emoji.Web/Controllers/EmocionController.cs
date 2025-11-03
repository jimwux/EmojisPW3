using Microsoft.AspNetCore.Mvc;
using PW3.Emoji.Logica;
using Microsoft.AspNetCore.Hosting; // Necesario para _webHostEnvironment
using System.IO; // Necesario para Path
using System; // Necesario para Exception

namespace PW3.Emoji.Web.Controllers;

public class EmocionController : Controller
{
    private readonly IAnalisisEmocionLogica _analisisEmocionLogica;
    private readonly IWebHostEnvironment _webHostEnvironment;

    public EmocionController(IAnalisisEmocionLogica analisisEmocionLogica, IWebHostEnvironment webHostEnvironment)
    {
        _analisisEmocionLogica = analisisEmocionLogica;
        _webHostEnvironment = webHostEnvironment;
    }
    
    [HttpGet]
    public IActionResult Analizar()
    {
        // Esto simplemente muestra la vista (el formulario para subir la imagen)
        return View(); 
    }

    [HttpPost]
    public async Task<IActionResult> Analizar(IFormFile imagen)
    {
        if (imagen == null || imagen.Length == 0)
            return View("Analizar");

        // --- 1. Obtener el ID del usuario y la ruta del archivo ---
        if (!HttpContext.Request.Cookies.TryGetValue("UsuarioId", out string? usuarioIdString) 
            || !int.TryParse(usuarioIdString, out int usuarioId))
        {
            TempData["Error"] = "Tu sesión ha expirado. Por favor, inicia sesión.";
            return RedirectToAction("Login", "Usuario");
        }

        var nombreArchivo = Guid.NewGuid().ToString() + Path.GetExtension(imagen.FileName);
        string wwwRootPath = _webHostEnvironment.WebRootPath;
        var rutaUploads = Path.Combine(wwwRootPath, "uploads");
        var rutaDestino = Path.Combine(rutaUploads, nombreArchivo);
        var rutaParaDb = "/uploads/" + nombreArchivo;
        string emocion = "Error de Análisis"; // Valor por defecto

        // --- 2. TRY-CATCH QUE ENVUELVE TODA LA LÓGICA DE ARCHIVOS E IA ---
        try
        {
            // A. Guardar la imagen en wwwroot/uploads
            if (!Directory.Exists(rutaUploads))
                Directory.CreateDirectory(rutaUploads);

            using (var stream = new FileStream(rutaDestino, FileMode.Create))
            {
                await imagen.CopyToAsync(stream); 
            }

            // B. Analizar la emoción (Punto de Falla Común)
            emocion = _analisisEmocionLogica.ObtenerEmocionDesdeImagen(rutaDestino);

            // C. Validación y Guardado en BD
            if (emocion == "Desconocida" || emocion.Contains("Error") || string.IsNullOrEmpty(emocion))
            {
                // Si la IA no pudo procesar la imagen o el modelo falló
                throw new Exception($"El modelo de IA no pudo detectar la emoción. Resultado crudo: '{emocion}'. La imagen podría ser inválida o el archivo MLModel1.mlnet no se copió.");
            }
            
            // D. ¡Guardar el análisis en la BD!
            await _analisisEmocionLogica.GuardarAnalisisAsync(emocion, usuarioId, rutaParaDb);

            // 3. Pasar la emoción y la ruta a la vista
            ViewBag.Emocion = emocion;
            ViewBag.ImagenRuta = rutaParaDb;
            return View("Resultado");

        }
        catch (Exception ex)
        {
            // 4. Captura cualquier error (archivo, IA, o DB)
            TempData["Error"] = $"ERROR CRÍTICO: El análisis falló o la imagen no se procesó. Mensaje: {ex.Message}";
            
            // Devolver los datos que tenemos para debug
            ViewBag.Emocion = emocion; // Mostrará el valor por defecto o la emoción cruda
            ViewBag.ImagenRuta = rutaParaDb; 
            
            return View("Resultado"); 
        }
    }
}