using System.ComponentModel.DataAnnotations;

namespace OfficeAssetManager.Core.DTO;

public class AssetCreateDto
{
    [Required]
    public string Name { get; set; } = string.Empty;
    [Required]
    public string AssetTag { get; set; } = string.Empty;
    [Required]
    public string SerialNumber { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}