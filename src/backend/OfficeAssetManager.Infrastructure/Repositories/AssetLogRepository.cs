using Microsoft.EntityFrameworkCore;
using OfficeAssetManager.Core.Domain.Entities;
using OfficeAssetManager.Core.Domain.RepositoryContracts;
using OfficeAssetManager.Core.ServiceContracts;
using OfficeAssetManager.Infrastructure.DbContext;
using System.Linq.Expressions;

namespace OfficeAssetManager.Infrastructure.Repositories;

public class AssetLogRepository : IAssetLogRepository
{
    private readonly AppDbContext _context;

    public AssetLogRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(AssetLog entity)
    {
        await _context.AssetLogs.AddAsync(entity);
    }

    public async Task<IEnumerable<AssetLog>> GetAllAsync()
    {
        return await _context.AssetLogs
            .OrderByDescending(l => l.CreatedAt)
            .ToListAsync();
    }

    public async Task<AssetLog?> GetByIdAsync(int id)
    {
        return await _context.AssetLogs.FindAsync(id);
    }

    public async Task<IEnumerable<AssetLog>> FindAsync(Expression<Func<AssetLog, bool>> predicate)
    {
        return await _context.AssetLogs.Where(predicate).ToListAsync();
    }

    public async Task<IEnumerable<AssetLog>> GetLogsByAssetIdAsync(int assetId)
    {
        return await _context.AssetLogs
            .Where(l => l.AssetId == assetId)
            .OrderByDescending(l => l.CreatedAt)
            .ToListAsync();
    }

    public void Update(AssetLog entity)
    {
        _context.AssetLogs.Update(entity);
    }

    public void Remove(AssetLog entity)
    {
        _context.AssetLogs.Remove(entity);
    }

    public async Task<bool> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync() > 0;
    }
}