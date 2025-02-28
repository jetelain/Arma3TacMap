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

        public DbSet<Orbat> Orbats { get; set; }

        public DbSet<OrbatUnit> OrbatUnits { get; set; }

        public DbSet<UserSymbolBookmark> SymbolBookmarks { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().ToTable(nameof(User));

            var tacMap = modelBuilder.Entity<TacMap>();
            tacMap.HasOne(t => t.HostileOrbat).WithMany().OnDelete(DeleteBehavior.SetNull);
            tacMap.HasOne(t => t.FriendlyOrbat).WithMany().OnDelete(DeleteBehavior.SetNull);
            tacMap.HasOne(t => t.Parent).WithMany().OnDelete(DeleteBehavior.Cascade);
            tacMap.Property(t => t.GameName).HasDefaultValue("arma3");
            tacMap.Property(t => t.Order).HasDefaultValue(0);
            tacMap.ToTable(nameof(TacMap));

            modelBuilder.Entity<TacMapAccess>().ToTable(nameof(TacMapAccess));

            modelBuilder.Entity<TacMapMarker>().ToTable(nameof(TacMapMarker));

            modelBuilder.Entity<Orbat>().ToTable(nameof(Orbat));

            var orbatUnit = modelBuilder.Entity<OrbatUnit>();
            orbatUnit.Property(t => t.NatoSymbolSet).HasDefaultValue("10");
            orbatUnit.ToTable(nameof(OrbatUnit));

            var userApiKey = modelBuilder.Entity<UserApiKey>();
            userApiKey.ToTable(nameof(UserApiKey));

            // Message templates
            modelBuilder.Entity<MessageTemplate>().ToTable(nameof(MessageTemplate));

            var line = modelBuilder.Entity<MessageLineTemplate>();
            line.HasOne(t => t.MessageTemplate).WithMany(t => t.Lines).OnDelete(DeleteBehavior.Cascade);
            line.ToTable(nameof(MessageLineTemplate));

            var field = modelBuilder.Entity<MessageFieldTemplate>();
            field.HasOne(t => t.MessageLineTemplate).WithMany(t => t.Fields).OnDelete(DeleteBehavior.Cascade);
            field.ToTable(nameof(MessageFieldTemplate));

            modelBuilder.Entity<UserSymbolBookmark>().ToTable(nameof(UserSymbolBookmark));

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
        public DbSet<Arma3TacMapWebApp.Entities.MessageTemplate> MessageTemplate { get; set; } = default!;
        public DbSet<Arma3TacMapWebApp.Entities.MessageLineTemplate> MessageLineTemplate { get; set; } = default!;
        public DbSet<Arma3TacMapWebApp.Entities.MessageFieldTemplate> MessageFieldTemplate { get; set; } = default!;
    }
}
