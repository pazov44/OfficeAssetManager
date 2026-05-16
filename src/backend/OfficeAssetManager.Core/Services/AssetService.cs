using OfficeAssetManager.Core.Domain.Entities;
using OfficeAssetManager.Core.Domain.Enums;
using OfficeAssetManager.Core.DTO;
using OfficeAssetManager.Core.Domain.RepositoryContracts;
using OfficeAssetManager.Core.ServiceContracts;

namespace OfficeAssetManager.Core.Services;

public class AssetService : IAssetService
{
    private readonly IAssetRepository _repository;

    public AssetService(IAssetRepository repository)
    {
        _repository = repository;
    }

    public async Task<AssetResponseDto> CreateAssetAsync(AssetCreateDto dto)
    {
        if (await _repository.AssetTagExistsAsync(dto.AssetTag))
            throw new Exception("Asset tag already exists.");

        var asset = new Asset
        {
            Name = dto.Name,
            AssetTag = dto.AssetTag,
            SerialNumber = dto.SerialNumber,
            Category = dto.Category,
            Description = dto.Description,
            Status = AssetStatus.Available,
            CreatedAt = DateTime.UtcNow
        };

        await _repository.AddAsync(asset);
        await _repository.SaveChangesAsync();

        return MapToDto(asset);
    }

    public async Task<IEnumerable<AssetResponseDto>> GetAllAssetsAsync()
    {
        var assets = await _repository.GetAllAsync();
        return assets.Select(MapToDto);
    }

    public async Task<AssetResponseDto?> GetAssetByIdAsync(int id)
    {
        var asset = await _repository.GetByIdAsync(id);
        return asset == null ? null : MapToDto(asset);
    }

    public async Task<AssetResponseDto?> UpdateAssetAsync(int id, AssetUpdateDto dto)
    {
        var asset = await _repository.GetByIdAsync(id);
        if (asset == null) return null;

        asset.Name = dto.Name;
        asset.Category = dto.Category;
        asset.Description = dto.Description;
        asset.Status = dto.Status;

        _repository.Update(asset);
        await _repository.SaveChangesAsync();

        return MapToDto(asset);
    }

    public async Task<bool> DeleteAssetAsync(int id)
    {
        var asset = await _repository.GetByIdAsync(id);
        if (asset == null) return false;

        _repository.Remove(asset);
        return await _repository.SaveChangesAsync();
    }

    public async Task<bool> IsAssetAvailableAsync(int id)
    {
        var asset = await _repository.GetByIdAsync(id);
        return asset != null && asset.Status == AssetStatus.Available;
    }

    public async Task<bool> UpdateAssetStatusAsync(int id, AssetStatus newStatus)
    {
        var asset = await _repository.GetByIdAsync(id);
        if (asset == null) return false;

        asset.Status = newStatus;
        _repository.Update(asset);
        return await _repository.SaveChangesAsync();
    }

    public async Task<IEnumerable<AssetResponseDto>> GetAssetsByCategoryAsync(string category)
    {
        var assets = await _repository.FindAsync(a => a.Category == category);
        return assets.Select(MapToDto);
    }

    public async Task<IEnumerable<AssetResponseDto>> GetAssetsByStatusAsync(AssetStatus status)
    {
        var assets = await _repository.FindAsync(a => a.Status == status);
        return assets.Select(MapToDto);
    }

    // Manual Mapping Helper
    private static AssetResponseDto MapToDto(Asset asset) => new AssetResponseDto
    {
        Id = asset.Id,
        Name = asset.Name,
        AssetTag = asset.AssetTag,
        SerialNumber = asset.SerialNumber,
        Category = asset.Category,
        Description = asset.Description,
        Status = asset.Status.ToString(),
        CreatedAt = asset.CreatedAt
    };
}