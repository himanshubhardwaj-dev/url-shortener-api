using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using URLShortner.API.Models;

namespace URLShortner.API.Data;

public partial class UrlShortnerDbContext : DbContext
{
    public UrlShortnerDbContext(DbContextOptions<UrlShortnerDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Url> Urls { get; set; }
    public virtual DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasPostgresExtension("citext");

        modelBuilder.Entity<Url>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("URLs_pkey");

            entity.ToTable("URLs");

            entity.HasIndex(e => e.ShortCode, "URLs_Short_Code_key").IsUnique();

            entity.Property(e => e.ClickCount).HasDefaultValue(0);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.IsDeleted)
                .HasDefaultValue(false)
                .HasColumnName("isDeleted");
            entity.Property(e => e.OriginalUrl).HasColumnType("citext");
            entity.Property(e => e.ShortCode)
                .HasMaxLength(10)
                .HasColumnName("Short_Code");

            entity.HasOne(d => d.User).WithMany(u => u.Urls).HasForeignKey(d => d.UserId).OnDelete(DeleteBehavior.Restrict);
        });
        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("Users");
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Email).IsUnique();
            entity.Property(e => e.FirstName).HasMaxLength(100);
            entity.Property(e => e.LastName).HasMaxLength(100);
            entity.Property(e => e.Email).HasMaxLength(255);
            entity.Property(e => e.Role).HasMaxLength(50).HasDefaultValue("User");
            entity.Property(e => e.isActive).HasDefaultValue(true);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
        });
        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
