using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace Arma3TacMapWebApp.Entities
{
    public class Arma3TacMapContext : DbContext
    {
        public Arma3TacMapContext(DbContextOptions<Arma3TacMapContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }

        public DbSet<TacMap> TacMaps { get; set; }

        public DbSet<TacMapAccess> TacMapAccesses { get; set; }

        public DbSet<TacMapMarker> TacMapMarkers { get; set; }

        public DbSet<UserApiKey> UserApiKeys { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().ToTable(nameof(User));

            modelBuilder.Entity<TacMap>().ToTable(nameof(TacMap));

            modelBuilder.Entity<TacMapAccess>().ToTable(nameof(TacMapAccess));

            modelBuilder.Entity<TacMapMarker>().ToTable(nameof(TacMapMarker));

            var userApiKey = modelBuilder.Entity<UserApiKey>();
            userApiKey.ToTable(nameof(UserApiKey));
        }

        internal void UpgradeData()
        {
            foreach(var marker in TacMapMarkers.Where(m => m.LastUpdate == null))
            {
                marker.LastUpdate = DateTime.UtcNow;
                Update(marker);
            }
            SaveChanges();
        }
    }
}
