using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace PW3.Emoji.Web.Filters;

public class RequireNotLoggedAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        var session = context.HttpContext.Session;
        if (session.TryGetValue("UsuarioId", out _))
        {
            context.Result = new RedirectToActionResult("Analizar", "Emocion", null);
        }
        base.OnActionExecuting(context);
    }
}