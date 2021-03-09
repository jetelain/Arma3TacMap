using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Arma3TacMapWebApp.Entities;
using System.Text.Json;

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

            modelBuilder.Entity<ReplayMap>().ToTable(nameof(ReplayMap));
            var frame = modelBuilder.Entity<ReplayFrame>();
            frame.Property(e => e.Data).HasConversion(
                    v => JsonSerializer.Serialize(v, null),
                    v => JsonSerializer.Deserialize<ReplayFrameData>(v, null));
            frame.ToTable(nameof(ReplayFrame)).HasKey(t => new { t.ReplayMapID, t.FrameNumber }); 
            modelBuilder.Entity<ReplayPlayer>().ToTable(nameof(ReplayPlayer)).HasKey(t => new { t.ReplayMapID, t.PlayerNumber });
            modelBuilder.Entity<ReplayGroup>().ToTable(nameof(ReplayGroup)).HasKey(t => new { t.ReplayMapID, t.GroupNumber });
            modelBuilder.Entity<ReplayVehicle>().ToTable(nameof(ReplayVehicle)).HasKey(t => new { t.ReplayMapID, t.VehicleNumber });
            modelBuilder.Entity<ReplayUnit>().ToTable(nameof(ReplayUnit)).HasKey(t => new { t.ReplayMapID, t.UnitNumber });
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

        public DbSet<Arma3TacMapWebApp.Entities.ReplayMap> ReplayMap { get; set; }
    }
}
