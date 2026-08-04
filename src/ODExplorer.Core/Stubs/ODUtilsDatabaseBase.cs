// ODUtils.Database.Base.ODDbContextBase — functional base for ODExplorerDbContext.
// Mirrors the real ODUtils base so the real provider persists commanders,
// journal entries and settings through EF Core.

using Microsoft.EntityFrameworkCore;
using ODUtils.Database.DTOs;

namespace ODUtils.Database.Base
{
    public abstract class ODDbContextBase(DbContextOptions options) : DbContext(options)
    {
        public DbSet<JournalCommanderDTO> JournalCommanders { get; set; } = null!;
        public DbSet<JournalEntryDTO> JournalEntries { get; set; } = null!;
        public DbSet<SettingsDTO> Settings { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            OnEfCoreModelCreating(modelBuilder);
        }

        public virtual void OnEfCoreModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<JournalCommanderDTO>(b =>
            {
                b.HasKey(x => x.Id);
                b.Property(x => x.Id).ValueGeneratedOnAdd();
                b.HasIndex(x => x.Name).IsUnique();
            });

            modelBuilder.Entity<JournalEntryDTO>(b =>
            {
                b.HasKey(x => new { x.Filename, x.Offset });
                b.HasIndex(x => x.EventTypeId);
                b.HasIndex(x => new { x.TimeStamp, x.Offset });
            });

            modelBuilder.Entity<SettingsDTO>(b =>
            {
                b.HasKey(x => x.Id);
            });
        }
    }
}
