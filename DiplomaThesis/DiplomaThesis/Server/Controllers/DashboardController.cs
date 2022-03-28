using DiplomaThesis.Server.Data;
using DiplomaThesis.Server.Services;
using DiplomaThesis.Shared.Commands;
using DiplomaThesis.Shared.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DiplomaThesis.Server.Controllers;

[Authorize]
[ApiController]
[Route("[controller]/[action]")]
public class DashboardController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly PowerBiService _service;
    
    public DashboardController(ApplicationDbContext context, PowerBiService service)
    {
        _context = context;
        _service = service;
    }
    
    //TODO add embed token
    [HttpGet("{dashboardId}")]
    public async Task<ActionResult> GetDashboard(
        [FromRoute] Guid dashboardId
        )
    {
        var result = await _service.GetDashboard(dashboardId);

        if (result is null)
        {
            return NotFound();
        }

        return Ok(result);
    }

    //TODO add embed token
    [HttpGet]
    public async Task<ActionResult> ListDashboards()
    {
        var result = await _service.GetDashboards();

        var response = result.Select(
            dashboard => new DashboardContract
            {
                Id = Guid.Empty,
                Name = dashboard.DisplayName,
                EmbedUrl = dashboard.EmbedUrl
            }).ToList();
        
        return Ok(response);
    }
    
    [HttpPost]
    public async Task<ActionResult> CreateDashboard(
        [FromBody] CreateDashboardCommand createDashboardCommand
        )
    {
        var result = await _service.CreateDashboard(createDashboardCommand.Name);

        if (result is null)
        {
            return StatusCode(500);
        }
        
        var response = new DashboardContract
        {
            Id = Guid.Empty,
            Name = result.DisplayName,
            EmbedUrl = result.EmbedUrl
        };

        return Ok(response);
    }
    
    [HttpPut]
    public ActionResult UpdateDashboard()
    {
        throw new NotImplementedException();
    }
    
    [HttpDelete]
    public ActionResult DeleteDashboard()
    {
        throw new NotImplementedException();
    }
}