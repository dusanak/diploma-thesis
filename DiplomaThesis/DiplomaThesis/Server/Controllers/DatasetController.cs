using System.Text.Json;
using DiplomaThesis.Server.Data;
using DiplomaThesis.Server.Services;
using DiplomaThesis.Shared.Commands;
using DiplomaThesis.Shared.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.PowerBI.Api.Models;
using Newtonsoft.Json.Linq;

namespace DiplomaThesis.Server.Controllers;

[Authorize]
[ApiController]
[Route("[controller]/[action]")]
public class DatasetController : ControllerBase
{
    private readonly PowerBiService _service;

    public DatasetController(PowerBiService service)
    {
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
    public async Task<ActionResult> ListDatasets()
    {
        var result = await _service.GetDatasets();
        

        return Ok(result.Select(dataset =>
            new DatasetContract
            {
                Id = Guid.Parse(dataset.Id),
                Name = dataset.Name
            }).ToList());
    }
    
    [Authorize(Roles = "Architect")]
    [HttpPost]
    public async Task<ActionResult> CreateDataset(
        [FromBody] CreateDatasetCommand createDatasetCommand
    )
    {
        if (createDatasetCommand.Name.Length == 0 || !createDatasetCommand.Columns.Any())
        {
            return BadRequest();
        }
        
        var result = await _service.CreateDataset(
            createDatasetCommand.Name, 
            createDatasetCommand.Columns.Select(name => new Column(name, "string"))
            );

        if (result is null)
        {
            return StatusCode(500);
        }
        
        return Ok(result);
    }
    
    [Authorize(Roles = "Architect")]
    [HttpPost("{datasetName}")]
    public async Task<ActionResult> UploadNewDataset(
        [FromRoute] string datasetName,
        [FromBody] List<object> rows
    )
    {
        if (datasetName.Length == 0 || rows.Count == 0)
        {
            return BadRequest();
        }

        var columns = new List<Column>();
        foreach (var element in ((JObject)rows[0]).Properties())
        {
            if (DateTime.TryParse(element.Value.ToString(), out _))
            {
                columns.Add(new Column(element.Name, "datetime"));
            } 
            else if (long.TryParse(element.Value.ToString(), out _))
            {
                columns.Add(new Column(element.Name, "int64"));
            }
            else
            {
                columns.Add(new Column(element.Name, "string"));
            }
        }
        
        var dataset = await _service.CreateDataset(datasetName, columns);
        if (dataset is null)
        {
            return StatusCode(500);
        }

        var result = await _service.PushRowsToDataset(Guid.Parse(dataset.Id), rows);
        return result ? Ok() : StatusCode(500);
    }
    
    [Authorize(Roles = "Architect")]
    [HttpPost("{datasetId}")]
    public async Task<ActionResult> UploadRowsToDataset(
        [FromRoute] Guid datasetId,
        [FromBody] List<object> rows
    )
    {
        var dataset = await _service.GetDataset(datasetId);
        if (dataset is null)
        {
            return NotFound();
        }

        var result = await _service.PushRowsToDataset(datasetId, rows);
        return result ? Ok() : StatusCode(500);
    }
}