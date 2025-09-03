using DealershipStockManagement.Application.Dtos;
using DealershipStockManagement.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DealershipStockManagement.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class StockController : ControllerBase
{
    private readonly IStockService _svc;
    public StockController(IStockService svc) => _svc = svc;

    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] int page = 1, [FromQuery] int pageSize = 10,
        [FromQuery] string? sortBy = "DTUpdated", [FromQuery] string? sortDir = "desc",
        [FromQuery] string? search = null)
    {
        string? ImageUrlFactory(int imgId) =>
            Url.Action(nameof(GetImage), "Stock", new { id = imgId }, Request.Scheme);

        var result = await _svc.GetPagedAsync(page, pageSize, search, sortBy, sortDir, ImageUrlFactory);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetSingle(int id)
    {
        string? ImageUrlFactory(int imgId) =>
            Url.Action(nameof(GetImage), "Stock", new { id = imgId }, Request.Scheme);

        var result = await _svc.GetByIdAsync(id, ImageUrlFactory);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpGet("image/{id}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetImage(int id)
    {
        var img = await _svc.GetImageAsync(id);
        if (img is null) return NotFound();
        var (data, filename, mime) = img.Value;
        return File(data, mime, filename);
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Create([FromForm] StockItemCreateDto dto)
    {
        try
        {
            var id = await _svc.CreateAsync(dto);
            return CreatedAtAction(nameof(GetSingle), new { id }, new { id });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("{id}")]
    [Authorize]
    public async Task<IActionResult> Update(int id, [FromForm] StockItemUpdateDto dto)
    {
        try
        {
            await _svc.UpdateAsync(id, dto);
            return NoContent();
        }
        catch (KeyNotFoundException) { return NotFound(); }
        catch (InvalidOperationException ex) { return BadRequest(new { message = ex.Message }); }
    }

    [HttpDelete("{id}")]
    [Authorize]
    public async Task<IActionResult> Delete(int id)
    {
        await _svc.DeleteAsync(id);
        return NoContent();
    }
}