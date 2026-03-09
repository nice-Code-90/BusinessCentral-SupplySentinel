using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Client;

namespace SupplySentinel.Infrastructure.REST;

public class AuthenticationHandler : DelegatingHandler
{
    private readonly BusinessCentralOptions _bcOptions;
    private readonly IConfidentialClientApplication _confidentialClient;

    public AuthenticationHandler(IOptions<BusinessCentralOptions> bcOptions)
    {
        _bcOptions = bcOptions.Value;
        _confidentialClient = ConfidentialClientApplicationBuilder
            .Create(_bcOptions.ClientId)
            .WithClientSecret(_bcOptions.ClientSecret)
            .WithAuthority(new Uri($"https://login.microsoftonline.com/{_bcOptions.TenantId}"))
            .Build();
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var authResult = await _confidentialClient
            .AcquireTokenForClient(new[] { "https://api.businesscentral.dynamics.com/.default" })
            .ExecuteAsync(cancellationToken);

        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", authResult.AccessToken);
        
        return await base.SendAsync(request, cancellationToken);
    }
}
