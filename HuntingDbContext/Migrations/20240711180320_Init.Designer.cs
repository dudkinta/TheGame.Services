﻿// <auto-generated />
using System;
using HuntingDbContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace HuntingDbContext.Migrations
{
    [DbContext(typeof(HuntingContext))]
    [Migration("20240711180320_Init")]
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

            modelBuilder.Entity("HuntingDbContext.Models.GameModel", b =>
                {
                    b.Property<Guid>("game_guid")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<byte>("status")
                        .HasColumnType("smallint");

                    b.Property<string>("targets")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("user_id")
                        .HasColumnType("integer");

                    b.HasKey("game_guid");

                    b.ToTable("games", (string)null);
                });
#pragma warning restore 612, 618
        }
    }
}
