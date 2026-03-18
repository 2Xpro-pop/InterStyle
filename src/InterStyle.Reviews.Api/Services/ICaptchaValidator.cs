namespace InterStyle.Reviews.Api.Services;

public interface ICaptchaValidator
{
    Task<bool> ValidateAsync(string token, CancellationToken cancellationToken = default);
}
