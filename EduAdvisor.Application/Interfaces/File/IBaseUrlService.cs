namespace EduAdvisor.Application.Interfaces.File;

public interface IBaseUrlService
{
    string GetBaseUrl();
    string ToAbsoluteMediaUrl(string? path);
}