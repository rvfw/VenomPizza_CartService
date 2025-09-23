using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Linq.Expressions;

namespace VenomPizzaCartService.src.attribute;

[AttributeUsage(AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
public class ValidateUserId : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        if (!context.HttpContext.Request.Headers.TryGetValue("Id", out var userIdString))
        {
            context.Result = new BadRequestObjectResult(new
            {
                Error = "Необходимо Id пользователя",
                Details = "Необходима авторизация через заголовок 'Id', содержащий Id пользователя"
            });
            return;
        }
        if (!int.TryParse(userIdString, out var userId))
        {
            context.Result = new BadRequestObjectResult(new
            {
                Error = "Id пользователя должно быть числом",
                Details = $"{userIdString} - некорректный Id"
            });
            return;
        }
        if (userId < 0)
        {
            context.Result = new BadRequestObjectResult(new
            {
                Error = "Id пользователя не может быть отрицательным"
            });
            return;
        }
        context.HttpContext.Items["Id"] = userId;
        base.OnActionExecuting(context);
    }
}
