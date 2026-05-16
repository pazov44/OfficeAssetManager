namespace OfficeAssetManager.Core.DTO;

public class ReservationResponseDto
{
    public int Id { get; set; }
    public string AssetName { get; set; } = string.Empty;
    public string UserEmail { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string Status { get; set; } = string.Empty;
}