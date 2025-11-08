using PW3.Emoji.Logica;
using PW3.Emoji.Entidades.EF;
using Moq;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace PW3.Emoji.Test.PW3.Emoji.Logica;

public class UsuarioLogicaUnitTest
{
    [Fact]
    public void CrearUsuario_AgregaUsuarioYGuardaCambios()
    {
        // Arrange
        var usuario = new Usuario { Email = "test@mail.com", HashPassword = "1234" };
        var rol = new Rol { Id = 1, Nombre = "USUARIO" };

        var mockSet = new Mock<DbSet<Usuario>>();
        var mockContext = new Mock<PW3_EmojiContext>();
        mockContext.Setup(c => c.Usuario).Returns(mockSet.Object);

        var mockRolLogica = new Mock<IRolLogica>();
        mockRolLogica.Setup(r => r.ObtenerRolPorNombre("USUARIO")).Returns(rol);

        var service = new UsuarioLogica(mockContext.Object, mockRolLogica.Object);

        // Act
        service.CrearUsuario(usuario);

        // Assert
        mockSet.Verify(m => m.Add(It.Is<Usuario>(u => u.Email == "test@mail.com")), Times.Once);
        mockContext.Verify(m => m.SaveChanges(), Times.Once);
        Assert.Equal(rol, usuario.Rol);
        Assert.NotEqual("1234", usuario.HashPassword);
    }

    [Fact]
    public void Login_UsuarioCorrecto_DevuelveUsuario()
    {
        // Arrange
        var password = "pass";
        var usuario = new Usuario { Email = "test@mail.com" };
        usuario.HashPassword = new PasswordHasher<Usuario>().HashPassword(usuario, password);

        var usuarios = new List<Usuario> { usuario }.AsQueryable();

        var mockSet = new Mock<DbSet<Usuario>>();
        mockSet.As<IQueryable<Usuario>>().Setup(m => m.Provider).Returns(usuarios.Provider);
        mockSet.As<IQueryable<Usuario>>().Setup(m => m.Expression).Returns(usuarios.Expression);
        mockSet.As<IQueryable<Usuario>>().Setup(m => m.ElementType).Returns(usuarios.ElementType);
        mockSet.As<IQueryable<Usuario>>().Setup(m => m.GetEnumerator()).Returns(usuarios.GetEnumerator());

        var mockContext = new Mock<PW3_EmojiContext>();
        mockContext.Setup(c => c.Usuario).Returns(mockSet.Object);

        var mockRolLogica = new Mock<IRolLogica>();
        var service = new UsuarioLogica(mockContext.Object, mockRolLogica.Object);

        // Act
        var result = service.Login("test@mail.com", password);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("test@mail.com", result!.Email);
    }

    [Fact]
    public void Login_UsuarioIncorrecto_DevuelveNull()
    {
        // Arrange
        var usuario = new Usuario { Email = "test@mail.com" };
        usuario.HashPassword = new PasswordHasher<Usuario>().HashPassword(usuario, "pass");

        var usuarios = new List<Usuario> { usuario }.AsQueryable();

        var mockSet = new Mock<DbSet<Usuario>>();
        mockSet.As<IQueryable<Usuario>>().Setup(m => m.Provider).Returns(usuarios.Provider);
        mockSet.As<IQueryable<Usuario>>().Setup(m => m.Expression).Returns(usuarios.Expression);
        mockSet.As<IQueryable<Usuario>>().Setup(m => m.ElementType).Returns(usuarios.ElementType);
        mockSet.As<IQueryable<Usuario>>().Setup(m => m.GetEnumerator()).Returns(usuarios.GetEnumerator());

        var mockContext = new Mock<PW3_EmojiContext>();
        mockContext.Setup(c => c.Usuario).Returns(mockSet.Object);

        var mockRolLogica = new Mock<IRolLogica>();
        var service = new UsuarioLogica(mockContext.Object, mockRolLogica.Object);

        // Act
        var result = service.Login("test@mail.com", "wrongpass");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void ObtenerUsuarioPorId_DevuelveUsuarioCorrecto()
    {
        // Arrange
        var usuario = new Usuario { Id = 5, Email = "test@mail.com" };
        var usuarios = new List<Usuario> { usuario }.AsQueryable();

        var mockSet = new Mock<DbSet<Usuario>>();
        mockSet.As<IQueryable<Usuario>>().Setup(m => m.Provider).Returns(usuarios.Provider);
        mockSet.As<IQueryable<Usuario>>().Setup(m => m.Expression).Returns(usuarios.Expression);
        mockSet.As<IQueryable<Usuario>>().Setup(m => m.ElementType).Returns(usuarios.ElementType);
        mockSet.As<IQueryable<Usuario>>().Setup(m => m.GetEnumerator()).Returns(usuarios.GetEnumerator());

        var mockContext = new Mock<PW3_EmojiContext>();
        mockContext.Setup(c => c.Usuario).Returns(mockSet.Object);

        var mockRolLogica = new Mock<IRolLogica>();
        var service = new UsuarioLogica(mockContext.Object, mockRolLogica.Object);

        // Act
        var result = service.ObtenerUsuarioPorId(5);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(5, result!.Id);
    }
}