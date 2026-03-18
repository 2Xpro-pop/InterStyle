using AdminPanel.Services;
using System.Net.Http.Headers;

namespace AdminPanel.Extensions;

public static class HttpClientBuildeJwtTokenAdderExtensions
{
    public static IHttpClientBuilder AddJwtToken(this IHttpClientBuilder builder)
    {
        builder.AddHttpMessageHandler(sp =>
        {
            var stateProvider = sp.GetRequiredService<JwtAuthenticationStateProvider>();
            return new JwtTokenMessageHandler(stateProvider);
        });
        return builder;
    }
}

file sealed class JwtTokenMessageHandler(JwtAuthenticationStateProvider stateProvider) : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var state = await stateProvider.GetAuthenticationStateAsync();

        if(state is JwtAuthenticationStateProvider.JwtAuthenticationState jwtState)
        {
            if(!jwtState.IsExpired)
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", jwtState.Token.RawData);
            }
        }

        return await base.SendAsync(request, cancellationToken);
    }
}
