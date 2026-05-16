using OfficeAssetManager.Core.Domain.Enums;

namespace OfficeAssetManager.Core.DTO;

public class AssetUpdateDto
{
    public string Name { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public AssetStatus Status { get; set; }
}