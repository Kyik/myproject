using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Http;

namespace JobPostingSystem;

public static class Extensions
{
    // Check if the request is an AJAX request
    public static bool IsAjax(this HttpRequest request)
    {
        return request.Headers.XRequestedWith == "XMLHttpRequest";
    }
    // Check if a specific model state key is valid
    public static bool IsValid(this ModelStateDictionary ms, string key)
    {
        return ms.GetFieldValidationState(key) == ModelValidationState.Valid;
    }
    // Set a value in the session
    public static void SetSessionValue(this ISession session, string key, string value)
    {
        session.SetString(key, value);
    }

    // Get a value from the session
    public static string? GetSessionValue(this ISession session, string key)
    {
        var value = session.GetString(key);
        return value == null ? default : value;
    }

    // Check if a session key exists
    public static bool HasSessionKey(this ISession session, string key)
    {
        return session.Keys.Contains(key);
    }
}
