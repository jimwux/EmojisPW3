using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Logging;
using Moq;
using PW3.Emoji.Entidades.EF;
using PW3.Emoji.Logica;
using PW3.Emoji.Web.Controllers;
using System.Drawing;
using Xunit;

namespace PW3.Emoji.Test.PW3.Emoji.Web.Unit;

public class EmocionControllerUnitTest
{
    private readonly Mock<ILogger<EmocionController>> _mockLogger;
    private readonly Mock<IAnalisisEmocionLogica> _mockAnalisisLogica;
    private readonly Mock<IWebHostEnvironment> _mockWebHostEnvironment;
    private readonly DefaultHttpContext _httpContext;

public EmocionControllerUnitTest()
{
    _mockLogger = new Mock<ILogger<EmocionController>>();
    _mockAnalisisLogica = new Mock<IAnalisisEmocionLogica>();
    _mockWebHostEnvironment = new Mock<IWebHostEnvironment>();
    _httpContext = new DefaultHttpContext();
    _httpContext.Session = new Mock<ISession>().Object;
}

    [Fact]
    public void Analizar_Get_RetornaView()
    {
        var controller = new EmocionController(_mockLogger.Object, _mockAnalisisLogica.Object, _mockWebHostEnvironment.Object);

        var result = controller.Analizar();

        Assert.IsType<ViewResult>(result);
    }

    [Fact]
    public async Task Analizar_Post_ImagenNula_RetornaVistaAnalizar()
    {
        var controller = new EmocionController(_mockLogger.Object, _mockAnalisisLogica.Object, _mockWebHostEnvironment.Object);

#pragma warning disable CS8625 
        var result = await controller.Analizar(null);
#pragma warning restore CS8625 

        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Equal("Analizar", viewResult.ViewName);
    }

    [Fact]
    public async Task Analizar_Post_SinSesion_RedireccionaALogin()
    {
        var controller = new EmocionController(_mockLogger.Object, _mockAnalisisLogica.Object, _mockWebHostEnvironment.Object);
        _httpContext.Session = new Mock<ISession>().Object;
        controller.ControllerContext = new ControllerContext { HttpContext = _httpContext };

        var tempData = new TempDataDictionary(_httpContext, Mock.Of<ITempDataProvider>());
        controller.TempData = tempData;

        var mockFile = CreateMockFormFile("test.jpg", "image/jpeg", [1, 2, 3]);

        var result = await controller.Analizar(mockFile.Object);

        var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Login", redirectResult.ActionName);
        Assert.Equal("Usuario", redirectResult.ControllerName);
        Assert.Equal("Tu sesión ha expirado. Por favor, inicia sesión.", controller.TempData["Error"]);
    }

    [Fact]
    public async Task Analizar_Post_SinCarasDetectadas_RetornaVistaMensaje()
    {
        _mockWebHostEnvironment.Setup(x => x.WebRootPath).Returns("c:\\wwwroot");
        _mockAnalisisLogica.Setup(x => x.DetectFaces(It.IsAny<byte[]>())).Returns(new List<Rectangle>());

        var controller = new EmocionController(_mockLogger.Object, _mockAnalisisLogica.Object, _mockWebHostEnvironment.Object);
        var httpContext = new DefaultHttpContext();
        httpContext.Session = CreateMockSession(1);
        controller.ControllerContext = new ControllerContext { HttpContext = httpContext };

        var tempData = new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>());
        controller.TempData = tempData;

        var mockFile = CreateMockFormFile("test.jpg", "image/jpeg", new byte[] { 1, 2, 3 });

        var result = await controller.Analizar(mockFile.Object);

        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Equal("Analizar", viewResult.ViewName);
        Assert.Equal("No se detectaron caras en la imagen. Intenta con otra imagen.", controller.TempData["Message"]);
    }

    [Fact]
    public async Task Analizar_Post_SinResultados_RetornaVistaMensaje()
    {
        _mockWebHostEnvironment.Setup(x => x.WebRootPath).Returns("c:\\wwwroot");
        _mockAnalisisLogica.Setup(x => x.DetectFaces(It.IsAny<byte[]>())).Returns(new List<Rectangle> { new Rectangle(0, 0, 100, 100) });
        _mockAnalisisLogica.Setup(x => x.ProcessFacesAsync(It.IsAny<List<Rectangle>>(), It.IsAny<byte[]>(), It.IsAny<float>(), It.IsAny<List<EmocionResult>>()))
            .Returns(Task.CompletedTask);

        var controller = new EmocionController(_mockLogger.Object, _mockAnalisisLogica.Object, _mockWebHostEnvironment.Object);
        var httpContext = new DefaultHttpContext();
        httpContext.Session = CreateMockSession(1);
        controller.ControllerContext = new ControllerContext { HttpContext = httpContext };

        var tempData = new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>());
        controller.TempData = tempData;

        var mockFile = CreateMockFormFile("test.jpg", "image/jpeg", new byte[] { 1, 2, 3 });

        var result = await controller.Analizar(mockFile.Object);

        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Equal("Analizar", viewResult.ViewName);
        Assert.Equal("No se obtuvieron predicciones del modelo. Revisa los logs.", controller.TempData["Message"]);
    }

    [Fact]
    public async Task Analizar_Post_EmocionDesconocida_RetornaVistaError()
    {
        _mockWebHostEnvironment.Setup(x => x.WebRootPath).Returns("c:\\wwwroot");
        _mockAnalisisLogica.Setup(x => x.DetectFaces(It.IsAny<byte[]>())).Returns(new List<Rectangle> { new Rectangle(0, 0, 100, 100) });
        _mockAnalisisLogica.Setup(x => x.ProcessFacesAsync(It.IsAny<List<Rectangle>>(), It.IsAny<byte[]>(), It.IsAny<float>(), It.IsAny<List<EmocionResult>>()))
            .Callback<List<Rectangle>, byte[], float, List<EmocionResult>>((faces, bytes, threshold, results) =>
            {
                results.Add(new EmocionResult { Emocion = "Desconocida", Confidence = 0.9f, IsRecognized = false });
            })
            .Returns(Task.CompletedTask);

        var controller = new EmocionController(_mockLogger.Object, _mockAnalisisLogica.Object, _mockWebHostEnvironment.Object);
        var httpContext = new DefaultHttpContext();
        httpContext.Session = CreateMockSession(1);
        controller.ControllerContext = new ControllerContext { HttpContext = httpContext };

        var tempData = new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>());
        controller.TempData = tempData;

        var mockFile = CreateMockFormFile("test.jpg", "image/jpeg", new byte[] { 1, 2, 3 });

        var result = await controller.Analizar(mockFile.Object);

        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Equal("Analizar", viewResult.ViewName);
        Assert.Contains("El modelo de IA no pudo detectar la emoción", controller.TempData["Error"]?.ToString());
    }

    [Fact]
    public async Task Analizar_Post_EmocionValida_GuardaYRetornaResultado()
    {
        _mockWebHostEnvironment.Setup(x => x.WebRootPath).Returns("c:\\wwwroot");
        _mockAnalisisLogica.Setup(x => x.DetectFaces(It.IsAny<byte[]>())).Returns(new List<Rectangle> { new Rectangle(0, 0, 100, 100) });
        _mockAnalisisLogica.Setup(x => x.ProcessFacesAsync(It.IsAny<List<Rectangle>>(), It.IsAny<byte[]>(), It.IsAny<float>(), It.IsAny<List<EmocionResult>>()))
            .Callback<List<Rectangle>, byte[], float, List<EmocionResult>>((faces, bytes, threshold, results) =>
            {
                results.Add(new EmocionResult { Emocion = "Happy", Confidence = 0.95f, IsRecognized = true });
            })
            .Returns(Task.CompletedTask);
        
        var analisisResultado = new AnalisisResultado { Id = 1, EmocionId = 1, UsuarioId = 1 };
        _mockAnalisisLogica.Setup(x => x.GuardarAnalisisAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<float>()))
            .ReturnsAsync(analisisResultado);

        var controller = new EmocionController(_mockLogger.Object, _mockAnalisisLogica.Object, _mockWebHostEnvironment.Object);
        _httpContext.Session = CreateMockSession(1);
        controller.ControllerContext = new ControllerContext { HttpContext = _httpContext };

        var mockFile = CreateMockFormFile("test.jpg", "image/jpeg", [1, 2, 3]);

        var result = await controller.Analizar(mockFile.Object);

        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Equal("Resultado", viewResult.ViewName);
        Assert.NotNull(controller.ViewBag.Emocion);
        Assert.NotNull(controller.ViewBag.ImagenRuta);
        Assert.Equal(0.95f, controller.ViewBag.Confidence);
        
        _mockAnalisisLogica.Verify(
    x => x.GuardarAnalisisAsync("Happy", It.IsAny<int>(), It.IsAny<string>(), 0.95f), Times.Once);
    }

    [Fact]
    public async Task Analizar_Post_ExcepcionGeneral_RetornaVistaError()
    {
        _mockWebHostEnvironment.Setup(x => x.WebRootPath).Returns("c:\\wwwroot");
        _mockAnalisisLogica.Setup(x => x.DetectFaces(It.IsAny<byte[]>())).Throws(new Exception("Error de prueba"));

        var controller = new EmocionController(_mockLogger.Object, _mockAnalisisLogica.Object, _mockWebHostEnvironment.Object);
        var httpContext = new DefaultHttpContext();
        httpContext.Session = CreateMockSession(1);
        controller.ControllerContext = new ControllerContext { HttpContext = httpContext };

        var tempData = new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>());
        controller.TempData = tempData;

        var mockFile = CreateMockFormFile("test.jpg", "image/jpeg", new byte[] { 1, 2, 3 });

        var result = await controller.Analizar(mockFile.Object);

        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Equal("Analizar", viewResult.ViewName);
        Assert.Contains("Error de prueba", controller.TempData["Error"]?.ToString());
    }

    private Mock<IFormFile> CreateMockFormFile(string fileName, string contentType, byte[] content)
    {
        var mockFile = new Mock<IFormFile>();
        var stream = new MemoryStream(content);
        mockFile.Setup(f => f.FileName).Returns(fileName);
        mockFile.Setup(f => f.ContentType).Returns(contentType);
        mockFile.Setup(f => f.Length).Returns(content.Length);
        mockFile.Setup(f => f.OpenReadStream()).Returns(stream);
        mockFile.Setup(f => f.CopyToAsync(It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
            .Returns((Stream target, CancellationToken token) =>
            {
                stream.Position = 0;
                return stream.CopyToAsync(target, token);
            });
        return mockFile;
    }

private ISession CreateMockSession(int usuarioId)
{
    var mockSession = new Mock<ISession>();
    byte[] userIdBytes = BitConverter.GetBytes(usuarioId);

#pragma warning disable CS8601
        mockSession.Setup(s => s.TryGetValue("UsuarioId", out It.Ref<byte[]>.IsAny))
        .Callback(new TryGetValueCallback((string key, out byte[] value) =>
        {
            value = userIdBytes;
        }))
        .Returns(true);
#pragma warning restore CS8601

        return mockSession.Object;
}

    private delegate void TryGetValueCallback(string key, out byte[] value);
}