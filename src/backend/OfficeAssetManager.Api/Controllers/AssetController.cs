using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OfficeAssetManager.Api.Helpers;
using OfficeAssetManager.Core.Domain.Enums;
using OfficeAssetManager.Core.DTO;
using OfficeAssetManager.Core.ServiceContracts;

namespace OfficeAssetManager.API.Controllers;
public class AssetController : ApiControllerBase
{
    private readonly IAssetService _assetService;
    private readonly IAssetLogService _logService;

    public AssetController(IAssetService assetService, IAssetLogService logService)
    {
        _assetService = assetService;
        _logService = logService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<AssetResponseDto>>> GetAll()
    {
        var assets = await _assetService.GetAllAssetsAsync();
        return Ok(assets);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<AssetResponseDto>> GetById(int id)
    {
        var asset = await _assetService.GetAssetByIdAsync(id);
        if (asset == null) return NotFound();
        return Ok(asset);
    }

    [Authorize(Roles = "Admin")] // Only Admins can add assets
    [HttpPost]
    public async Task<ActionResult<AssetResponseDto>> Create(AssetCreateDto dto)
    {
        try
        {
            var result = await _assetService.CreateAssetAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, AssetUpdateDto dto)
    {
        var result = await _assetService.UpdateAssetAsync(id, dto);
        if (result == null) return NotFound();
        return Ok(result);
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var success = await _assetService.DeleteAssetAsync(id);
        if (!success) return NotFound();
        return NoContent();
    }

    [HttpGet("{id}/logs")]
    public async Task<ActionResult<IEnumerable<AssetLogResponseDto>>> GetLogs(int id)
    {
        var logs = await _logService.GetLogsByAssetIdAsync(id);
        return Ok(logs);
    }
}