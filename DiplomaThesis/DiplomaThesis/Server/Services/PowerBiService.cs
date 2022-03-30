using System.Runtime.InteropServices;
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

    public async Task<IEnumerable<Report>> GetReports()
    {
        var powerBiClient = GetPowerBiClient();

        var response = await powerBiClient.Reports.GetReportsInGroupWithHttpMessagesAsync(
            Guid.Parse(_powerBiOptions.Value.GroupId));

        return response.Response.IsSuccessStatusCode ? response.Body.Value : new List<Report>();
    }

    public async Task<Report?> GetReport(Guid reportId)
    {
        var powerBiClient = GetPowerBiClient();

        var response = await powerBiClient.Reports.GetReportInGroupWithHttpMessagesAsync(
            Guid.Parse(_powerBiOptions.Value.GroupId),
            reportId);

        return response.Response.IsSuccessStatusCode ? response.Body : null;
    }

    public async Task<IEnumerable<Dashboard>> GetDashboards()
    {
        var powerBiClient = GetPowerBiClient();

        var response = await powerBiClient.Dashboards.GetDashboardsInGroupWithHttpMessagesAsync(
            Guid.Parse(_powerBiOptions.Value.GroupId));

        return response.Response.IsSuccessStatusCode ? response.Body.Value : new List<Dashboard>();
    }

    public string GetEmbedTokenForReport(Guid reportId, Guid datasetId)
    {
        PowerBIClient pbiClient = GetPowerBiClient();

        var tokenRequest = new GenerateTokenRequest(TokenAccessLevel.View, datasetId.ToString());

        var response = pbiClient.Reports.GenerateToken(
            Guid.Parse(_powerBiOptions.Value.GroupId),
            reportId,
            tokenRequest);

        return response.Token;
    }

    public async Task<EmbedToken> GetEmbedTokenForReports(IList<Guid> reportIds, IList<Guid> datasetIds,
        [Optional] IList<Guid> targetWorkspaceIds)
    {
        PowerBIClient pbiClient = GetPowerBiClient();

        // Convert report Ids to required types
        var reports = reportIds.Select(reportId => new GenerateTokenRequestV2Report(reportId)).ToList();

        // Convert dataset Ids to required types
        var datasets = datasetIds.Select(datasetId => new GenerateTokenRequestV2Dataset(datasetId.ToString())).ToList();

        // Convert target workspace Ids to required types
        IList<GenerateTokenRequestV2TargetWorkspace> targetWorkspaces = null;
        if (targetWorkspaceIds != null)
        {
            targetWorkspaces = targetWorkspaceIds
                .Select(targetWorkspaceId => new GenerateTokenRequestV2TargetWorkspace(targetWorkspaceId)).ToList();
        }

        // Create a request for getting Embed token 
        // This method works only with new Power BI V2 workspace experience
        var tokenRequest = new GenerateTokenRequestV2(
            datasets,
            reports,
            targetWorkspaceIds != null ? targetWorkspaces : null
        );

        // Generate Embed token
        var embedToken = await pbiClient.EmbedToken.GenerateTokenAsync(tokenRequest);

        return embedToken;
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

    public async Task<Dataset?> GetDataset(Guid datasetId)
    {
        var powerBiClient = GetPowerBiClient();

        var response = await powerBiClient.Datasets.GetDatasetInGroupWithHttpMessagesAsync(
            Guid.Parse(_powerBiOptions.Value.GroupId),
            datasetId.ToString());

        return response.Response.IsSuccessStatusCode ? response.Body : null;
    }
    
    public async Task<IEnumerable<Dataset>> GetDatasets()
    {
        var powerBiClient = GetPowerBiClient();

        var response = await powerBiClient.Datasets.GetDatasetsInGroupWithHttpMessagesAsync(
            Guid.Parse(_powerBiOptions.Value.GroupId));

        return response.Response.IsSuccessStatusCode ? response.Body.Value : new List<Dataset>();
    }
    
    public async Task<Dataset?> CreateDataset(string datasetName)
    {
        var powerBiClient = GetPowerBiClient();

        var createDatasetRequest = new CreateDatasetRequest(
            datasetName, 
            new List<Table>{new Table()});
        var response = await powerBiClient.Datasets.PostDatasetInGroupWithHttpMessagesAsync(
            Guid.Parse(_powerBiOptions.Value.GroupId),
            createDatasetRequest
        );
        return response.Response.IsSuccessStatusCode ? response.Body : null;
    }
    
    public async Task PushRowsToDataset(Guid datasetId, List<object> rows, string tableName="Data")
    {
        var powerBiClient = GetPowerBiClient();

        var response = await powerBiClient.Datasets.PostRowsInGroupWithHttpMessagesAsync(
            Guid.Parse(_powerBiOptions.Value.GroupId),
            datasetId.ToString(),
            tableName,
            new PostRowsRequest(rows)
        );
    } 
}