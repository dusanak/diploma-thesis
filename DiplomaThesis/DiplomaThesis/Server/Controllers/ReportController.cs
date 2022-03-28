using DiplomaThesis.Server.Data;
using DiplomaThesis.Server.Services;
using DiplomaThesis.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DiplomaThesis.Server.Controllers;

[Authorize]
[ApiController]
[Route("[controller]/[action]")]
public class ReportController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly PowerBiService _service;
    
    public ReportController(ApplicationDbContext context, PowerBiService service)
    {
        _context = context;
        _service = service;
    }
    
    [HttpGet("{reportId}")]
    public async Task<ActionResult> GetReport(
        [FromRoute] Guid reportId
        )
    {
        var result = await _service.GetReport(reportId);

        if (result is null)
        {
            return NotFound();
        }

        return Ok(new ReportContract
        {
            Id = result.Id,
            Name = result.Name,
            EmbedUrl = result.EmbedUrl,
            EmbedToken = _service.GetEmbedTokenForReport(result.Id, Guid.Parse(result.DatasetId))
        });
    }

    [HttpGet]
    public async Task<ActionResult> ListReports()
    {
        var result = (await _service.GetReports()).ToList();

        // var token = await _service.GetEmbedTokenForReports(
        //     result.Select(report => report.Id).ToList(),
        //     result.Select(report => Guid.Parse(report.DatasetId)).ToList());

        var response = result.Select(
            report => new ReportContract
            {
                Id = report.Id,
                Name = report.Name,
                EmbedUrl = report.EmbedUrl,
                EmbedToken = _service.GetEmbedTokenForReport(report.Id, Guid.Parse(report.DatasetId))
            }).ToList();

        return Ok(response);
    }
    
    [HttpPost]
    public ActionResult CreateReport()
    {
        throw new NotImplementedException();
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