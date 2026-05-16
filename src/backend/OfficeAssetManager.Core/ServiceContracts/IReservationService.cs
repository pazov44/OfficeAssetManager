using OfficeAssetManager.Core.DTO;

namespace OfficeAssetManager.Core.ServiceContracts;

public interface IReservationService
{
    Task<ReservationResponseDto> CreateReservationAsync(int userId, ReservationRequestDto dto);
    Task<IEnumerable<ReservationResponseDto>> GetUserReservationsAsync(int userId);
    Task<IEnumerable<ReservationResponseDto>> GetAllReservationsAsync();
    Task<bool> CancelReservationAsync(int reservationId, int userId);
    Task<bool> UpdateStatusAsync(int reservationId, string status); // For Admins
}