using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace E_Doctor.Infrastructure.Identity.Services;
public class TokenService
{
    private readonly UserManager<AppUserIdentity> _userManager;
    private readonly IConfiguration _config;
   
    public TokenService(UserManager<AppUserIdentity> userManager, IConfiguration config)
    {
        _userManager = userManager;
        _config = config;
    }

    public async Task<JwtSecurityToken> CreateTokenAsync(AppUserIdentity user)
    {
        var key = _config["Jwt:SecretKey"];

        if (string.IsNullOrEmpty(key))
        {
            return new JwtSecurityToken();
        }

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
        var userRoles = await _userManager.GetRolesAsync(user);

        user.Email ??= "";
        user.UserName ??= "";

        var authClaims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new(ClaimTypes.Name, user.UserName.ToString()[..1].ToUpper()),
                new(ClaimTypes.Email, user.Email)
            };
        foreach (var userRole in userRoles)
        {
            authClaims.Add(new Claim(ClaimTypes.Role, userRole));
        }
        var token = new JwtSecurityToken(
           issuer: _config["Jwt:ValidIssuer"],
           audience: _config["Jwt:ValidAudience"],
           expires: DateTime.Now.AddDays(30),
           claims: authClaims,
           signingCredentials: credentials);

        return token;
    }

}
