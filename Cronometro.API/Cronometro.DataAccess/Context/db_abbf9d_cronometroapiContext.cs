﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using Cronometro.Entities.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace Cronometro.DataAccess.Context;

public partial class db_abbf9d_cronometroapiContext : DbContext
{
    public db_abbf9d_cronometroapiContext()
    { 
    }
    public db_abbf9d_cronometroapiContext(DbContextOptions<db_abbf9d_cronometroapiContext> options)
        : base(options)
    {
    }

    public virtual DbSet<tbProyectosTiempos> tbProyectosTiempos { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<tbProyectosTiempos>(entity =>
        {
            entity.HasKey(e => e.RegistroID).HasName("PK__tbProyec__B897313EA86C5224");

            entity.ToTable("tbProyectosTiempos", "Prod");

            entity.Property(e => e.Descripcion).HasMaxLength(300);
            entity.Property(e => e.EstadoTrabajo).HasDefaultValue(true);
            entity.Property(e => e.FechaSystema).HasColumnType("datetime");
            entity.Property(e => e.HoraFin).HasColumnType("datetime");
            entity.Property(e => e.HoraInicio).HasColumnType("datetime");
            entity.Property(e => e.Nombreusuario).HasMaxLength(30);
            entity.Property(e => e.ProyectoCode).HasMaxLength(30);
            entity.Property(e => e.Referencia).HasMaxLength(20);
            entity.Property(e => e.TotalHoras).HasPrecision(3);
        });

        OnModelCreatingPartial(modelBuilder);
    }



    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}