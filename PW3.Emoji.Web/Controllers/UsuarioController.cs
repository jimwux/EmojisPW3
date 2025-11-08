using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PW3.Emoji.Entidades.EF;
using PW3.Emoji.Logica;
using PW3.Emoji.Web.Filters;
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
    [RequireNotLogged]
    public IActionResult Registro()
    {
        return View();
    }

    [HttpPost]
    [RequireNotLogged]
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
    [RequireNotLogged]
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    [AllowAnonymous]
    [RequireNotLogged]
    public IActionResult Login(string email, string password)
    {
        var usuario = _usuarioLogica.Login(email, password);
        if (usuario != null)
        {
            HttpContext.Session.SetInt32("UsuarioId", usuario.Id);
            HttpContext.Session.SetString("Rol", usuario.Rol.Nombre);
            HttpContext.Session.SetString("UsuarioNombre", usuario.Nombre);
            return RedirectToAction("Analizar", "Emocion");
        }
        else
        {
            TempData["Error"] = "Credenciales inválidas. Por favor, intente de nuevo.";
            return View();
        }
    }

    [RequireLogin]
    public IActionResult CerrarSesion()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Login");
    }

}