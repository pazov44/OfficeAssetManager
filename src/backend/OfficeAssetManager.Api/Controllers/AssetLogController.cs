using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OfficeAssetManager.Api.Helpers;
using OfficeAssetManager.Core.DTO;
using OfficeAssetManager.Core.ServiceContracts;

namespace OfficeAssetManager.API.Controllers;

[Authorize(Roles = "Admin")] // Only Admins should see the full audit trail
public class AssetLogController : ApiControllerBase
{
    private readonly IAssetLogService _logService;

    public AssetLogController(IAssetLogService logService)
    {
        _logService = logService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<AssetLogResponseDto>>> GetAllActivity()
    {
        var logs = await _logService.GetAllLogsAsync();
        return Ok(logs);
    }

    [HttpGet("asset/{assetId}")]
    public async Task<ActionResult<IEnumerable<AssetLogResponseDto>>> GetByAsset(int assetId)
    {
        var logs = await _logService.GetLogsByAssetIdAsync(assetId);
        return Ok(logs);
    }
}