using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace API.Swagger;

/// <summary>
/// Adds OpenAPI <c>examples</c> (Swagger UI shows a select menu) for JSON responses:
/// success with <c>data</c> as list or single object where <c>name</c> is bilingual (En / Ar),
/// plus unified error <c>Result</c> shapes for common status codes.
/// </summary>
public sealed class SwaggerBilingualResponseExamplesOperationFilter : IOperationFilter
{
    private static readonly OpenApiExample SuccessListExample = new()
    {
        Summary = "Success — list (name: En + Ar)",
        Description = "مثال عندما يكون data مصفوفة وكل عنصر فيه name بلغتين.",
        Value = BuildSuccessValue(isArray: true)
    };

    private static readonly OpenApiExample SuccessSingleExample = new()
    {
        Summary = "Success — single item (name: En + Ar)",
        Description = "مثال عندما يكون data كائن واحد وفيه name بلغتين.",
        Value = BuildSuccessValue(isArray: false)
    };

    private static readonly OpenApiExample ErrorExample = new()
    {
        Summary = "Error — unified Result",
        Description = "نفس شكل الفشل الموحد (رسالة واحدة، data = null).",
        Value = new OpenApiObject
        {
            ["isSuccess"] = new OpenApiBoolean(false),
            ["message"] = new OpenApiString("المخزن غير موجود"),
            ["statusCode"] = new OpenApiInteger(404),
            ["data"] = new OpenApiNull()
        }
    };

    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        foreach (var (statusCode, response) in operation.Responses)
        {
            if (statusCode == "204" || statusCode == "304")
                continue;

            if (!ShouldAttachExamples(response, statusCode))
                continue;

            EnsureJsonMediaType(response);

            var media = response.Content!["EduAdvisor.Application/json"];
            media.Examples ??= new Dictionary<string, OpenApiExample>();

            if (IsSuccessStatus(statusCode))
            {
                media.Examples.TryAdd("SuccessListWithBilingualNames", SuccessListExample);
                media.Examples.TryAdd("SuccessSingleWithBilingualNames", SuccessSingleExample);
            }
            else if (IsErrorStatus(statusCode))
            {
                media.Examples.TryAdd("UnifiedErrorResult", ErrorExample);
            }
        }
    }

    private static bool ShouldAttachExamples(OpenApiResponse response, string statusCode)
    {
        if (response.Content is { Count: > 0 })
        {
            if (response.Content.Keys.Any(k =>
                    k.Contains("octet-stream", StringComparison.OrdinalIgnoreCase) ||
                    k.StartsWith("image/", StringComparison.OrdinalIgnoreCase) ||
                    k.Contains("pdf", StringComparison.OrdinalIgnoreCase)))
                return false;

            if (response.Content.ContainsKey("EduAdvisor.Application/json"))
                return true;

            if (response.Content.Keys.Any(k => k.StartsWith("text/", StringComparison.OrdinalIgnoreCase)))
                return false;
        }

        return IsSuccessStatus(statusCode) || IsErrorStatus(statusCode);
    }

    private static bool IsSuccessStatus(string code) =>
        code is "200" or "201" or "202";

    private static bool IsErrorStatus(string code) =>
        code is "400" or "401" or "403" or "404" or "409" or "422" or "500";

    private static void EnsureJsonMediaType(OpenApiResponse response)
    {
        response.Content ??= new Dictionary<string, OpenApiMediaType>();

        if (!response.Content.ContainsKey("EduAdvisor.Application/json"))
            response.Content["EduAdvisor.Application/json"] = new OpenApiMediaType();
    }

    private static IOpenApiAny BuildSuccessValue(bool isArray)
    {
        var bilingualName = new OpenApiObject
        {
            ["En"] = new OpenApiString("Egypt"),
            ["Ar"] = new OpenApiString("مصر")
        };

        var item1 = new OpenApiObject
        {
            ["id"] = new OpenApiInteger(1),
            ["name"] = bilingualName
        };

        var item2 = new OpenApiObject
        {
            ["id"] = new OpenApiInteger(2),
            ["name"] = new OpenApiObject
            {
                ["En"] = new OpenApiString("Saudi Arabia"),
                ["Ar"] = new OpenApiString("السعودية")
            }
        };

        IOpenApiAny data = isArray
            ? new OpenApiArray { item1, item2 }
            : item1;

        return new OpenApiObject
        {
            ["isSuccess"] = new OpenApiBoolean(true),
            ["message"] = new OpenApiString("Data fetched successfully"),
            ["statusCode"] = new OpenApiInteger(200),
            ["data"] = data
        };
    }
}
