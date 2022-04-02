using DiplomaThesis.Server.Data;
using DiplomaThesis.Server.Services;
using DiplomaThesis.Shared.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DiplomaThesis.Server.Controllers;

[Authorize]
[ApiController]
[Route("[controller]/[action]")]
public class DatasetController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly PowerBiService _service;

    public DatasetController(ApplicationDbContext context, PowerBiService service)
    {
        _context = context;
        _service = service;
    }

    [Authorize(Roles = "Architect")]
    [HttpGet("{datasetId}")]
    public async Task<ActionResult> GetDataset(
        [FromRoute] Guid datasetId
    )
    {
        var result = await _service.GetDataset(datasetId);
        if (result is null)
        {
            return NotFound();
        }

        return Ok(new DatasetContract
        {
            Id = Guid.Parse(result.Id),
            Name = result.Name
        });
    }

    [Authorize(Roles = "Architect")]
    [HttpGet]
    public async Task<ActionResult> ListDatasets(
        [FromRoute] Guid datasetId
    )
    {
        var result = await _service.GetDatasets();

        return Ok(result.Select(dataset =>
            new DatasetContract
            {
                Id = Guid.Parse(dataset.Id),
                Name = dataset.Name
            }).ToList());
    }
}