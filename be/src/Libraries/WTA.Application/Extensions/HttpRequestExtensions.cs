using Microsoft.AspNetCore.Http;

namespace WTA.Application.Extensions;

public static class HttpRequestExtensions
{
    public static bool IsJsonRequest(this HttpRequest request)
    {
        return request.Headers.Accept.ToString().Contains("application/json") ||
            request.Headers["X-Accept"].Contains("application/json") ||
            request.Query["accept"].ToString().Contains("application/json");
    }
}
