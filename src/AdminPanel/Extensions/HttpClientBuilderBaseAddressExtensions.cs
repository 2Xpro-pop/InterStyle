namespace AdminPanel.Extensions;

public static class HttpClientBuilderBaseAddressExtensions
{
    public static IHttpClientBuilder WithBaseAddress(this IHttpClientBuilder builder, string baseAddress)
    {
        return builder.ConfigureHttpClient(client => client.BaseAddress = new Uri(baseAddress));
    }
}
