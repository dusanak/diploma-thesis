using System.Security.Claims;
using DiplomaThesis.Server.Data;
using DiplomaThesis.Server.Models;
using DiplomaThesis.Server.Services;
using DiplomaThesis.Shared;
using DiplomaThesis.Shared.Commands;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
    
    [Authorize(Roles = "Architect")]
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

        var reportsInDb = await _context.Reports.ToListAsync();
        var reportInDb = reportsInDb.FirstOrDefault(
            reportDb => reportDb?.PowerBiId.Equals(reportId) ?? false, 
            null);
        if (reportInDb is null)
        {
            var newReportDb = new ReportDb
            {
                Id = Guid.NewGuid(),
                PowerBiId = reportId,
                UserGroup = null
            };
            _context.Reports.Add(newReportDb);
            await _context.SaveChangesAsync();

            reportInDb = newReportDb;
        }

        return Ok(new ReportContract
        {
            Id = result.Id,
            Name = result.Name,
            EmbedUrl = result.EmbedUrl,
            EmbedToken = _service.GetEmbedTokenForReport(result.Id, Guid.Parse(result.DatasetId)),
            UserGroupId = reportInDb.UserGroupId ?? Guid.Empty
        });
    }

    [HttpGet]
    public async Task<ActionResult> ListReports()
    {
        var result = (await _service.GetReports()).ToList();
        var reportsInDb = await _context.Reports.ToListAsync();

        var loggedInUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var loggedInUser = await _context.Users.FindAsync(loggedInUserId);
        var loggedInUsersGroupId = loggedInUser!.UserGroupId ?? Guid.Empty;

        var response = new List<ReportContract>();
        foreach (var report in result)
        {
            var reportInDb = reportsInDb.FirstOrDefault(
                reportDb => reportDb?.PowerBiId.Equals(report.Id) ?? false, null);
            if (reportInDb is null)
            {
                var newReportDb = new ReportDb
                {
                    Id = Guid.NewGuid(),
                    PowerBiId = report.Id,
                    UserGroup = null
                };
                _context.Reports.Add(newReportDb);
                await _context.SaveChangesAsync();

                reportInDb = newReportDb;
            }

            if (reportInDb.UserGroupId?.Equals(loggedInUsersGroupId) ?? true)
            {
                response.Add(new ReportContract
                {
                    Id = report.Id,
                    Name = report.Name,
                    EmbedUrl = report.EmbedUrl,
                    EmbedToken = _service.GetEmbedTokenForReport(report.Id, Guid.Parse(report.DatasetId)),
                    UserGroupId = reportInDb.UserGroupId ?? Guid.Empty
                });
            }
        }
        return Ok(response);
    }

    [Authorize(Roles = "Architect")]
    [HttpPost]
    public async Task<ActionResult> CloneReport(
        [FromBody] CloneReportCommand cloneReportCommand
    )
    {
        var originalReport = await _service.GetReport(cloneReportCommand.ReportId);
        if (originalReport is null)
        {
            return NotFound();
        }
        
        if (cloneReportCommand.NewName is null || cloneReportCommand.NewName.Length == 0)
        {
            return BadRequest();
        }

        var report = await _service.CloneReport(
            cloneReportCommand.ReportId,
            cloneReportCommand.NewName);

        if (report is null)
        {
            return StatusCode(500);
        }
        
        var newReportDb = new ReportDb
        {
            Id = Guid.NewGuid(),
            PowerBiId = report.Id,
            UserGroup = null
        };
        _context.Reports.Add(newReportDb);
        await _context.SaveChangesAsync();

        var response = new ReportContract 
        {
            Id = report.Id,
            Name = report.Name,
            EmbedUrl = report.EmbedUrl,
            EmbedToken = _service.GetEmbedTokenForReport(report.Id, Guid.Parse(report.DatasetId)),
            UserGroupId = Guid.Empty
        };

        return Ok(response);
    }
    
    [Authorize(Roles = "Architect")]
    [HttpPut]
    public ActionResult MoveReportToUserGroup(
        [FromBody] MoveReportToUserGroupCommand moveReportToUserGroupCommand
    )
    {
        var reports = _context.Reports.ToList();
        var report = reports.FirstOrDefault(
            reportDb => reportDb?.PowerBiId.Equals(moveReportToUserGroupCommand.ReportId)?? false, null);
        if (report is null) return NotFound();
    
        if (moveReportToUserGroupCommand.UserGroupId.Equals(Guid.Empty))
        {
            var userGroups = _context.UserGroups.ToList();
            var userGroup = userGroups.FirstOrDefault(
                group =>
                {
                    if (group is null || group.Reports is null)
                    {
                        return false;
                    }
                    return group.Users.Any(userInGroup => userInGroup.Id.Equals(report.Id.ToString()));
                }, null);
            report.UserGroup = null;
            userGroup?.Reports.Remove(report);
        }
        else
        {
            var userGroup = _context.UserGroups.Find(moveReportToUserGroupCommand.UserGroupId);
            if (userGroup is null) return NotFound();
                
            report.UserGroup = userGroup;
        }
    
        _context.SaveChanges();
            
        return Ok();
    }
    
    [HttpPost]
    public ActionResult CreateReport()
    {
        throw new NotImplementedException();
    }
    
    [HttpPut]
    public ActionResult UpdateReport()
    {
        throw new NotImplementedException();
    }
    
    [HttpDelete]
    public ActionResult DeleteReport()
    {
        throw new NotImplementedException();
    }
}