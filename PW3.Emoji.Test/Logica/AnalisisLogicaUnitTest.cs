using Xunit;
using Moq;
using Microsoft.EntityFrameworkCore;
using PW3.Emoji.Logica;
using PW3.Emoji.Entidades.EF;

namespace PW3.Emoji.Test.Logica;

public class AnalisisLogicaUnitTest
{
    private Mock<DbSet<AnalisisResultado>> GetMockAnalisisResultados(List<AnalisisResultado> data)
    {
        var queryable = data.AsQueryable();
        var mockSet = new Mock<DbSet<AnalisisResultado>>();
        mockSet.As<IQueryable<AnalisisResultado>>().Setup(m => m.Provider).Returns(queryable.Provider);
        mockSet.As<IQueryable<AnalisisResultado>>().Setup(m => m.Expression).Returns(queryable.Expression);
        mockSet.As<IQueryable<AnalisisResultado>>().Setup(m => m.ElementType).Returns(queryable.ElementType);
        mockSet.As<IQueryable<AnalisisResultado>>().Setup(m => m.GetEnumerator()).Returns(queryable.GetEnumerator());
        return mockSet;
    }

    private Mock<DbSet<Emocion>> GetMockEmociones(List<Emocion> data)
    {
        var queryable = data.AsQueryable();
        var mockSet = new Mock<DbSet<Emocion>>();
        mockSet.As<IQueryable<Emocion>>().Setup(m => m.Provider).Returns(queryable.Provider);
        mockSet.As<IQueryable<Emocion>>().Setup(m => m.Expression).Returns(queryable.Expression);
        mockSet.As<IQueryable<Emocion>>().Setup(m => m.ElementType).Returns(queryable.ElementType);
        mockSet.As<IQueryable<Emocion>>().Setup(m => m.GetEnumerator()).Returns(queryable.GetEnumerator());
        return mockSet;
    }

    private Mock<DbSet<Usuario>> GetMockUsuarios(List<Usuario> data)
    {
        var queryable = data.AsQueryable();
        var mockSet = new Mock<DbSet<Usuario>>();
        mockSet.As<IQueryable<Usuario>>().Setup(m => m.Provider).Returns(queryable.Provider);
        mockSet.As<IQueryable<Usuario>>().Setup(m => m.Expression).Returns(queryable.Expression);
        mockSet.As<IQueryable<Usuario>>().Setup(m => m.ElementType).Returns(queryable.ElementType);
        mockSet.As<IQueryable<Usuario>>().Setup(m => m.GetEnumerator()).Returns(queryable.GetEnumerator());
        return mockSet;
    }

    [Fact]
    public void ObtenerAnalisis_FiltraPorEmocion()
    {
        var emocion = new Emocion { Id = 1, Nombre = "Feliz" };
        var usuario = new Usuario { Id = 1, Email = "a@a.com" };
        var resultados = new List<AnalisisResultado>
        {
            new AnalisisResultado { EmocionId = 1, Emocion = emocion, Usuario = usuario, UsuarioId = 1, FechaAnalisis = new DateTime(2024,1,1) },
            new AnalisisResultado { EmocionId = 2, Emocion = new Emocion { Id = 2 }, Usuario = usuario, UsuarioId = 1, FechaAnalisis = new DateTime(2024,1,2) }
        };
        var mockSet = GetMockAnalisisResultados(resultados);
        var mockContext = new Mock<PW3_EmojiContext>();
        mockContext.Setup(c => c.AnalisisResultados).Returns(mockSet.Object);

        var service = new AnalisisLogica(mockContext.Object);

        var res = service.ObtenerAnalisis(1, 1, null, null, 1);

        Assert.Single(res);
        Assert.Equal(1, res[0].EmocionId);
    }

    [Fact]
    public void ObtenerAnalisis_FiltraPorFecha()
    {
        var usuario = new Usuario { Id = 1 };
        var resultados = new List<AnalisisResultado>
        {
            new AnalisisResultado { EmocionId = 1, Usuario = usuario, UsuarioId = 1, FechaAnalisis = new DateTime(2024,1,1) },
            new AnalisisResultado { EmocionId = 1, Usuario = usuario, UsuarioId = 1, FechaAnalisis = new DateTime(2024,2,1) }
        };
        var mockSet = GetMockAnalisisResultados(resultados);
        var mockContext = new Mock<PW3_EmojiContext>();
        mockContext.Setup(c => c.AnalisisResultados).Returns(mockSet.Object);

        var service = new AnalisisLogica(mockContext.Object);

        var res = service.ObtenerAnalisis(1, null, new DateTime(2024,2,1), null, 1);

        Assert.Single(res);
        Assert.Equal(new DateTime(2024,2,1), res[0].FechaAnalisis);
    }

    [Fact]
    public void ObtenerAnalisis_FiltraPorUsuarioFilter()
    {
        var usuario1 = new Usuario { Id = 1 };
        var usuario2 = new Usuario { Id = 2 };
        var resultados = new List<AnalisisResultado>
        {
            new AnalisisResultado { EmocionId = 1, Usuario = usuario1, UsuarioId = 1, FechaAnalisis = DateTime.Now },
            new AnalisisResultado { EmocionId = 1, Usuario = usuario2, UsuarioId = 2, FechaAnalisis = DateTime.Now }
        };
        var mockSet = GetMockAnalisisResultados(resultados);
        var mockContext = new Mock<PW3_EmojiContext>();
        mockContext.Setup(c => c.AnalisisResultados).Returns(mockSet.Object);

        var service = new AnalisisLogica(mockContext.Object);

        var res = service.ObtenerAnalisis(null, null, null, null, 1, usuarioFilter: 2, esAdmin: true);

        Assert.Single(res);
        Assert.Equal(2, res[0].UsuarioId);
    }

    [Fact]
    public void ObtenerAnalisis_AdminVeTodos()
    {
        var usuario1 = new Usuario { Id = 1 };
        var usuario2 = new Usuario { Id = 2 };
        var resultados = new List<AnalisisResultado>
        {
            new AnalisisResultado { EmocionId = 1, Usuario = usuario1, UsuarioId = 1, FechaAnalisis = DateTime.Now },
            new AnalisisResultado { EmocionId = 1, Usuario = usuario2, UsuarioId = 2, FechaAnalisis = DateTime.Now }
        };
        var mockSet = GetMockAnalisisResultados(resultados);
        var mockContext = new Mock<PW3_EmojiContext>();
        mockContext.Setup(c => c.AnalisisResultados).Returns(mockSet.Object);

        var service = new AnalisisLogica(mockContext.Object);

        var res = service.ObtenerAnalisis(null, null, null, null, 1, esAdmin: true);

        Assert.Equal(2, res.Count);
    }

    [Fact]
    public void ObtenerAnalisis_Paginacion()
    {
        var usuario = new Usuario { Id = 1 };
        var resultados = new List<AnalisisResultado>();
        for (int i = 0; i < 5; i++)
        {
            resultados.Add(new AnalisisResultado { EmocionId = 1, Usuario = usuario, UsuarioId = 1, FechaAnalisis = DateTime.Now.AddDays(-i) });
        }
        var mockSet = GetMockAnalisisResultados(resultados);
        var mockContext = new Mock<PW3_EmojiContext>();
        mockContext.Setup(c => c.AnalisisResultados).Returns(mockSet.Object);

        var service = new AnalisisLogica(mockContext.Object);

        var res = service.ObtenerAnalisis(1, null, null, null, 1);
        Assert.Equal(4, res.Count);

        var res2 = service.ObtenerAnalisis(1, null, null, null, 2);
        Assert.Single(res2);
    }

    [Fact]
    public void ObtenerEmociones_DevuelveTodas()
    {
        var emociones = new List<Emocion>
        {
            new Emocion { Id = 1, Nombre = "Feliz" },
            new Emocion { Id = 2, Nombre = "Triste" }
        };
        var mockSet = GetMockEmociones(emociones);
        var mockContext = new Mock<PW3_EmojiContext>();
        mockContext.Setup(c => c.Emocion).Returns(mockSet.Object);

        var service = new AnalisisLogica(mockContext.Object);

        var res = service.ObtenerEmociones();
        Assert.Equal(2, res.Count);
        Assert.Contains(res, e => e.Nombre == "Feliz");
    }

    [Fact]
    public void ObtenerUsuarios_DevuelveTodos()
    {
        var usuarios = new List<Usuario>
        {
            new Usuario { Id = 1, Email = "a@a.com" },
            new Usuario { Id = 2, Email = "b@b.com" }
        };
        var mockSet = GetMockUsuarios(usuarios);
        var mockContext = new Mock<PW3_EmojiContext>();
        mockContext.Setup(c => c.Usuario).Returns(mockSet.Object);

        var service = new AnalisisLogica(mockContext.Object);

        var res = service.ObtenerUsuarios();
        Assert.Equal(2, res.Count);
        Assert.Contains(res, u => u.Email == "a@a.com");
    }
}