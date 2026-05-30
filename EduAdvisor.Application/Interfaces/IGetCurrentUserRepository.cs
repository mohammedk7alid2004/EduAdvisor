namespace EduAdvisor.Application.Interfaces
{
    public interface IGetCurrentUserRepository
    {
        public string GetCurrentUser();
        public bool IsAuthenticated();
        public string GetUserId();
    }
}
