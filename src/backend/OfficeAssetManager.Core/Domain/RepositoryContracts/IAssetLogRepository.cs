using OfficeAssetManager.Core.Domain.Entities;

namespace OfficeAssetManager.Core.Domain.RepositoryContracts;

public interface IAssetLogRepository : IRepository<AssetLog>
{
    Task<IEnumerable<AssetLog>> GetLogsByAssetIdAsync(int assetId);
}