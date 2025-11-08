using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using PW3.Emoji.Entidades.EF;
using PW3.Emoji.Logica;
using PW3.Emoji.Web.Controllers;

namespace PW3.Emoji.Test.Web.Unit;

public class AnalisisControllerUnitTest
{
    [Fact]
    public void ListarAnalisis_UsuarioComun_RetornaViewConDatos()
    {
        var mockAnalisisLogica = new Mock<IAnalisisLogica>();
        var mockUsuarioLogica = new Mock<IUsuarioLogica>();

        var analisisList = new List<AnalisisResultado> { new AnalisisResultado { Id = 1 } };
        mockAnalisisLogica.Setup(x => x.ObtenerAnalisis(It.IsAny<int?>(), null, null, null, 1, null, false))
            .Returns(analisisList);
        mockAnalisisLogica.Setup(x => x.ObtenerEmociones()).Returns(new List<Emocion> { new Emocion { Id = 1, Nombre = "Feliz" } });

        mockUsuarioLogica.Setup(x => x.ObtenerUsuarioPorId(It.IsAny<int>())).Returns(new Usuario { Id = 1, Nombre = "Juan" });

        var controller = new AnalisisController(mockAnalisisLogica.Object, mockUsuarioLogica.Object);

        var httpContext = new DefaultHttpContext();
        httpContext.Session = CreateMockSession("USUARIO", 1);
        controller.ControllerContext = new ControllerContext { HttpContext = httpContext };

        var result = controller.ListarAnalisis(null);

        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Equal(analisisList, viewResult.Model);
        Assert.Equal("Juan", controller.ViewBag.NombreUsuario);
        Assert.False(controller.ViewBag.EsAdmin);
        Assert.NotNull(controller.ViewBag.Emociones);
    }

    [Fact]
    public void ListarAnalisis_Admin_RetornaViewConUsuarios()
    {
        var mockAnalisisLogica = new Mock<IAnalisisLogica>();
        var mockUsuarioLogica = new Mock<IUsuarioLogica>();

        var analisisList = new List<AnalisisResultado> { new AnalisisResultado { Id = 2 } };
        mockAnalisisLogica.Setup(x => x.ObtenerAnalisis(null, null, null, null, 1, null, true)).Returns(analisisList);
        mockAnalisisLogica.Setup(x => x.ObtenerEmociones()).Returns(new List<Emocion> { new Emocion { Id = 2, Nombre = "Triste" } });
        mockAnalisisLogica.Setup(x => x.ObtenerUsuarios()).Returns(new List<Usuario> { new Usuario { Id = 1, Nombre = "Juan" }, new Usuario { Id = 2, Nombre = "Ana" } });

        var controller = new AnalisisController(mockAnalisisLogica.Object, mockUsuarioLogica.Object);

        var httpContext = new DefaultHttpContext();
        httpContext.Session = CreateMockSession("ADMIN", 99);
        controller.ControllerContext = new ControllerContext { HttpContext = httpContext };

        var result = controller.ListarAnalisis(null);

        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Equal(analisisList, viewResult.Model);
        Assert.True(controller.ViewBag.EsAdmin);
        Assert.NotNull(controller.ViewBag.Usuarios);
        Assert.NotNull(controller.ViewBag.Emociones);
    }

    [Fact]
    public void ListarAnalisis_SinUsuarioEnSesion_RetornaViewConNombrePorDefecto()
    {
        var mockAnalisisLogica = new Mock<IAnalisisLogica>();
        var mockUsuarioLogica = new Mock<IUsuarioLogica>();

        var analisisList = new List<AnalisisResultado>();
        mockAnalisisLogica.Setup(x => x.ObtenerAnalisis(null, null, null, null, 1, null, false)).Returns(analisisList);
        mockAnalisisLogica.Setup(x => x.ObtenerEmociones()).Returns(new List<Emocion>());

        var controller = new AnalisisController(mockAnalisisLogica.Object, mockUsuarioLogica.Object);

        var httpContext = new DefaultHttpContext();
        httpContext.Session = CreateMockSession("USUARIO", null);
        controller.ControllerContext = new ControllerContext { HttpContext = httpContext };

        var result = controller.ListarAnalisis(null);

        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Null(controller.ViewBag.NombreUsuario);
    }

    private ISession CreateMockSession(string rol, int? usuarioId)
    {
        var mockSession = new Mock<ISession>();

#pragma warning disable CS8601
        mockSession.Setup(s => s.TryGetValue("Rol", out It.Ref<byte[]>.IsAny))
            .Callback(new TryGetValueCallback((string key, out byte[] value) =>
            {
                value = rol != null ? System.Text.Encoding.UTF8.GetBytes(rol) : null!;
            }))
            .Returns(rol != null);
#pragma warning restore CS8601

        if (usuarioId.HasValue)
        {
            byte[] userIdBytes = BitConverter.GetBytes(usuarioId.Value);
#pragma warning disable CS8601
            mockSession.Setup(s => s.TryGetValue("UsuarioId", out It.Ref<byte[]>.IsAny))
                .Callback(new TryGetValueCallback((string key, out byte[] value) =>
                {
                    value = userIdBytes;
                }))
                .Returns(true);
#pragma warning restore CS8601
        }
        else
        {
#pragma warning disable CS8601
            mockSession.Setup(s => s.TryGetValue("UsuarioId", out It.Ref<byte[]>.IsAny))
                .Callback(new TryGetValueCallback((string key, out byte[] value) =>
                {
                    value = null!;
                }))
                .Returns(false);
#pragma warning restore CS8601
        }

        return mockSession.Object;
    }

    private delegate void TryGetValueCallback(string key, out byte[] value);
}