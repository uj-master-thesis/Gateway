using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gateway;

public class HostInjectorDelegatingHandler : DelegatingHandler
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public HostInjectorDelegatingHandler(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        if (request.Headers.TryGetValues("Authorization", out var token))
        {
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(token.First().Replace("Bearer ", ""));
            var tokenS = jsonToken as JwtSecurityToken;
            var email = tokenS?.Payload.GetValueOrDefault("https://example.com/email") as string;
            request.Headers.Add("User-Email", email);
        }
        return base.SendAsync(request, cancellationToken);
    }
}