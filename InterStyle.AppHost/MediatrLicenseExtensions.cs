namespace InterStyle.AppHost;

public static class MediatrLicenseExtensions
{
    public static IResourceBuilder<T> WithMediatrLicense<T>(this IResourceBuilder<T> builder, IResourceBuilder<ParameterResource> licenseKey) where T : IResourceWithEnvironment
    {
        return builder.WithEnvironment(async callback =>
        {
            var licenseKeyValue = await licenseKey.Resource.GetValueAsync(callback.CancellationToken);

            if (string.IsNullOrEmpty(licenseKeyValue))
            {
                return;
            }

            callback.EnvironmentVariables["MediatR__LicenseKey"] = licenseKeyValue;
        });
    }
}
