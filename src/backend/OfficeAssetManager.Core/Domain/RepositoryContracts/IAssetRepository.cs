using OfficeAssetManager.Core.Domain.Entities;

namespace OfficeAssetManager.Core.Domain.RepositoryContracts;

public interface IAssetRepository : IRepository<Asset>
{
    Task<Asset?> GetByAssetTagAsync(string assetTag);
    Task<bool> AssetTagExistsAsync(string assetTag);
}