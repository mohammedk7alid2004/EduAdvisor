namespace EduAdvisor.Application.Common.Abstractions.Consts;

public static class RegexPatterns
{
    public const string UniversityEmail = @"^[^@\s]+@[^@\s]+\.[^@\s]+\.[^@\s]+$";
    public const string Email = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
    public const string ImageFilePattern = @"(?i)^.*\.(jpg|jpeg|png)$";
    public const string Password = @"(?=(.*[0-9]))(?=.*[\!@#$%^&*()\\[\]{}\-_+=~`|:;""'<>,./?])(?=.*[a-z])(?=(.*[A-Z]))(?=(.*)).{8,}";
}