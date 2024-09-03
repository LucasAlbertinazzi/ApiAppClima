using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace ApiAppClima.Models;

public partial class PostgresContext : DbContext
{
    public PostgresContext()
    {
    }

    public PostgresContext(DbContextOptions<PostgresContext> options)
        : base(options)
    {
    }

    public virtual DbSet<TblHistClima> TblHistClimas { get; set; }

    public virtual DbSet<TblUsuario> TblUsuarios { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasPostgresExtension("pg_catalog", "adminpack");

        modelBuilder.Entity<TblHistClima>(entity =>
        {
            entity.HasKey(e => e.Idhist).HasName("tbl_hist_clima_pkey");

            entity.ToTable("tbl_hist_clima");

            entity.Property(e => e.Idhist).HasColumnName("idhist");
            entity.Property(e => e.Cidade)
                .HasColumnType("character varying")
                .HasColumnName("cidade");
            entity.Property(e => e.Coduser).HasColumnName("coduser");
            entity.Property(e => e.Descricao)
                .HasColumnType("character varying")
                .HasColumnName("descricao");
            entity.Property(e => e.Temperatura)
                .HasColumnType("character varying")
                .HasColumnName("temperatura");
        });

        modelBuilder.Entity<TblUsuario>(entity =>
        {
            entity.HasKey(e => e.Codusuario).HasName("tbl_usuarios_pkey");

            entity.ToTable("tbl_usuarios");

            entity.Property(e => e.Codusuario).HasColumnName("codusuario");
            entity.Property(e => e.Nome)
                .HasColumnType("character varying")
                .HasColumnName("nome");
            entity.Property(e => e.Senha).HasColumnName("senha");
            entity.Property(e => e.Status)
                .HasDefaultValue(true)
                .HasColumnName("status");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
