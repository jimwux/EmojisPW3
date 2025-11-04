using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace PW3.Emoji.Entidades.EF;

public partial class PW3_EmojiContext : DbContext
{
    public PW3_EmojiContext()
    {
    }

    public PW3_EmojiContext(DbContextOptions<PW3_EmojiContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AnalisisResultado> AnalisisResultados { get; set; }

    public virtual DbSet<Emocion> Emocion { get; set; }

    public virtual DbSet<Imagen> Imagen { get; set; }

    public virtual DbSet<Rol> Rol { get; set; }

    public virtual DbSet<Usuario> Usuario { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=.\\SQLEXPRESS;Database=PW3_Emoji;Trusted_Connection=True;TrustServerCertificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AnalisisResultado>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Analisis__3214EC07E3FD976B");

            entity.ToTable("AnalisisResultado");

            entity.HasIndex(e => e.EmocionId, "IX_Analisis_EmocionId");

            entity.HasIndex(e => e.ImagenId, "IX_Analisis_ImagenId");

            entity.HasIndex(e => e.UsuarioId, "IX_Analisis_UsuarioId");

            entity.Property(e => e.FechaAnalisis).HasDefaultValueSql("(sysutcdatetime())");

            entity.HasOne(d => d.Emocion).WithMany(p => p.AnalisisResultados)
                .HasForeignKey(d => d.EmocionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Analisis_Emocion");

            entity.HasOne(d => d.Imagen).WithMany(p => p.AnalisisResultados)
                .HasForeignKey(d => d.ImagenId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Analisis_Imagen");

            entity.HasOne(d => d.Usuario).WithMany(p => p.AnalisisResultados)
                .HasForeignKey(d => d.UsuarioId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Analisis_Usuario");
        });

        modelBuilder.Entity<Emocion>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Emocion__3214EC0709188CAD");

            entity.ToTable("Emocion");

            entity.HasIndex(e => e.Nombre, "UQ_Emocion_Nombre").IsUnique();

            entity.Property(e => e.Descripcion).HasMaxLength(100);
            entity.Property(e => e.Nombre).HasMaxLength(50);
        });

        modelBuilder.Entity<Imagen>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Imagen__3214EC0739CCA6A4");

            entity.ToTable("Imagen");

            entity.HasIndex(e => e.UsuarioId, "IX_Imagen_UsuarioId");

            entity.Property(e => e.FechaSubida).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.Ruta).HasMaxLength(400);

            entity.HasOne(d => d.Usuario).WithMany(p => p.Imagens)
                .HasForeignKey(d => d.UsuarioId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Imagen_Usuario");
        });

        modelBuilder.Entity<Rol>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Rol__3214EC077CE478FD");

            entity.ToTable("Rol");

            entity.Property(e => e.Nombre).HasMaxLength(50);
        });

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Usuario__3214EC07EBC80A05");

            entity.ToTable("Usuario");

            entity.HasIndex(e => e.Email, "UQ_Usuario_Email").IsUnique();

            entity.Property(e => e.Email).HasMaxLength(256);
            entity.Property(e => e.HashPassword).HasMaxLength(256);
            entity.Property(e => e.Nombre).HasMaxLength(100);

            entity.HasOne(d => d.Rol).WithMany(p => p.Usuarios)
                .HasForeignKey(d => d.RolId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Usuario_Rol");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
