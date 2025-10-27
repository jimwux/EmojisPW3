using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace PW3.Emoji.Entidades.EF;

public partial class PW3_EmojiContext : DbContext
{
    public PW3_EmojiContext(DbContextOptions<PW3_EmojiContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AnalisisResultado> AnalisisResultado { get; set; }

    public virtual DbSet<Emocion> Emocion { get; set; }

    public virtual DbSet<Emoji> Emoji { get; set; }

    public virtual DbSet<Imagen> Imagen { get; set; }

    public virtual DbSet<MapeoEmocionEmoji> MapeoEmocionEmoji { get; set; }

    public virtual DbSet<Rol> Rol { get; set; }

    public virtual DbSet<Usuario> Usuario { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AnalisisResultado>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Analisis__3214EC07E3FD976B");

            entity.HasIndex(e => e.EmocionId, "IX_Analisis_EmocionId");

            entity.HasIndex(e => e.ImagenId, "IX_Analisis_ImagenId");

            entity.HasIndex(e => e.UsuarioId, "IX_Analisis_UsuarioId");

            entity.Property(e => e.FechaAnalisis).HasDefaultValueSql("(sysutcdatetime())");

            entity.HasOne(d => d.Emocion).WithMany(p => p.AnalisisResultado)
                .HasForeignKey(d => d.EmocionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Analisis_Emocion");

            entity.HasOne(d => d.Imagen).WithMany(p => p.AnalisisResultado)
                .HasForeignKey(d => d.ImagenId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Analisis_Imagen");

            entity.HasOne(d => d.Usuario).WithMany(p => p.AnalisisResultado)
                .HasForeignKey(d => d.UsuarioId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Analisis_Usuario");
        });

        modelBuilder.Entity<Emocion>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Emocion__3214EC0709188CAD");

            entity.HasIndex(e => e.Nombre, "UQ_Emocion_Nombre").IsUnique();

            entity.Property(e => e.Descripcion).HasMaxLength(100);
            entity.Property(e => e.Nombre).HasMaxLength(50);
        });

        modelBuilder.Entity<Emoji>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Emoji__3214EC07EBBB0A7D");

            entity.Property(e => e.Alias).HasMaxLength(50);
            entity.Property(e => e.CodigoUnicode).HasMaxLength(10);
        });

        modelBuilder.Entity<Imagen>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Imagen__3214EC0739CCA6A4");

            entity.HasIndex(e => e.UsuarioId, "IX_Imagen_UsuarioId");

            entity.Property(e => e.FechaSubida).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.Ruta).HasMaxLength(400);

            entity.HasOne(d => d.Usuario).WithMany(p => p.Imagen)
                .HasForeignKey(d => d.UsuarioId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Imagen_Usuario");
        });

        modelBuilder.Entity<MapeoEmocionEmoji>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__MapeoEmo__3214EC075C79F2E8");

            entity.HasIndex(e => e.EmocionId, "UQ_Mapeo_EmocionId").IsUnique();

            entity.HasOne(d => d.Emocion).WithOne(p => p.MapeoEmocionEmoji)
                .HasForeignKey<MapeoEmocionEmoji>(d => d.EmocionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Mapeo_Emocion");

            entity.HasOne(d => d.Emoji).WithMany(p => p.MapeoEmocionEmoji)
                .HasForeignKey(d => d.EmojiId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Mapeo_Emoji");
        });

        modelBuilder.Entity<Rol>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Rol__3214EC077CE478FD");

            entity.Property(e => e.Nombre).HasMaxLength(50);
        });

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Usuario__3214EC07EBC80A05");

            entity.HasIndex(e => e.Email, "UQ_Usuario_Email").IsUnique();

            entity.Property(e => e.Email).HasMaxLength(256);
            entity.Property(e => e.HashPassword).HasMaxLength(256);
            entity.Property(e => e.Nombre).HasMaxLength(100);

            entity.HasOne(d => d.Rol).WithMany(p => p.Usuario)
                .HasForeignKey(d => d.RolId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Usuario_Rol");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
