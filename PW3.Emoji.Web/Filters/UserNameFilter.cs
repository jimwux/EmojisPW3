using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using PW3.Emoji.Logica;

namespace PW3.Emoji.Web.Filters;

public class UserNameFilter : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var httpContext = context.HttpContext;

        if (context.Controller is Controller ctrl)
        {
            ctrl.ViewBag.NombreUsuario = httpContext.Session.GetString("UsuarioNombre");
        }

        await next();
    }
}
