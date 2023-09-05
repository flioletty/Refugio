using System;
using Microsoft.EntityFrameworkCore;
using DBModels;

namespace DBCore
{
    public class VKContext : DbContext
    {
        public VKContext(DbContextOptions<VKContext> options) : base(options)
        {
            Database.EnsureCreated();
        }

        public DbSet<User> Users { get; set; }

        public DbSet<Group> Groups { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("Refugio");

            modelBuilder.Entity<User>().HasKey(x => x.Id);

            modelBuilder.Entity<Group>().HasKey(z => z.Id);

            modelBuilder.Entity<User>()
                        .HasMany(p => p.Groups)
                        .WithMany(c => c.Users)
                        .UsingEntity(j => j.ToTable("Connection"));

            base.OnModelCreating(modelBuilder);
        }
    }
}