using Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace Api.Infrastructure;

public partial class ApplicationDbContext : DbContext
{
    public ApplicationDbContext() { }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public virtual DbSet<Card> Cards { get; set; }

    public virtual DbSet<Transaction> Transactions { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Card>(entity =>
        {
            entity.HasKey(e => e.CardId).HasName("PK_Cards");

            entity.Property(e => e.CardId).HasColumnName("CardID");
            entity.Property(e => e.CardNumber).HasMaxLength(50);
            entity.Property(e => e.Pin).HasColumnName("PIN");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.User).WithMany(p => p.Cards)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Cards_Users");
        });

        modelBuilder.Entity<Transaction>(entity =>
        {
            entity.HasKey(e => e.TransactionId).HasName("PK_Transactions");

            entity.Property(e => e.TransactionId).HasColumnName("TransactionID");
            entity.Property(e => e.Amount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.CardId).HasColumnName("CardID");
            entity.Property(e => e.TransactionDate).HasColumnType("datetime");
            entity.Property(e => e.TransactionType).HasMaxLength(50);

            entity.HasOne(d => d.Card).WithMany(p => p.Transactions)
                .HasForeignKey(d => d.CardId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Transactions_Cards");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK_Users");

            entity.Property(e => e.UserId).HasColumnName("UserID");
            entity.Property(e => e.AccountNumber).HasMaxLength(50);
            entity.Property(e => e.CurrentBalance).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.UserName).HasMaxLength(100);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}

//scaffolding:
// Scaffold-DbContext "Server=ANDRE\MSSQLSERVER22;Database=MetafarDB;Trusted_Connection=True;TrustServerCertificate=True;" Microsoft.EntityFrameworkCore.SqlServer -OutputDir Entities