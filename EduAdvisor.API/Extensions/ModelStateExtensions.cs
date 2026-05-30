using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace API.Extensions;

public static class ModelStateExtensions
{
    public static string GetFirstErrorMessage(this ModelStateDictionary modelState)
    {
        foreach (var entry in modelState)
        {
            if (entry.Value?.Errors.Count > 0)
                return entry.Value.Errors[0].ErrorMessage;
        }

        return "Validation failed.";
    }
}
