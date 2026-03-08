using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

namespace AdminPanel.Extensions;

public static class WebAssemblyHostEnvironmentExtensions
{
    public static string GetSiteBaseAddress(this IWebAssemblyHostEnvironment environment)
    {
        ArgumentNullException.ThrowIfNull(environment);

        var uri = new Uri(environment.BaseAddress);

        return uri.GetLeftPart(UriPartial.Authority);
    }
}