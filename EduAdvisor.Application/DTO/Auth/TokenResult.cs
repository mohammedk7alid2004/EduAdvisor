namespace EduAdvisor.Application.DTO.Auth
{
    public class TokenResult
    {
        public string Token { get; set; } = string.Empty;

        public string RefreshToken { get; set; } = string.Empty;

        public DateTime ExpiresAt { get; set; }
    }
}
