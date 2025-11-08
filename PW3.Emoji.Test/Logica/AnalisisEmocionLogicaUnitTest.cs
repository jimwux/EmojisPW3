using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.Serialization;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.Logging;
using Moq;
using PW3.Emoji.Entidades.EF;
using PW3.Emoji.Logica;

namespace PW3.Emoji.Test.Logica;

public class AnalisisEmocionLogicaUnitTest
{
    [Fact]
    public async Task GuardarAnalisisAsync_EmocionExistente_GuardaYRetornaResultado()
    {
        var emocion = new Emocion { Id = 3, Nombre = "Feliz" };
        var emocionesData = new List<Emocion> { emocion };
        var mockEmocionSet = CreateMockDbSet(emocionesData);

        var mockImagenSet = new Mock<DbSet<Imagen>>();
        var mockAnalisisSet = new Mock<DbSet<AnalisisResultado>>();

        var mockContext = new Mock<PW3_EmojiContext>();
        mockContext.Setup(c => c.Emocion).Returns(mockEmocionSet.Object);
        mockContext.Setup(c => c.Imagen).Returns(mockImagenSet.Object);
        mockContext.Setup(c => c.AnalisisResultados).Returns(mockAnalisisSet.Object);
        mockContext.Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var mockLogger = new Mock<ILogger<AnalisisEmocionLogica>>();
        var service = CreateService(mockContext.Object, mockLogger.Object);

        var result = await service.GuardarAnalisisAsync("FELIZ", 42, "ruta/test.jpg", 0.85f);

        Assert.NotNull(result);
        Assert.Equal(42, result.UsuarioId);
        Assert.Equal(3, result.EmocionId);
        Assert.Equal("ruta/test.jpg", result.Imagen!.Ruta);

        mockImagenSet.Verify(s => s.Add(It.IsAny<Imagen>()), Times.Once);
        mockAnalisisSet.Verify(s => s.Add(It.IsAny<AnalisisResultado>()), Times.Once);
        mockContext.Verify(s => s.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GuardarAnalisisAsync_EmocionInexistente_LanzaExcepcion()
    {
        var emocionesData = new List<Emocion>();
        var mockEmocionSet = CreateMockDbSet(emocionesData);

        var mockImagenSet = new Mock<DbSet<Imagen>>();
        var mockAnalisisSet = new Mock<DbSet<AnalisisResultado>>();

        var mockContext = new Mock<PW3_EmojiContext>();
        mockContext.Setup(c => c.Emocion).Returns(mockEmocionSet.Object);
        mockContext.Setup(c => c.Imagen).Returns(mockImagenSet.Object);
        mockContext.Setup(c => c.AnalisisResultados).Returns(mockAnalisisSet.Object);

        var mockLogger = new Mock<ILogger<AnalisisEmocionLogica>>();
        var service = CreateService(mockContext.Object, mockLogger.Object);

        await Assert.ThrowsAnyAsync<Exception>(() => service.GuardarAnalisisAsync("Desconocida", 1, "ruta", 0.5f));

        mockImagenSet.Verify(s => s.Add(It.IsAny<Imagen>()), Times.Never);
        mockAnalisisSet.Verify(s => s.Add(It.IsAny<AnalisisResultado>()), Times.Never);
        mockContext.Verify(s => s.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    private static Mock<DbSet<T>> CreateMockDbSet<T>(List<T> data) where T : class
    {
        var queryable = data.AsQueryable();
        var mockSet = new Mock<DbSet<T>>();

        mockSet.As<IAsyncEnumerable<T>>()
            .Setup(m => m.GetAsyncEnumerator(It.IsAny<CancellationToken>()))
            .Returns(new TestAsyncEnumerator<T>(data.GetEnumerator()));

        mockSet.As<IQueryable<T>>()
            .Setup(m => m.Provider)
            .Returns(new TestAsyncQueryProvider<T>(queryable.Provider));

        mockSet.As<IQueryable<T>>().Setup(m => m.Expression).Returns(queryable.Expression);
        mockSet.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(queryable.ElementType);
        mockSet.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(() => data.GetEnumerator());

        return mockSet;
    }

    private static AnalisisEmocionLogica CreateService(PW3_EmojiContext context, ILogger<AnalisisEmocionLogica> logger)
    {
#pragma warning disable SYSLIB0050
        var instance = (AnalisisEmocionLogica)FormatterServices.GetUninitializedObject(typeof(AnalisisEmocionLogica));
#pragma warning restore SYSLIB0050
        typeof(AnalisisEmocionLogica).GetField("_context", BindingFlags.Instance | BindingFlags.NonPublic)!.SetValue(instance, context);
        typeof(AnalisisEmocionLogica).GetField("_logger", BindingFlags.Instance | BindingFlags.NonPublic)!.SetValue(instance, logger);
        return instance;
    }

    private sealed class TestAsyncEnumerator<T> : IAsyncEnumerator<T>
    {
        private readonly IEnumerator<T> _inner;
        public TestAsyncEnumerator(IEnumerator<T> inner) => _inner = inner;
        public T Current => _inner.Current;
        public ValueTask DisposeAsync()
        {
            _inner.Dispose();
            return ValueTask.CompletedTask;
        }
        public ValueTask<bool> MoveNextAsync() => new(_inner.MoveNext());
    }

    private sealed class TestAsyncQueryProvider<TEntity> : IAsyncQueryProvider
    {
        private readonly IQueryProvider _inner;
        public TestAsyncQueryProvider(IQueryProvider inner) => _inner = inner;

        public IQueryable CreateQuery(Expression expression) => _inner.CreateQuery(expression);
        
        public IQueryable<TElement> CreateQuery<TElement>(Expression expression) => _inner.CreateQuery<TElement>(expression);
        
        public object? Execute(Expression expression) => _inner.Execute(expression);
        
        public TResult Execute<TResult>(Expression expression) => _inner.Execute<TResult>(expression);

        public TResult ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken = default)
        {
            var resultType = typeof(TResult).GetGenericArguments()[0];
            var executeMethod = typeof(IQueryProvider)
                .GetMethods()
                .First(m => m.Name == nameof(IQueryProvider.Execute) && m.IsGenericMethod)
                .MakeGenericMethod(resultType);

            var result = executeMethod.Invoke(_inner, [expression]);
            
            var fromResultMethod = typeof(Task)
                .GetMethod(nameof(Task.FromResult))!
                .MakeGenericMethod(resultType);
            
            return (TResult)fromResultMethod.Invoke(null, [result])!;
        }
    }
}