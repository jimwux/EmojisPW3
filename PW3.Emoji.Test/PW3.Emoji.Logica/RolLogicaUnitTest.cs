using PW3.Emoji.Logica;
using PW3.Emoji.Entidades.EF;
using Moq;

namespace PW3.Emoji.Test.PW3.Emoji.Logica;

public class RolLogicaUnitTest
{
    [Fact]
    public void ObtenerRoles_ReturnsAllRoles()
    {
        // Arrange
        var roles = new List<Rol>
            {
                new Rol { Id = 1, Nombre = "Admin" },
                new Rol { Id = 2, Nombre = "User" }
            }.AsQueryable();

        var mockSet = new Mock<Microsoft.EntityFrameworkCore.DbSet<Rol>>();
        mockSet.As<IQueryable<Rol>>().Setup(m => m.Provider).Returns(roles.Provider);
        mockSet.As<IQueryable<Rol>>().Setup(m => m.Expression).Returns(roles.Expression);
        mockSet.As<IQueryable<Rol>>().Setup(m => m.ElementType).Returns(roles.ElementType);
        mockSet.As<IQueryable<Rol>>().Setup(m => m.GetEnumerator()).Returns(roles.GetEnumerator());

        var mockContext = new Mock<PW3_EmojiContext>();
        mockContext.Setup(c => c.Rol).Returns(mockSet.Object);

        var service = new RolLogica(mockContext.Object);

        // Act
        var result = service.ObtenerRoles();

        // Assert
        Assert.Equal(2, result.Count);
        Assert.Contains(result, r => r.Nombre == "Admin");
        Assert.Contains(result, r => r.Nombre == "User");
    }

    [Fact]
    public void ObtenerRolPorNombre_ReturnsCorrectRol()
    {
        // Arrange
        var roles = new List<Rol>
            {
                new Rol { Id = 1, Nombre = "Admin" },
                new Rol { Id = 2, Nombre = "User" }
            }.AsQueryable();

        var mockSet = new Mock<Microsoft.EntityFrameworkCore.DbSet<Rol>>();
        mockSet.As<IQueryable<Rol>>().Setup(m => m.Provider).Returns(roles.Provider);
        mockSet.As<IQueryable<Rol>>().Setup(m => m.Expression).Returns(roles.Expression);
        mockSet.As<IQueryable<Rol>>().Setup(m => m.ElementType).Returns(roles.ElementType);
        mockSet.As<IQueryable<Rol>>().Setup(m => m.GetEnumerator()).Returns(roles.GetEnumerator());

        var mockContext = new Mock<PW3_EmojiContext>();
        mockContext.Setup(c => c.Rol).Returns(mockSet.Object);

        var service = new RolLogica(mockContext.Object);

        // Act
        var result = service.ObtenerRolPorNombre("Admin");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Admin", result!.Nombre);
    }

    [Fact]
    public void ObtenerRolPorNombre_ReturnsNullIfNotFound()
    {
        // Arrange
        var roles = new List<Rol>
            {
                new Rol { Id = 1, Nombre = "Admin" }
            }.AsQueryable();

        var mockSet = new Mock<Microsoft.EntityFrameworkCore.DbSet<Rol>>();
        mockSet.As<IQueryable<Rol>>().Setup(m => m.Provider).Returns(roles.Provider);
        mockSet.As<IQueryable<Rol>>().Setup(m => m.Expression).Returns(roles.Expression);
        mockSet.As<IQueryable<Rol>>().Setup(m => m.ElementType).Returns(roles.ElementType);
        mockSet.As<IQueryable<Rol>>().Setup(m => m.GetEnumerator()).Returns(roles.GetEnumerator());

        var mockContext = new Mock<PW3_EmojiContext>();
        mockContext.Setup(c => c.Rol).Returns(mockSet.Object);

        var service = new RolLogica(mockContext.Object);

        // Act
        var result = service.ObtenerRolPorNombre("User");

        // Assert
        Assert.Null(result);
    }
}
