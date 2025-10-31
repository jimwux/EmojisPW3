using Microsoft.AspNetCore.Mvc;
using PW3.Emoji.Logica;

namespace PW3.Emoji.Web.Controllers
{
    public class EmocionController : Controller
    {
        private readonly ILogger<EmocionController> _logger;
        private readonly IAnalisisEmocionLogica _analisisEmocionLogica;

        public EmocionController(ILogger<EmocionController> logger, IAnalisisEmocionLogica analisisEmocionLogica)
        {
            _logger = logger;
            _analisisEmocionLogica = analisisEmocionLogica;
        }

        [HttpGet]
        public IActionResult Analizar()
        {
            if (!HttpContext.Request.Cookies.ContainsKey("UsuarioId"))
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

            using var memoryStream = new MemoryStream();
            await imagen.CopyToAsync(memoryStream);
            byte[] imageBytes = memoryStream.ToArray();

            var faces = _analisisEmocionLogica.DetectFaces(imageBytes);

            if (faces.Count == 0)
            {
                TempData["Message"] = "No se detectaron caras en la imagen. Intenta con otra imagen.";
                return View("Analizar");
            }

            var results = new List<EmocionResult>();
            const float CONFIDENCE_THRESHOLD = 0.60f;
            await _analisisEmocionLogica.ProcessFacesAsync(faces, imageBytes, CONFIDENCE_THRESHOLD, results);

            ViewBag.Results = results;
            ViewBag.ImagenRuta = "/uploads/" + imagen.FileName;

            var rutaUploads = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
            if (!Directory.Exists(rutaUploads))
                Directory.CreateDirectory(rutaUploads);

            var rutaDestino = Path.Combine(rutaUploads, imagen.FileName);
            System.IO.File.WriteAllBytes(rutaDestino, imageBytes);

            return View("Resultado");
        }
    }
}
