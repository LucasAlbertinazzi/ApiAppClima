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
        modelBuilder.Entity<TblHistClima>(entity =>
        {
            entity.HasKey(e => e.Idhist).HasName("tbl_hist_clima_pkey");

            entity.ToTable("tbl_hist_clima");

            entity.Property(e => e.Idhist).HasColumnName("idhist");
            entity.Property(e => e.Cidade)
                .HasMaxLength(255)
                .HasColumnName("cidade");
            entity.Property(e => e.Coduser).HasColumnName("coduser");
            entity.Property(e => e.DataHora).HasColumnName("data_hora");
            entity.Property(e => e.Descricao).HasColumnName("descricao");
            entity.Property(e => e.DirecaoVento)
                .HasMaxLength(50)
                .HasColumnName("direcao_vento");
            entity.Property(e => e.Latitude).HasColumnName("latitude");
            entity.Property(e => e.Longitude).HasColumnName("longitude");
            entity.Property(e => e.NascerDoSol).HasColumnName("nascer_do_sol");
            entity.Property(e => e.Nuvens)
                .HasMaxLength(50)
                .HasColumnName("nuvens");
            entity.Property(e => e.PorDoSol).HasColumnName("por_do_sol");
            entity.Property(e => e.Pressao)
                .HasMaxLength(50)
                .HasColumnName("pressao");
            entity.Property(e => e.Temperatura)
                .HasMaxLength(50)
                .HasColumnName("temperatura");
            entity.Property(e => e.TemperaturaMaxima)
                .HasMaxLength(50)
                .HasColumnName("temperatura_maxima");
            entity.Property(e => e.TemperaturaMinima)
                .HasMaxLength(50)
                .HasColumnName("temperatura_minima");
            entity.Property(e => e.Umidade)
                .HasMaxLength(50)
                .HasColumnName("umidade");
            entity.Property(e => e.VelocidadeVento)
                .HasMaxLength(50)
                .HasColumnName("velocidade_vento");
            entity.Property(e => e.Visibilidade)
                .HasMaxLength(50)
                .HasColumnName("visibilidade");
        });

        modelBuilder.Entity<TblUsuario>(entity =>
        {
            entity.HasKey(e => e.Codusuario).HasName("tbl_usuario_pkey");

            entity.ToTable("tbl_usuario");

            entity.Property(e => e.Codusuario).HasColumnName("codusuario");
            entity.Property(e => e.Nome)
                .HasColumnType("character varying")
                .HasColumnName("nome");
            entity.Property(e => e.Senha).HasColumnName("senha");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
