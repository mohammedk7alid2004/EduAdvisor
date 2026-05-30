namespace EduAdvisor.Application.DTO.Auth
{
    public class RefreshTokenResponseDto
    {
        public string Token { get; set; } = string.Empty;
        public DateTime TokenExpiresAt { get; set; }
        public string RefreshToken { get; set; } = string.Empty;
        public DateTime RefreshTokenExpiresAt { get; set; }
    }
}