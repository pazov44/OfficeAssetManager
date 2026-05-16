namespace OfficeAssetManager.Core.DTO;

public class AssetLogResponseDto
{
    public int Id { get; set; }
    public int AssetId { get; set; }
    public string Action { get; set; } = string.Empty; // e.g., "Created", "Reserved", "Returned"
    public string Details { get; set; } = string.Empty; // e.g., "Status changed from Available to Reserved"
    public string PerformedBy { get; set; } = string.Empty; // User email or "System"
    public DateTime CreatedAt { get; set; }
}