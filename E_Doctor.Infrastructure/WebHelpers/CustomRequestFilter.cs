using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Doctor.Infrastructure.WebHelpers
{
    public class CustomRequestFilter : ActionFilterAttribute
    {
        public override void OnActionExecuted(ActionExecutedContext context)
        {

            var httpContext = context.HttpContext;
            var controllerName = context.RouteData.Values["controller"];

            var token = SessionManager.GetToken(httpContext);
            
            if (string.IsNullOrWhiteSpace(token))
            {
                if (controllerName?.ToString() == "User")
                {
                    return;
                }

                context.Result = new RedirectToActionResult("Index", "User", null);
            }
        }
    }
}
