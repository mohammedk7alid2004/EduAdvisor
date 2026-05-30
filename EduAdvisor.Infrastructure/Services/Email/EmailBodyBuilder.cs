namespace EduAdvisor.Infrastructure.Services.Email;

public static class EmailBodyBuilder
{
    public static string GenerateEmailBody(
        string templateName,
        Dictionary<string, string> placeholders)
    {
        var templatePath = Path.Combine(
            Directory.GetCurrentDirectory(),
            "EmailTemplates",
            $"{templateName}.html");

        var body = System.IO.File.ReadAllText(templatePath);
        foreach (var (key, value) in placeholders)
            body = body.Replace(key, value);

        return body;
    }
}