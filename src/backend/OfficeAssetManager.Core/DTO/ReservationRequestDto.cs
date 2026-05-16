namespace OfficeAssetManager.Core.DTO;

public class ReservationRequestDto
{
    public int AssetId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
}