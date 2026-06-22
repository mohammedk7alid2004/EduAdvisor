using System.Net.Http.Json;
using System.Text.Json;
using EduAdvisor.Application.DTO.AiRecommendation;
using EduAdvisor.Application.Interfaces.ExternalServices;
using Microsoft.Extensions.Logging;

namespace EduAdvisor.Infrastructure.ExternalServices.AiRecommendation;

public sealed class AiRecommendationService(
    HttpClient httpClient,
    ILogger<AiRecommendationService> logger)
    : IAiRecommendationService
{
    private const int MaximumLoggedBodyLength = 300;

    private static readonly JsonSerializerOptions JsonOptions = new(
        JsonSerializerDefaults.Web)
    {
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
        PropertyNameCaseInsensitive = true
    };

    public async Task<AiRecommendationResponseDto?> GetRecommendationsAsync(
        AiRecommendationRequestDto request,
        CancellationToken cancellationToken)
    {
        try
        {
            using var response = await httpClient.PostAsJsonAsync(
                "recommend",
                request,
                JsonOptions,
                cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                var errorBody = await ReadResponseBodyAsync(
                    response,
                    cancellationToken);

                logger.LogError(
                    "AI recommendation service returned status {StatusCode}. Body: {ResponseBody}",
                    (int)response.StatusCode,
                    Truncate(errorBody));

                return null;
            }

            if (!IsJsonResponse(response))
            {
                var responseBody = await ReadResponseBodyAsync(
                    response,
                    cancellationToken);

                logger.LogError(
                    "AI recommendation service returned unsupported content type {ContentType}. Body: {ResponseBody}",
                    response.Content.Headers.ContentType?.MediaType ?? "unknown",
                    Truncate(responseBody));

                return null;
            }

            var result = await response.Content
                .ReadFromJsonAsync<AiRecommendationResponseDto>(
                    JsonOptions,
                    cancellationToken);

            if (result is null)
            {
                logger.LogError(
                    "AI recommendation service returned an empty response.");

                return null;
            }

            if (result.Recommendations.Count is 0)
            {
                logger.LogWarning(
                    "AI recommendation service returned no recommendations.");
            }

            return result;
        }
        catch (OperationCanceledException)
            when (!cancellationToken.IsCancellationRequested)
        {
            logger.LogError(
                "AI recommendation service request timed out.");

            return null;
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (HttpRequestException exception)
        {
            logger.LogError(
                exception,
                "HTTP request to AI recommendation service failed.");

            return null;
        }
        catch (JsonException exception)
        {
            logger.LogError(
                exception,
                "Failed to deserialize AI recommendation response.");

            return null;
        }
    }

    private static bool IsJsonResponse(HttpResponseMessage response)
    {
        var mediaType = response.Content.Headers.ContentType?.MediaType;

        return mediaType is not null &&
               (mediaType.Equals(
                    "application/json",
                    StringComparison.OrdinalIgnoreCase) ||
                mediaType.EndsWith(
                    "+json",
                    StringComparison.OrdinalIgnoreCase));
    }

    private static async Task<string> ReadResponseBodyAsync(
        HttpResponseMessage response,
        CancellationToken cancellationToken)
    {
        return await response.Content.ReadAsStringAsync(cancellationToken);
    }

    private static string Truncate(string value)
    {
        return value.Length <= MaximumLoggedBodyLength
            ? value
            : value[..MaximumLoggedBodyLength];
    }
}