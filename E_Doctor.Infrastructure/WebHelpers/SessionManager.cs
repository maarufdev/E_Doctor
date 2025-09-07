using Microsoft.AspNetCore.Http;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace E_Doctor.Infrastructure.WebHelpers
{
    public static class SessionManager
    {
        public static void SetTokenCookie(HttpContext httpContext, string token)
        {
            ArgumentNullException.ThrowIfNull(httpContext, nameof(httpContext));

            httpContext.Response.Cookies.Append("token", token, new CookieOptions { Expires = DateTime.Now.AddDays(30) });
        }

        public static void SetCurrentIdCookie(HttpContext httpContext, string role)
        {
            httpContext?.Response.Cookies.Append("id", role, new CookieOptions { Expires = DateTime.Now.AddDays(30) });
        }
        public static void SetCurrentRoleCookie(HttpContext httpContext, string role)
        {
            httpContext?.Response.Cookies.Append("role", role, new CookieOptions { Expires = DateTime.Now.AddDays(30) });
        }
        public static void SetCurrentNameCookie(HttpContext httpContext, string role)
        {
            httpContext?.Response.Cookies.Append("name", role, new CookieOptions { Expires = DateTime.Now.AddDays(30) });
        }
        public static string? GetToken(HttpContext httpContext)
        {
            var token = String.Empty;
            if (httpContext is not null)
            {
                token = httpContext.Request.Cookies["token"];
            }
            return token;
        }
        public static string? GetCurrentId(HttpContext httpContext)
        {
            var role = String.Empty;
            if (httpContext is not null)
            {
                role = httpContext?.Request.Cookies["id"];
            }
            return role;
        }
        public static string? GetCurrentRole(HttpContext httpContext)
        {
            var role = String.Empty;
            if (httpContext is not null)
            {
                role = httpContext?.Request.Cookies["role"];
            }
            return role;
        }
        public static string? GetCurrentRole(IHttpContextAccessor httpContext)
        {
            var role = String.Empty;
            if (httpContext is not null)
            {
                role = httpContext.HttpContext?.Request.Cookies["role"];
            }
            return role;
        }
        public static string? GetCurrentName(IHttpContextAccessor httpContext)
        {
            var name = String.Empty;
            if (httpContext is not null)
            {
                name = httpContext.HttpContext?.Request.Cookies["name"];
            }
            return name;
        }
        public static void ClearAllCookies(IHttpContextAccessor httpContext)
        {
            if (httpContext.HttpContext is not null)
            {
                foreach (var cookie in httpContext.HttpContext.Request.Cookies.Keys)
                {
                    httpContext.HttpContext.Response.Cookies.Delete(cookie);
                }
            }
        }
        public static string SetupCookie(IHttpContextAccessor httpContext, string token, string username)
        {
            var jwt_token = new JwtSecurityTokenHandler().ReadJwtToken(token);
            var roleClaimValue = string.Empty;
            var landingPage = "/Login/Index";

            if (jwt_token is not null)
            {
                var claims = jwt_token.Claims;
                var idClaim = claims.Where(c => c.Type == ClaimTypes.NameIdentifier).FirstOrDefault();
                var roleClaim = claims.Where(c => c.Type == ClaimTypes.Role).FirstOrDefault();

                if (httpContext.HttpContext is not null)
                {
                    SetTokenCookie(httpContext.HttpContext, token);
                    if (idClaim != null)
                    {
                        SetCurrentIdCookie(httpContext.HttpContext, idClaim.Value);
                    }

                    if (roleClaim != null)
                    {
                        SetCurrentRoleCookie(httpContext.HttpContext, roleClaim.Value);
                        roleClaimValue = roleClaim.Value;
                    }

                    if (username != null)
                    {
                        SetCurrentNameCookie(httpContext.HttpContext, username);
                    }
                }
            }

            //if (roleClaimValue == UserRoles.Admin)
            //{
            //    landingPage = "/SystemAdminDashboard/Index";
            //}
            //else if (roleClaimValue == UserRoles.Agent)
            //{
            //    landingPage = "/AgentDashboard/Index";
            //}
            //else if (roleClaimValue == UserRoles.Buyer)
            //{
            //    landingPage = "/BuyerDashboard/Index";
            //}
            //else if (roleClaimValue == UserRoles.Collection)
            //{
            //    landingPage = "/CollectionDashboard/Index";
            //}

            return landingPage;
        }
    }
}
