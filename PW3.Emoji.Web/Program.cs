using Microsoft.EntityFrameworkCore;
using PW3.Emoji.Entidades.EF;
using PW3.Emoji.Logica;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<PW3_EmojiContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Default")));

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddScoped<IAnalisisEmocionLogica, AnalisisEmocionLogica>();
builder.Services.AddScoped<IUsuarioLogica, UsuarioLogica>();
builder.Services.AddScoped<IRolLogica, RolLogica>();
builder.Services.AddScoped<IAnalisisLogica, AnalisisLogica>();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(2);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Emocion}/{action=Analizar}/{id?}");

app.Run();
