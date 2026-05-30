using EduAdvisor.Application.Queries.Users;

namespace EduAdvisor.Application.Validators.Users
{
    public class GetUserRolesValidator : AbstractValidator<GetUserRolesQuery>
    {
        public GetUserRolesValidator()
        {
            RuleFor(x => x.UserId).NotEmpty();
        }
    }
}
