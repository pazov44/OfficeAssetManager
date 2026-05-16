using OfficeAssetManager.Core.DTO;

namespace OfficeAssetManager.Core.ServiceContracts;

public interface IAssetLogService
{
    // For the system to record a new event
    Task RecordLogAsync(int assetId, string action, string details, string performedBy);

    // For the UI to display the history of a specific asset
    Task<IEnumerable<AssetLogResponseDto>> GetLogsByAssetIdAsync(int assetId);

    // For the Admin to see all recent activity across the office
    Task<IEnumerable<AssetLogResponseDto>> GetAllLogsAsync();
}