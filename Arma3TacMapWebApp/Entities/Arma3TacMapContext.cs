using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Arma3TacMapWebApp.Entities;

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

        public DbSet<Orbat> Orbats { get; set; }

        public DbSet<OrbatUnit> OrbatUnits { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().ToTable(nameof(User));

            var tacMap = modelBuilder.Entity<TacMap>();
            tacMap.HasOne(t => t.HostileOrbat).WithMany().OnDelete(DeleteBehavior.SetNull);
            tacMap.HasOne(t => t.FriendlyOrbat).WithMany().OnDelete(DeleteBehavior.SetNull);
            tacMap.ToTable(nameof(TacMap));

            modelBuilder.Entity<TacMapAccess>().ToTable(nameof(TacMapAccess));

            modelBuilder.Entity<TacMapMarker>().ToTable(nameof(TacMapMarker));

            modelBuilder.Entity<Orbat>().ToTable(nameof(Orbat));

            modelBuilder.Entity<OrbatUnit>().ToTable(nameof(OrbatUnit));

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
