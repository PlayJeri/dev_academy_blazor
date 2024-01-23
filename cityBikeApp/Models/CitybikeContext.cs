using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace cityBikeApp.Models;

public partial class CitybikeContext : DbContext
{
    public CitybikeContext(DbContextOptions<CitybikeContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Journey> Journeys { get; set; }

    public virtual DbSet<Station> Stations { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Journey>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("journey_pkey");

            entity.ToTable("journey");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.DepartureDateTime)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("departure_date_time");
            entity.Property(e => e.DepartureStationId).HasColumnName("departure_station_id");
            entity.Property(e => e.Distance).HasColumnName("distance");
            entity.Property(e => e.Duration).HasColumnName("duration");
            entity.Property(e => e.ReturnDateTime)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("return_date_time");
            entity.Property(e => e.ReturnStationId).HasColumnName("return_station_id");

            entity.HasOne(d => d.DepartureStation).WithMany(p => p.JourneyDepartureStations)
                .HasForeignKey(d => d.DepartureStationId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("journey_departure_station_id_fkey");

            entity.HasOne(d => d.ReturnStation).WithMany(p => p.JourneyReturnStations)
                .HasForeignKey(d => d.ReturnStationId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("journey_return_station_id_fkey");
        });

        modelBuilder.Entity<Station>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("station_pkey");

            entity.ToTable("station");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.CoordinateX)
                .HasMaxLength(100)
                .HasDefaultValueSql("NULL::character varying")
                .HasColumnName("coordinate_x");
            entity.Property(e => e.CoordinateY)
                .HasMaxLength(100)
                .HasDefaultValueSql("NULL::character varying")
                .HasColumnName("coordinate_y");
            entity.Property(e => e.StationAddress)
                .HasMaxLength(100)
                .HasDefaultValueSql("NULL::character varying")
                .HasColumnName("station_address");
            entity.Property(e => e.StationName)
                .HasMaxLength(100)
                .HasDefaultValueSql("NULL::character varying")
                .HasColumnName("station_name");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
