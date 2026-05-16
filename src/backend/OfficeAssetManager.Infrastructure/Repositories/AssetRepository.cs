using Microsoft.EntityFrameworkCore;
using OfficeAssetManager.Core.Domain.Entities;
using OfficeAssetManager.Core.Domain.RepositoryContracts;
using OfficeAssetManager.Core.ServiceContracts;
using OfficeAssetManager.Infrastructure.DbContext;
using System.Linq.Expressions;

namespace OfficeAssetManager.Infrastructure.Repositories;

public class AssetRepository : IAssetRepository
{
    private readonly AppDbContext _context;
    public AssetRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Asset?> GetByAssetTagAsync(string assetTag)
    {
        return await _context.Assets.FirstOrDefaultAsync(a => a.AssetTag == assetTag);
    }

    public async Task<bool> AssetTagExistsAsync(string assetTag)
    {
        return await _context.Assets.AnyAsync(a => a.AssetTag == assetTag);
    }

    public async Task<IEnumerable<Asset>> GetAllAsync()
    {
        return await _context.Assets
            .ToListAsync();
    }

    public async Task<Asset?> GetByIdAsync(int id)
    {
        return await _context.Assets.FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task<IEnumerable<Asset>> FindAsync(Expression<Func<Asset, bool>> predicate)
    {
        return await _context.Assets
            .Where(predicate)
            .ToListAsync();
    }

    public async Task AddAsync(Asset entity)
    {
        await _context.Assets.AddAsync(entity);
    }

    public void Update(Asset entity)
    {
        _context.Assets.Update(entity);
    }

    public void Remove(Asset entity)
    {
        _context.Assets.Remove(entity);
    }

    public async Task<bool> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync() > 0;
    }
}