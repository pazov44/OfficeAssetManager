using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
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

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<ActionResult<AssetResponseDto>> Create(AssetCreateDto dto)
    {
        try
        {
            var result = await _assetService.CreateAssetAsync(dto);

            var userEmail = User.FindFirstValue(ClaimTypes.Email) ?? User.Identity?.Name ?? "Admin Admin";
            await _logService.RecordLogAsync(
            
              result.Id,
               "Asset Created",
               $"Asset '{result.Name}' with Tag {result.AssetTag} added to assets",
               userEmail
            );

            return Ok(result);
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

        var userEmail = User.FindFirstValue(ClaimTypes.Email) ?? User.Identity?.Name ?? "Admin Admin";
        await _logService.RecordLogAsync(
        
           id,
          "Asset Updated",
           $"Asset state changed. New Status code: {dto.Status}",
            userEmail
        );

        return Ok(result);
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var asset = await _assetService.GetAssetByIdAsync(id);
        if (asset == null) return NotFound();

        string assetName = asset.Name;

        var userEmail = User.FindFirstValue(ClaimTypes.Email) ?? User.Identity?.Name ?? "Admin Admin";
        await _logService.RecordLogAsync(
            id,
            "Asset Removed",
            $"Asset '{assetName}' permanently removed from assets",
            userEmail
        );

        var success = await _assetService.DeleteAssetAsync(id);
        if (!success) return NotFound();

        return Ok(new { message = "Asset successfully deleted" });
    }

    [HttpGet("{id}/logs")]
    public async Task<ActionResult<IEnumerable<AssetLogResponseDto>>> GetLogs(int id)
    {
        var logs = await _logService.GetLogsByAssetIdAsync(id);
        return Ok(logs);
    }
}