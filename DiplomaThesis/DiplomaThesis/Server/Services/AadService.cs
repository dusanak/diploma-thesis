using DiplomaThesis.Server.Models;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Client;

namespace DiplomaThesis.Server.Services;

public class AadService
{
    private readonly IConfiguration _config;
    private readonly IOptions<AzureAd> _azureAd;

    public AadService(IConfiguration config, IOptions<AzureAd> azureAd)
    {
        _config = config;
        _azureAd = azureAd;
    }

    public string GetAccessToken()
    {
        var tenantSpecificUrl = _azureAd.Value.AuthorityUri + _azureAd.Value.DirectoryId;
        
        var clientApp = ConfidentialClientApplicationBuilder
            .Create(_azureAd.Value.ClientId)
            .WithClientSecret(_config.GetValue<string>("ClientSecret"))
            .WithAuthority(tenantSpecificUrl)
            .Build();
        
        var authenticationResult = clientApp.AcquireTokenForClient(new[] { _azureAd.Value.Resource + "/.default" }).ExecuteAsync().Result;

        return authenticationResult.AccessToken;
    }
}