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
                    "AI recommendation service returned status code {StatusCode}. Body: {ResponseBody}",
                    (int)response.StatusCode,
                    Truncate(errorBody));

                return null;
            }

            var mediaType = response.Content.Headers.ContentType?.MediaType;

            if (!string.Equals(
                    mediaType,
                    "application/json",
                    StringComparison.OrdinalIgnoreCase) &&
                !mediaType?.EndsWith("+json", StringComparison.OrdinalIgnoreCase) == true)
            {
                var responseBody = await ReadResponseBodyAsync(
                    response,
                    cancellationToken);

                logger.LogError(
                    "AI recommendation service returned an unsupported content type {ContentType}. Body: {ResponseBody}",
                    mediaType ?? "unknown",
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
                    "AI recommendation service returned an empty or invalid response.");
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
            // Preserve cancellation requested by the API client.
            throw;
        }
        catch (HttpRequestException exception)
        {
            logger.LogError(
                exception,
                "HTTP request to the AI recommendation service failed.");

            return null;
        }
        catch (JsonException exception)
        {
            logger.LogError(
                exception,
                "Failed to deserialize the AI recommendation service response.");

            return null;
        }
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