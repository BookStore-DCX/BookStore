using System.Net.Http.Headers;

namespace BookStore.Mvc.Infrastructure;

public class JwtAuthorizationHandler : DelegatingHandler
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public JwtAuthorizationHandler(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var httpContext = _httpContextAccessor.HttpContext;
        var token = httpContext?.Session.GetString(SessionKeys.JwtToken);

        if (string.IsNullOrWhiteSpace(token))
        {
            token = httpContext?.User?.FindFirst(SessionKeys.JwtToken)?.Value;
        }

        if (!string.IsNullOrWhiteSpace(token))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        return base.SendAsync(request, cancellationToken);
    }
}
