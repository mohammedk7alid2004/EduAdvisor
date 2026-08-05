using EduAdvisor.Domain.Entities.Base;


namespace EduAdvisor.Domain.Entities.AuthModule;

public class RefreshToken: BaseEntity
{
    public string Token { get; set; } = string.Empty;
    public DateTime ExpiresOn { get; set; }
    public DateTime? RevokedOn { get; set; }
    public string UserId { get; set; }=string.Empty;
    public User? User { get; set; }

    public bool IsActive => RevokedOn == null && !IsExpired;
    public bool IsExpired => DateTime.UtcNow >= ExpiresOn;
}
