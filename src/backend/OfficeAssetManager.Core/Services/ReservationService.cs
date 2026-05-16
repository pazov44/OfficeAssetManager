using OfficeAssetManager.Core.Domain.Entities;
using OfficeAssetManager.Core.Domain.Enums;
using OfficeAssetManager.Core.DTO;
using OfficeAssetManager.Core.Domain.RepositoryContracts;
using OfficeAssetManager.Core.ServiceContracts;

namespace OfficeAssetManager.Core.Services;

public class ReservationService : IReservationService
{
    private readonly IReservationRepository _reservationRepo;
    private readonly IAssetRepository _assetRepo;

    public ReservationService(IReservationRepository reservationRepo, IAssetRepository assetRepo)
    {
        _reservationRepo = reservationRepo;
        _assetRepo = assetRepo;
    }

    public async Task<ReservationResponseDto> CreateReservationAsync(int userId, ReservationRequestDto dto)
    {
        // Check if Asset exists
        var asset = await _assetRepo.GetByIdAsync(dto.AssetId);
        if (asset == null) throw new Exception("Asset not found.");

        // Check for overlapping dates
        if (await _reservationRepo.HasOverlappingReservationAsync(dto.AssetId, dto.StartDate, dto.EndDate))
            throw new Exception("Asset is already reserved for these dates.");

        // Create Entity
        var reservation = new Reservation
        {
            UserId = userId,
            AssetId = dto.AssetId,
            StartDate = dto.StartDate,
            EndDate = dto.EndDate,
            Status = ReservationStatus.Pending
        };

        await _reservationRepo.AddAsync(reservation);
        await _reservationRepo.SaveChangesAsync();

        // Load the asset details for the response
        return MapToDto(reservation, asset.Name, "");
    }

    public async Task<IEnumerable<ReservationResponseDto>> GetUserReservationsAsync(int userId)
    {
        var results = await _reservationRepo.GetByUserIdAsync(userId);
        return results.Select(r => MapToDto(r, r.Asset?.Name ?? "Unknown", r.User?.Email ?? ""));
    }

    public async Task<IEnumerable<ReservationResponseDto>> GetAllReservationsAsync()
    {
        var results = await _reservationRepo.GetAllAsync();
        return results.Select(r => MapToDto(r, r.Asset?.Name ?? "Unknown", r.User?.Email ?? ""));
    }

    public async Task<bool> CancelReservationAsync(int reservationId, int userId)
    {
        var reservation = await _reservationRepo.GetByIdAsync(reservationId);
        if (reservation == null || reservation.UserId != userId) return false;

        reservation.Status = ReservationStatus.Cancelled;
        _reservationRepo.Update(reservation);
        return await _reservationRepo.SaveChangesAsync();
    }

    public async Task<bool> UpdateStatusAsync(int reservationId, string status)
    {
        var reservation = await _reservationRepo.GetByIdAsync(reservationId);
        if (reservation == null) return false;

        if (Enum.TryParse<ReservationStatus>(status, true, out var newStatus))
        {
            reservation.Status = newStatus;
            _reservationRepo.Update(reservation);
            return await _reservationRepo.SaveChangesAsync();
        }
        return false;
    }

    private static ReservationResponseDto MapToDto(Reservation r, string assetName, string userEmail) => new()
    {
        Id = r.Id,
        AssetName = assetName,
        UserEmail = userEmail,
        StartDate = r.StartDate,
        EndDate = r.EndDate,
        Status = r.Status.ToString()
    };
}