using EduAdvisor.Application.DTO.AiRecommendation;

namespace EduAdvisor.Application.Interfaces.ExternalServices;

public interface IAiRecommendationService
{
    Task<AiRecommendationResponseDto?> GetRecommendationsAsync(
        AiRecommendationRequestDto request,
        CancellationToken cancellationToken);
}