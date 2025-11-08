using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using PW3.Emoji.Entidades.EF;
using PW3.Emoji.Logica;
using PW3.Emoji.Web.Controllers;
using PW3.Emoji.Web.Models;
using Xunit;

namespace PW3.Emoji.Test.PW3.Emoji.Web.Unit;

public class UsuarioControllerUnitTest
{
    [Fact]
    public void Registro_Get_RetornaView()
    {
        var mockLogica = new Mock<IUsuarioLogica>();
        var controller = new UsuarioController(mockLogica.Object);

        var result = controller.Registro();

        Assert.IsType<ViewResult>(result);
    }

    [Fact]
    public void Registro_Post_ModelInvalido_RetornaViewConModelo()
    {
        var mockLogica = new Mock<IUsuarioLogica>();
        var controller = new UsuarioController(mockLogica.Object);
        controller.ModelState.AddModelError("Email", "Requerido");

        var usuarioVm = new UsuarioViewModel { Email = "test@mail.com" };

        var result = controller.Registro(usuarioVm);

        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Equal(usuarioVm, viewResult.Model);
    }

    [Fact]
    public void Registro_Post_ModelValido_CreaUsuarioYRedirige()
    {
        var mockLogica = new Mock<IUsuarioLogica>();
        var controller = new UsuarioController(mockLogica.Object);

        var tempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());
        controller.TempData = tempData;

        var usuarioVm = new UsuarioViewModel { Email = "test@mail.com", HashPassword = "1234", Nombre = "Test" };

        var result = controller.Registro(usuarioVm);

        mockLogica.Verify(x => x.CrearUsuario(It.Is<Usuario>(u => u.Email == usuarioVm.Email)), Times.Once);
        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Login", redirect.ActionName);
        Assert.Equal("Usuario registrado exitosamente.", controller.TempData["Mensaje"]);
    }

    [Fact]
    public void Login_Get_RetornaView()
    {
        var mockLogica = new Mock<IUsuarioLogica>();
        var controller = new UsuarioController(mockLogica.Object);

        var result = controller.Login();

        Assert.IsType<ViewResult>(result);
    }

    [Fact]
    public void Login_Post_UsuarioValido_SeteaSesionYRedirige()
    {
        var usuario = new Usuario { Id = 7, Email = "test@mail.com", Nombre = "Test", Rol = new Rol { Nombre = "USUARIO" } };
        var mockLogica = new Mock<IUsuarioLogica>();
        mockLogica.Setup(x => x.Login("test@mail.com", "1234")).Returns(usuario);

        var controller = new UsuarioController(mockLogica.Object);
        var httpContext = new DefaultHttpContext();
        httpContext.Session = new Mock<ISession>().Object;
        controller.ControllerContext = new ControllerContext { HttpContext = httpContext };

        var result = controller.Login("test@mail.com", "1234");

        Assert.IsType<RedirectToActionResult>(result);
    }

    [Fact]
    public void Login_Post_UsuarioInvalido_RetornaViewConError()
    {
        var mockLogica = new Mock<IUsuarioLogica>();
        mockLogica.Setup(x => x.Login("test@mail.com", "badpass")).Returns((Usuario?)null);

        var controller = new UsuarioController(mockLogica.Object);
        var httpContext = new DefaultHttpContext();
        controller.ControllerContext = new ControllerContext { HttpContext = httpContext };

        var tempData = new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>());
        controller.TempData = tempData;

        var result = controller.Login("test@mail.com", "badpass");

        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Equal("Credenciales inválidas. Por favor, intente de nuevo.", controller.TempData["Error"]);
    }

    [Fact]
    public void CerrarSesion_LimpiaSesionYRedirige()
    {
        var mockLogica = new Mock<IUsuarioLogica>();
        var controller = new UsuarioController(mockLogica.Object);

        var sessionMock = new Mock<ISession>();
        var httpContext = new DefaultHttpContext();
        httpContext.Session = sessionMock.Object;
        controller.ControllerContext = new ControllerContext { HttpContext = httpContext };

        var result = controller.CerrarSesion();

        sessionMock.Verify(s => s.Clear(), Times.Once);
        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Login", redirect.ActionName);
    }
}