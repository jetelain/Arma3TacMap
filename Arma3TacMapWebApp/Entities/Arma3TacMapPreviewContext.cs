using Microsoft.EntityFrameworkCore;

namespace Arma3TacMapWebApp.Entities
{
    public class Arma3TacMapPreviewContext : DbContext
    {
        public Arma3TacMapPreviewContext(DbContextOptions<Arma3TacMapPreviewContext> options)
            : base(options)
        {
        }

        public DbSet<TacMapPreview> TacMapPreviews { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var tacMapPreview = modelBuilder.Entity<TacMapPreview>();
            tacMapPreview.HasKey(m => new { m.TacMapID, m.Size });
            tacMapPreview.ToTable(nameof(TacMapPreview));
        }
    }
}
