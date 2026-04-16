using Dozday.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Dozday.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
public class QualityAssuranceController(IQualityAssuranceService service) : ControllerBase
{
    private readonly IQualityAssuranceService _service = service;

    [HttpGet]
    public async Task<IActionResult> GetAllAsync()
    {
        var result = await _service.GetAllAsync();
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetByIdAsync(string id)
    {
        var result = await _service.GetByIdAsync(id);
        if (result == null) return NotFound();
        return Ok(result);
    }
}