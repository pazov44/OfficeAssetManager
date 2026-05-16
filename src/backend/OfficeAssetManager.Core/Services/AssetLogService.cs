using OfficeAssetManager.Core.Domain.Entities;
using OfficeAssetManager.Core.DTO;
using OfficeAssetManager.Core.Domain.RepositoryContracts;
using OfficeAssetManager.Core.ServiceContracts;

namespace OfficeAssetManager.Core.Services;

public class AssetLogService : IAssetLogService
{
    private readonly IAssetLogRepository _logRepository;

    public AssetLogService(IAssetLogRepository logRepository)
    {
        _logRepository = logRepository;
    }

    public async Task RecordLogAsync(int assetId, string action, string details, string performedBy)
    {
        var log = new AssetLog
        {
            AssetId = assetId,
            Action = action,
            Details = details,
            PerformedBy = performedBy,
            CreatedAt = DateTime.UtcNow
        };

        await _logRepository.AddAsync(log);
        await _logRepository.SaveChangesAsync();
    }

    public async Task<IEnumerable<AssetLogResponseDto>> GetLogsByAssetIdAsync(int assetId)
    {
        var logs = await _logRepository.GetLogsByAssetIdAsync(assetId);
        return logs.Select(MapToDto);
    }

    public async Task<IEnumerable<AssetLogResponseDto>> GetAllLogsAsync()
    {
        var logs = await _logRepository.GetAllAsync();
        return logs.Select(MapToDto);
    }

    // Helper method for manual mapping
    private static AssetLogResponseDto MapToDto(AssetLog log) => new AssetLogResponseDto
    {
        Id = log.Id,
        AssetId = log.AssetId,
        Action = log.Action,
        Details = log.Details,
        PerformedBy = log.PerformedBy,
        CreatedAt = log.CreatedAt
    };
}