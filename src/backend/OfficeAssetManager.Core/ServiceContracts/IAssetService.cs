using OfficeAssetManager.Core.DTO;
using OfficeAssetManager.Core.Domain.Enums;

namespace OfficeAssetManager.Core.ServiceContracts
{
    public interface IAssetService
    {
        Task<IEnumerable<AssetResponseDto>> GetAllAssetsAsync();
        Task<AssetResponseDto?> GetAssetByIdAsync(int id);
        Task<IEnumerable<AssetResponseDto>> GetAssetsByStatusAsync(AssetStatus status);
        Task<IEnumerable<AssetResponseDto>> GetAssetsByCategoryAsync(string category);

        Task<AssetResponseDto> CreateAssetAsync(AssetCreateDto assetCreateDto);
        Task<AssetResponseDto?> UpdateAssetAsync(int id, AssetUpdateDto assetUpdateDto);
        Task<bool> DeleteAssetAsync(int id);

        Task<bool> UpdateAssetStatusAsync(int id, AssetStatus newStatus);
        Task<bool> IsAssetAvailableAsync(int id);
    }
}