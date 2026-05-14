using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using OfficeAssetManager.Core.Domain.Entities;

namespace OfficeAssetManager.Infrastructure.Data
{

    public class AppDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, int>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Asset> Assets => Set<Asset>();
        public DbSet<Reservation> Reservations => Set<Reservation>();
        public DbSet<AssetLog> AssetLogs => Set<AssetLog>();

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder); // Mandatory for Identity tables

            builder.Entity<Asset>(entity =>
            {
                entity.HasIndex(a => a.AssetTag).IsUnique();
                entity.Property(a => a.Name).IsRequired().HasMaxLength(100);
                entity.Property(a => a.AssetTag).IsRequired().HasMaxLength(50);
            });

            builder.Entity<Reservation>(entity =>
            {
                entity.HasOne(r => r.User)
                    .WithMany(u => u.Reservations)
                    .HasForeignKey(r => r.UserId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(r => r.Asset)
                    .WithMany(a => a.Reservations)
                    .HasForeignKey(r => r.AssetId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            builder.Entity<AssetLog>(entity =>
            {
                entity.HasOne(al => al.Asset)
                    .WithMany()
                    .HasForeignKey(al => al.AssetId);

                entity.Property(al => al.Action).IsRequired().HasMaxLength(100);
            });
        }
    }

}
