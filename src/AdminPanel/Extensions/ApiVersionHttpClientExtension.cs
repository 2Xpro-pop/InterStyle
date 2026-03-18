using System.Diagnostics.CodeAnalysis;

namespace AdminPanel.Extensions;

public static class ApiVersionHttpClientExtension
{
    public static IHttpClientBuilder AddApiV1(this IHttpClientBuilder builder)
    {
        builder.AddHttpMessageHandler(() => new ApiVersionMessageHandler("1.0"));

        return builder;
    }
}

/// <summary>
/// Add ?api-version={version} to all requests
/// </summary>
file class ApiVersionMessageHandler(string version) : DelegatingHandler
{
    private readonly string _version = version;

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        if (request.RequestUri is not null)
        {
            var uri = request.RequestUri;

            var query = uri.Query;

            if (!query.Contains("api-version=", StringComparison.OrdinalIgnoreCase))
            {
                var separator = string.IsNullOrEmpty(query) ? "?" : "&";
                var newUri = new Uri(uri + $"{separator}api-version={_version}");

                request.RequestUri = newUri;
            }
        }

        return base.SendAsync(request, cancellationToken);
    }
}