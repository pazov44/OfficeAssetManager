using Microsoft.EntityFrameworkCore;
using OfficeAssetManager.Core.Domain.Entities;
using OfficeAssetManager.Core.Domain.Enums;
using OfficeAssetManager.Core.Domain.RepositoryContracts;
using OfficeAssetManager.Core.ServiceContracts;
using OfficeAssetManager.Infrastructure.DbContext;
using System.Linq.Expressions;

namespace OfficeAssetManager.Infrastructure.Repositories;

public class ReservationRepository : IReservationRepository
{
    private readonly AppDbContext _context;

    public ReservationRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Reservation>> GetAllAsync()
    {
        return await _context.Reservations
            .Include(r => r.Asset)
            .Include(r => r.User)
            .ToListAsync();
    }

    public async Task<Reservation?> GetByIdAsync(int id)
    {
        return await _context.Reservations
            .Include(r => r.Asset)
            .Include(r => r.User)
            .FirstOrDefaultAsync(r => r.Id == id);
    }

    public async Task<IEnumerable<Reservation>> FindAsync(Expression<Func<Reservation, bool>> predicate)
    {
        return await _context.Reservations
            .Include(r => r.Asset)
            .Include(r => r.User)
            .Where(predicate)
            .ToListAsync();
    }

    public async Task<IEnumerable<Reservation>> GetByUserIdAsync(int userId)
    {
        return await _context.Reservations
            .Include(r => r.Asset)
            .Where(r => r.UserId == userId)
            .ToListAsync();
    }

    public async Task<IEnumerable<Reservation>> GetByAssetIdAsync(int assetId)
    {
        return await _context.Reservations
            .Include(r => r.User)
            .Where(r => r.AssetId == assetId)
            .ToListAsync();
    }

    public async Task<IEnumerable<Reservation>> GetActiveReservationsAsync()
    {
        return await _context.Reservations
            .Include(r => r.Asset)
            .Include(r => r.User)
            .Where(r => r.Status == ReservationStatus.Approved || r.Status == ReservationStatus.Pending)
            .ToListAsync();
    }

    public async Task<bool> HasOverlappingReservationAsync(int assetId, DateTime start, DateTime end)
    {
        return await _context.Reservations
            .AnyAsync(r => r.AssetId == assetId &&
                           r.Status != ReservationStatus.Cancelled &&
                           r.Status != ReservationStatus.Rejected &&
                           ((start >= r.StartDate && start <= r.EndDate) ||
                            (end >= r.StartDate && end <= r.EndDate)));
    }

    public async Task AddAsync(Reservation entity)
    {
        await _context.Reservations.AddAsync(entity);
    }

    public void Update(Reservation entity)
    {
        _context.Reservations.Update(entity);
    }

    public void Remove(Reservation entity)
    {
        _context.Reservations.Remove(entity);
    }

    public async Task<bool> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync() > 0;
    }
}