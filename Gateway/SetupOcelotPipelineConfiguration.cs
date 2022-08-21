using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gateway;

internal class SetupOcelotPipelineConfiguration
{
    public static async Task AddEmailToHeaderMiddleware(HttpContext httpContext, Func<Task> next)
    {
        if (httpContext.Response.StatusCode != 401 && httpContext.Request.Headers.TryGetValue("Authorization", out var token))
        {
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(token.First().Replace("Bearer ", ""));
            var tokenS = jsonToken as JwtSecurityToken;
            var email = tokenS?.Payload.GetValueOrDefault("https://example.com/email") as string;
            httpContext.Request.Headers.Add("User_Email", new StringValues(email));
        }
        await next.Invoke();
    }
}
