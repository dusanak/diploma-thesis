using DiplomaThesis.Server.Models;
using Microsoft.Extensions.Options;
using Microsoft.PowerBI.Api;
using Microsoft.PowerBI.Api.Models;
using Microsoft.Rest;

namespace DiplomaThesis.Server.Services;

public class PowerBiService
{
    private readonly AadService _aadService;
    private readonly IOptions<PowerBi> _powerBiOptions;

    public PowerBiService(AadService aadService, IOptions<PowerBi> powerBiOptions)
    {
        _aadService = aadService;
        _powerBiOptions = powerBiOptions;
    }

    private PowerBIClient GetPowerBiClient()
    {
        var tokenCredentials = new TokenCredentials(_aadService.GetAccessToken(), "Bearer");
        return new PowerBIClient(new Uri(_powerBiOptions.Value.ApiRoot), tokenCredentials);
    }

    public async Task<Dashboard?> GetDashboard(Guid dashboardId)
    {
        var powerBiClient = GetPowerBiClient();

        var response = await powerBiClient.Dashboards.GetDashboardInGroupWithHttpMessagesAsync(
            Guid.Parse(_powerBiOptions.Value.GroupId),
            dashboardId);

        return response.Response.IsSuccessStatusCode ? response.Body : null;
    }
    
    public async Task<IEnumerable<Dashboard>> GetDashboards()
    {
        var powerBiClient = GetPowerBiClient();

        var response = await powerBiClient.Dashboards.GetDashboardsInGroupWithHttpMessagesAsync(
            Guid.Parse(_powerBiOptions.Value.GroupId));

        return response.Response.IsSuccessStatusCode ? response.Body.Value : new List<Dashboard>();
    }
    
    public async Task<Dashboard?> CreateDashboard(string name)
    {
        var powerBiClient = GetPowerBiClient();

        var addDashboardRequest = new AddDashboardRequest { Name = name };

        var response = await powerBiClient.Dashboards.AddDashboardInGroupWithHttpMessagesAsync(
            Guid.Parse(_powerBiOptions.Value.GroupId),
            addDashboardRequest);

        return response.Response.IsSuccessStatusCode ? response.Body : null;
    }
}