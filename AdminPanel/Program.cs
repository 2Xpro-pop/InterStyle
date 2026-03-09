using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using AdminPanel;
using Microsoft.AspNetCore.Components.Authorization;
using AdminPanel.Services;
using Refit;
using Blazor.Extensions.Logging;
using AdminPanel.Extensions;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

var url = builder.HostEnvironment.GetSiteBaseAddress();

builder.Services.AddLogging(logging =>
{
    logging.AddBrowserConsole();
});


builder.Services.AddAuthorizationCore();
builder.Services.AddCascadingAuthenticationState();

builder.Services.AddJwtAuthentication();
builder.Services.AddSingleton<IAuthService, AuthService>();

builder.Services.AddRefitClient<IIdentityApi>()
    .WithBaseAddress(url)
    .AddApiV1();

builder.Services.AddRefitClient<ICurtainsApi>()
    .WithBaseAddress(url)
    .AddApiV1()
    .AddJwtToken();

builder.Services.AddRefitClient<ILeadsApi>()
    .WithBaseAddress(url)
    .AddApiV1()
    .AddJwtToken();

await builder.Build().RunAsync();
