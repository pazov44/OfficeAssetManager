using Microsoft.AspNetCore.Identity;

namespace OfficeAssetManager.Core.Domain.Entities
{

    public class ApplicationUser : IdentityUser<int>
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();

        public string? RefreshToken { get; set; }
        public DateTime RefreshTokenExpiryTime { get; set; }
    }
}