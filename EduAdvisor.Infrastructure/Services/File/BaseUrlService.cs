using EduAdvisor.Application.Interfaces.File;
using Microsoft.AspNetCore.Http;

namespace EduAdvisor.Infrastructure.Services.File;

public class BaseUrlService(IHttpContextAccessor httpContextAccessor) : IBaseUrlService
{
    public string GetBaseUrl()
    {
        var request = httpContextAccessor.HttpContext?.Request;
        if (request == null) return string.Empty;
        return $"{request.Scheme}://{request.Host.Value}";
    }

    public string ToAbsoluteMediaUrl(string? path)
    {
        if (string.IsNullOrWhiteSpace(path)) return path ?? string.Empty;
        var p = path.Trim();
        if (p.StartsWith("http://", StringComparison.OrdinalIgnoreCase) ||
            p.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
            return p;
        var baseUrl = GetBaseUrl().TrimEnd('/');
        var rel = p.StartsWith('/') ? p : "/" + p;
        return string.IsNullOrEmpty(baseUrl) ? rel : $"{baseUrl}{rel}";
    }
}