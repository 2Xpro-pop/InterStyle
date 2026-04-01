using System.Diagnostics;

namespace AdminPanel.Extensions;

public sealed class HttpLoggingHandler(ILoggerFactory loggerFactory) : DelegatingHandler
{
    private readonly ILogger _logger = loggerFactory.CreateLogger("AdminPanel.Http");

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var method = request.Method;
        var uri = request.RequestUri;

        _logger.LogInformation("HTTP {Method} {Uri} started", method, uri);

        var stopwatch = Stopwatch.StartNew();

        try
        {
            var response = await base.SendAsync(request, cancellationToken);

            stopwatch.Stop();

            _logger.LogInformation(
                "HTTP {Method} {Uri} completed with {StatusCode} in {ElapsedMs}ms",
                method, uri, (int)response.StatusCode, stopwatch.ElapsedMilliseconds);

            return response;
        }
        catch (Exception ex)
        {
            stopwatch.Stop();

            _logger.LogError(ex,
                "HTTP {Method} {Uri} failed after {ElapsedMs}ms",
                method, uri, stopwatch.ElapsedMilliseconds);

            throw;
        }
    }
}
