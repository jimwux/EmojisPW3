using System.Net;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using PW3.Emoji.Entidades.EF;
using PW3.Emoji.Logica;
using PW3.Emoji.Web.Filters;

namespace PW3.Emoji.Test.Web.Integration;

public class UsuarioControllerIntegrationTest : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public UsuarioControllerIntegrationTest(WebApplicationFactory<Program> factory)
    {
        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.UseEnvironment("Testing");

            builder.ConfigureServices(services =>
            {
                var filtersToRemove = services
                    .Where(d => d.ServiceType == typeof(UserNameFilter))
                    .ToList();

                foreach (var filter in filtersToRemove)
                    services.Remove(filter);

                services.AddControllersWithViews(options =>
                {
                    options.Filters.Clear();
                });

                services.AddAntiforgery(options =>
                {
                    options.Cookie.Name = "test-antiforgery";
                    options.HeaderName = "test-antiforgery";
                });

                var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(IUsuarioLogica));
                if (descriptor != null)
                    services.Remove(descriptor);

                var mockUsuarioLogica = new Mock<IUsuarioLogica>();
                mockUsuarioLogica.Setup(x => x.CrearUsuario(It.IsAny<Usuario>()));

                services.AddScoped(_ => mockUsuarioLogica.Object);
            });
        });
    }

    [Fact]
    public async Task Registro_Get_RetornaView()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/Usuario/Registro");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var content = await response.Content.ReadAsStringAsync();
        Assert.Contains("Registro", content, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task Login_Get_RetornaView()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/Usuario/Login");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var content = await response.Content.ReadAsStringAsync();
        Assert.Contains("Login", content, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task Registro_Post_UsuarioValido_RedireccionaALogin()
    {
        // Arrange
        var client = _factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false,
            HandleCookies = true
        });

        var getResponse = await client.GetAsync("/Usuario/Registro");
        var getContent = await getResponse.Content.ReadAsStringAsync();

        var tokenMatch = System.Text.RegularExpressions.Regex.Match(
            getContent,
            @"<input name=""__RequestVerificationToken"" type=""hidden"" value=""([^""]+)""");

        var token = tokenMatch.Success ? tokenMatch.Groups[1].Value : string.Empty;

        var form = new FormUrlEncodedContent(new[]
        {
        new KeyValuePair<string, string>("__RequestVerificationToken", token),
        new KeyValuePair<string, string>("Nombre", "TestNombre"),
        new KeyValuePair<string, string>("Email", "testint@mail.com"),
        new KeyValuePair<string, string>("HashPassword", "Password123!"),
        new KeyValuePair<string, string>("RolId", "1"),
    });

        // Act
        var response = await client.PostAsync("/Usuario/Registro", form);
        var body = await response.Content.ReadAsStringAsync();

        // Debug
        if (response.StatusCode != HttpStatusCode.Redirect && response.StatusCode != HttpStatusCode.Found)
        {
            Console.WriteLine("STATUS: " + response.StatusCode);
            Console.WriteLine("BODY: " + body);
        }

        // Assert
        Assert.True(
            response.StatusCode == HttpStatusCode.Redirect || response.StatusCode == HttpStatusCode.Found,
            $"Expected redirect but got {response.StatusCode}. Body: {body.Substring(0, Math.Min(500, body.Length))}"
        );
        Assert.Contains("/Usuario/Login", response.Headers.Location?.ToString());
    }
}
