using Microsoft.AspNetCore.Mvc;
using PW3.Emoji.Entidades.EF;
using PW3.Emoji.Logica;
using PW3.Emoji.Web.Mappers;
using PW3.Emoji.Web.Models;

namespace PW3.Emoji.Web.Controllers;

public class UsuarioController : Controller
{
    private readonly IUsuarioLogica _usuarioLogica;

    public UsuarioController(
        IUsuarioLogica usuarioLogica)
    {
        _usuarioLogica = usuarioLogica;
    }

    [HttpGet]
    public IActionResult Registro()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Registro(UsuarioViewModel usuario)
    {
        if (!ModelState.IsValid)
        {
            return View(usuario);
        }
        Usuario usuarioEntity = UsuarioMapper.ToEntity(usuario);
        _usuarioLogica.CrearUsuario(usuarioEntity);
        TempData["Mensaje"] = "Usuario registrado exitosamente.";
        return RedirectToAction("Login");
    }

    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Login(string email, string password)
    {
        var usuario = _usuarioLogica.Login(email, password);
        if (usuario != null)
        {
            HttpContext.Response.Cookies.Append("UsuarioId", usuario.Id.ToString(), new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTimeOffset.UtcNow.AddHours(2)
            });
            return RedirectToAction("Analizar", "Emocion");
        }
        else
        {
            TempData["Error"] = "Credenciales inválidas. Por favor, intente de nuevo.";
            return View();
        }
    }

}