using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace PW3.Emoji.Entidades.EF;

public partial class Pw320252cEmojiContext : DbContext
{
    public Pw320252cEmojiContext()
    {
    }

    public Pw320252cEmojiContext(DbContextOptions<Pw320252cEmojiContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Emocion> Emocions { get; set; }

    public virtual DbSet<Emoji> Emojis { get; set; }

    public virtual DbSet<Imagen> Imagens { get; set; }

    public virtual DbSet<MapeoEmocionEmoji> MapeoEmocionEmojis { get; set; }

    public virtual DbSet<ResultadoAnalisi> ResultadoAnalises { get; set; }

    public virtual DbSet<Rol> Rols { get; set; }

    public virtual DbSet<Usuario> Usuarios { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=JIMENA\\SQLEXPRESS;Database=PW3-2025-2c-Emoji;Trusted_Connection=True;TrustServerCertificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Emocion>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Emocion__3214EC07984C394F");

            entity.ToTable("Emocion");

            entity.HasIndex(e => e.Nombre, "UQ_Emocion_Nombre").IsUnique();

            entity.Property(e => e.Descripcion).HasMaxLength(100);
            entity.Property(e => e.Nombre).HasMaxLength(50);
        });

        modelBuilder.Entity<Emoji>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Emoji__3214EC070F60735E");

            entity.ToTable("Emoji");

            entity.Property(e => e.Alias).HasMaxLength(50);
            entity.Property(e => e.CodigoUnicode).HasMaxLength(10);
        });

        modelBuilder.Entity<Imagen>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Imagen__3214EC07F637D549");

            entity.ToTable("Imagen");

            entity.HasIndex(e => e.UsuarioId, "IX_Imagen_UsuarioId");

            entity.Property(e => e.FechaSubida).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.Ruta).HasMaxLength(400);

            entity.HasOne(d => d.Usuario).WithMany(p => p.Imagens)
                .HasForeignKey(d => d.UsuarioId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Imagen_Usuario");
        });

        modelBuilder.Entity<MapeoEmocionEmoji>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__MapeoEmo__3214EC0727732255");

            entity.ToTable("MapeoEmocionEmoji");

            entity.HasIndex(e => e.EmocionId, "UQ_Mapeo_EmocionId").IsUnique();

            entity.HasOne(d => d.Emocion).WithOne(p => p.MapeoEmocionEmoji)
                .HasForeignKey<MapeoEmocionEmoji>(d => d.EmocionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Mapeo_Emocion");

            entity.HasOne(d => d.Emoji).WithMany(p => p.MapeoEmocionEmojis)
                .HasForeignKey(d => d.EmojiId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Mapeo_Emoji");
        });

        modelBuilder.Entity<ResultadoAnalisi>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Resultad__3214EC075D5001F8");

            entity.ToTable("ResultadoAnalisis");

            entity.HasIndex(e => e.EmocionId, "IX_Resultado_EmocionId");

            entity.HasIndex(e => e.ImagenId, "IX_Resultado_ImagenId");

            entity.Property(e => e.FechaAnalisis).HasDefaultValueSql("(sysutcdatetime())");

            entity.HasOne(d => d.Emocion).WithMany(p => p.ResultadoAnalisis)
                .HasForeignKey(d => d.EmocionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Resultado_Emocion");

            entity.HasOne(d => d.Imagen).WithMany(p => p.ResultadoAnalisis)
                .HasForeignKey(d => d.ImagenId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Resultado_Imagen");
        });

        modelBuilder.Entity<Rol>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Rol__3214EC07F9863A36");

            entity.ToTable("Rol");

            entity.Property(e => e.Nombre).HasMaxLength(50);
        });

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Usuario__3214EC07AB39AA3A");

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
