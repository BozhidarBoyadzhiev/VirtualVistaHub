using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace VirtualVistaHub.Models;

public partial class VirtualVistaBaseContext : DbContext
{
    public VirtualVistaBaseContext()
    {
    }

    public VirtualVistaBaseContext(DbContextOptions<VirtualVistaBaseContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Propety> Propeties { get; set; }

    public virtual DbSet<Staff> Staff { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Initial Catalog=VirtualVistaBase;Integrated Security=True;Encrypt=True;Trust Server Certificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Propety>(entity =>
        {
            entity.HasKey(e => e.PropertyId).HasName("PK__Propetie__70C9A7354D425250");

            entity.Property(e => e.AdditionalInformation)
                .HasMaxLength(2500)
                .IsUnicode(false);
            entity.Property(e => e.ApprovalStatus)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Deleted).HasDefaultValueSql("((0))");
            entity.Property(e => e.District)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Sold).HasDefaultValueSql("((0))");
            entity.Property(e => e.TypeOfContrusction)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.TypeOfProperty)
                .HasMaxLength(70)
                .IsUnicode(false);

            entity.HasOne(d => d.User).WithMany(p => p.Propeties)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__Propeties__UserI__3B75D760");
        });

        modelBuilder.Entity<Staff>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Staff__1788CC4CE2A0D75A");

            entity.Property(e => e.UserId).ValueGeneratedNever();
            entity.Property(e => e.UserLevel)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.User).WithOne(p => p.Staff)
                .HasForeignKey<Staff>(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Staff__UserId__3E52440B");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Users__1788CC4C1A5F8FFE");

            entity.Property(e => e.Deleted).HasDefaultValueSql("((0))");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.FirstName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.LastName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Password)
                .HasMaxLength(100)
                .IsUnicode(false);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
