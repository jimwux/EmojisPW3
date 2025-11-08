using Microsoft.EntityFrameworkCore;
using PW3.Emoji.Entidades.EF;
using PW3.Emoji.Logica;
using PW3.Emoji.Web.Filters;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<PW3_EmojiContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Default")));

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddScoped<IAnalisisEmocionLogica, AnalisisEmocionLogica>();
builder.Services.AddScoped<IAnalisisLogica, AnalisisLogica>();
builder.Services.AddScoped<IUsuarioLogica, UsuarioLogica>();
builder.Services.AddScoped<IRolLogica, RolLogica>();

// Session support (required to use HttpContext.Session)
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(1);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Register UserNameFilter and add globally
builder.Services.AddScoped<UserNameFilter>();
builder.Services.AddControllersWithViews(options =>
{
    options.Filters.AddService<UserNameFilter>();
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

// Enable session before authorization
app.UseSession();   

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Emocion}/{action=Analizar}/{id?}");

app.Run();
