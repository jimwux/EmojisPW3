using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using PW3.Emoji.Logica;

namespace PW3.Emoji.Web.Filters;

public class UserNameFilter : IAsyncActionFilter
{
 private readonly IUsuarioLogica _usuarioLogica;

     public UserNameFilter(IUsuarioLogica usuarioLogica)
     {
        _usuarioLogica = usuarioLogica;
     }

     public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
     {
         try
             {
             var httpContext = context.HttpContext;
             var usuarioId = httpContext.Session.GetInt32("UsuarioId");
             if (usuarioId.HasValue && context.Controller is Controller ctrl)
             {
             var nombre = await _usuarioLogica.ObtenerNombrePorIdAsync(usuarioId.Value);
             ctrl.ViewBag.NombreUsuario = nombre ?? "Usuario";
             }
         }
         catch
         {
         // Fall back silently if anything fails
         }

        await next();
     }
}
