using Microsoft.AspNetCore.Mvc.Rendering;

namespace E_Doctor.Web.Helpers;
public static class CustomHtmlHelpers
{
    public static bool IsActiveController(this IHtmlHelper htmlHelper, string controllerName)
    {
        var routeData = htmlHelper.ViewContext.RouteData.Values;
        var currentController = routeData["controller"]?.ToString();

        return string.Equals(controllerName, currentController, StringComparison.OrdinalIgnoreCase);
    }
}
