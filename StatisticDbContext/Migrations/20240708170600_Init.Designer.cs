﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using StatisticDbContext;

#nullable disable

namespace StatisticDbContext.Migrations
{
    [DbContext(typeof(StatisticContext))]
    [Migration("20240708170600_Init")]
    partial class Init
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.4")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("StatisticDbContext.Models.StorageModel", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("id"));

                    b.Property<long>("coin")
                        .HasColumnType("bigint");

                    b.Property<int>("energy")
                        .HasColumnType("integer");

                    b.Property<DateTime>("last_check_energy")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("id");

                    b.ToTable("storage", (string)null);
                });
#pragma warning restore 612, 618
        }
    }
}
