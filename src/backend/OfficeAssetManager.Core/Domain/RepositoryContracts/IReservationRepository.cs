using OfficeAssetManager.Core.Domain.Entities;

namespace OfficeAssetManager.Core.Domain.RepositoryContracts;

public interface IReservationRepository : IRepository<Reservation>
{
    Task<IEnumerable<Reservation>> GetByUserIdAsync(int userId);
    Task<IEnumerable<Reservation>> GetByAssetIdAsync(int assetId);
    Task<IEnumerable<Reservation>> GetActiveReservationsAsync();
    Task<bool> HasOverlappingReservationAsync(int assetId, DateTime start, DateTime end);
}