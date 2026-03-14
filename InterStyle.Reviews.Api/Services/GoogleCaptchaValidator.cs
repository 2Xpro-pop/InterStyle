using System.Text.Json;

namespace InterStyle.Reviews.Api.Services;

public sealed class GoogleCaptchaValidator : ICaptchaValidator
{
    private readonly HttpClient _httpClient;
    private readonly string _secretKey;

    public GoogleCaptchaValidator(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _secretKey = configuration["Captcha:SecretKey"] ?? throw new InvalidOperationException("Captcha:SecretKey is not configured");
    }

    public async Task<bool> ValidateAsync(string token, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(token))
        {
            return false;
        }

        var requestContent = new FormUrlEncodedContent(
        [
            new KeyValuePair<string, string>("secret", _secretKey),
            new KeyValuePair<string, string>("response", token)
        ]);

        try
        {
            var response = await _httpClient.PostAsync(
                "https://www.google.com/recaptcha/api/siteverify",
                requestContent,
                cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                return false;
            }

            var jsonResponse = await response.Content.ReadAsStringAsync(cancellationToken);
            var result = JsonSerializer.Deserialize<RecaptchaResponse>(jsonResponse, JsonSerializerOptions.Web);

            return result?.Success ?? false;
        }
        catch
        {
            return false;
        }
    }

    private sealed record RecaptchaResponse(bool Success);
}
