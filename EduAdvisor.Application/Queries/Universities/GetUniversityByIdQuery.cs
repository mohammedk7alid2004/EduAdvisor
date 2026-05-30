using EduAdvisor.Application.DTO.Universities;

namespace EduAdvisor.Application.Queries.Universities;

public sealed record GetUniversityByIdQuery(Guid Id)
    : IRequest<Result<UniversityResponse>>;
